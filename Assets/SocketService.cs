using Assets.EventArgs;
using Socket.IO.NET35;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniRx;
using UnityEngine;

public class SocketService
{
    public Guid Id;
    public bool Connected = false;

    private Socket.IO.NET35.Socket MainSocket = null;
    private static SocketService _instance = null;
    private bool CallbacksRegistered = false;
    public static SocketService Instance
    {
        get
        {
            if (_instance == null)
            {
                ConstructInstance(); // Instantiate singleton on first use.
            }

            return _instance;
        }
    }

    private static SocketService ConstructInstance()
    {
        _instance = new SocketService();
        _instance.Id = Guid.NewGuid();

        return _instance;
    }

    private void RegisterCallbackFunctions(string server)
    {
        if (CallbacksRegistered == false)
        {
            // Events received from the socket
            MainSocket.On(Socket.IO.NET35.Socket.EVENT_CONNECT, () => OnConnect());
            MainSocket.On(Socket.IO.NET35.Socket.EVENT_RECONNECT, () => OnReconnect());
            MainSocket.On(Socket.IO.NET35.Socket.EVENT_RECONNECTING, () => OnReconnect());
            MainSocket.On(Socket.IO.NET35.Socket.EVENT_DISCONNECT, () => OnServerSideDisconnect());
            MainSocket.On("hi", (message) =>
            {
                var messageAsString = message as string;
                MessageBroker.Default.Publish(new MessageEventArgs() { Message = messageAsString });
            });

            MainSocket.On("echo", (message) =>
            {
                var messageAsString = message as string;
                MessageBroker.Default.Publish(new MessageEventArgs() { Message = "Echo: " + messageAsString });
            });

            // Events received from the App
            MessageBroker.Default.Receive<SocketMessageEventArgs>().Subscribe(args =>
            {
                FireOnMessage(args.Message);
            });

            MessageBroker.Default.Receive<DisconnectMessageEventArgs>().Subscribe(args =>
            {
                DisconnectSocket();
            });

            CallbacksRegistered = true; 
        }
    }

    public void ConnectSocket(string server)
    {
        if (Connected)
        {
            MessageBroker.Default.Publish(new MessageEventArgs() { Message = "Socket already connected. Cancelling connect request."});
            return;
        }

        if (MainSocket == null)
        {
            // create new socket, but do not autoconnect
            MainSocket = IO.Socket(server, new IO.Options() { AutoConnect = false } );
        }

        if (CallbacksRegistered == false)
        {
            RegisterCallbackFunctions(server);
        }

        // TODO: the socket should expose a connected property, will come soon
        MainSocket.Connect();
    }

    private void OnConnect()
    {
        Connected = true;
        Debug.Log("SocketService connected: " + Id.ToString());
        MainSocket.Emit("hi", "Connect Hi from " + Id.ToString());
    }

    private void OnReconnect()
    {
        Connected = true;
        Debug.Log("SocketService RECONNECTED: " + Id.ToString());
        MainSocket.Emit("hi", "Reconnect Hi from " + Id.ToString());
    }

    public void DisconnectSocket()
    {
        if (Connected == false)
        {
            MessageBroker.Default.Publish(new MessageEventArgs() { Message = "Socket not connected. Ignoring disconnect." });
            return;
        }

        Connected = false;
        Debug.Log("SocketService disconnected from client: " + Id.ToString());
        MainSocket.Disconnect();
        MainSocket.Close();
        //MainSocket = null;
        MessageBroker.Default.Publish(new MessageEventArgs() { Message = "Socket disconnected." });

    }

    private void OnServerSideDisconnect()
    {
        Connected = false;
        Debug.Log("SocketService disconnected from server: " + Id.ToString());
        MainSocket.Disconnect();
        MainSocket.Close();
    }

    private void FireOnMessage(string message)
    {
        // TODO: connected check
        if (Connected == false) return;

        MainSocket.Emit("hi", message);
    }
}
