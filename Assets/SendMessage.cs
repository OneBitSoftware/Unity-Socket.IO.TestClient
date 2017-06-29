using Assets.EventArgs;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SendMessage : MonoBehaviour
{
    public InputField MessageField;

    /// <summary>
    /// Send message button click event
    /// </summary>
    public void SendMessage_Click()
    {
        if (string.IsNullOrEmpty(MessageField.text)) return;

        var messageToSend = string.Empty;
        var socketService = SocketService.Instance;
        if (socketService == null || !socketService.Connected) // make sure we're connected
        {   
            messageToSend = "Socket not connected";
            MessageBroker.Default.Publish(new MessageEventArgs() { Message = messageToSend });
            // don't send to socket on error
        }
        else
        {
            messageToSend = MessageField.text;
            MessageBroker.Default.Publish(new MessageEventArgs() { Message = messageToSend });
            MessageBroker.Default.Publish(new SocketMessageEventArgs() { Message = messageToSend });
        }

        MessageField.text = string.Empty; // clear the input field
    }
}
