using Assets.EventArgs;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ConnectButton : MonoBehaviour
{
    public Text ServerField;
    public InputField SocketCount;
    public InputField LoopDelay;

    private IObservable<long> _loopObservable;
    private IDisposable _loopSubscription;

    public void Connect_Click()
    {
        if (string.IsNullOrEmpty(ServerField.text)) return;

        SocketService.Instance.ConnectSocket(ServerField.text);
    }

    public void Disconnect_Click()
    {
        StopLoop();
        MessageBroker.Default.Publish(new DisconnectMessageEventArgs());
    }

    public void StartLoop_Click()
    {
        StartEmitLoop();
    }

    public void StopLoop_Click()
    {
        StopLoop();
    }

    private void StopLoop()
    {
        if (_loopSubscription != null)
        {
            _loopSubscription.Dispose();
        }
    }

    private void StartEmitLoop()
    {
        if (SocketCount == null || LoopDelay == null)
        {
            MessageBroker.Default.Publish(new MessageEventArgs() { Message = "Controls are null" });
        }

        if (_loopObservable == null)
        {
            _loopObservable = Observable.Interval(TimeSpan.FromMilliseconds(Convert.ToDouble(LoopDelay.text)));
        }

        _loopSubscription = _loopObservable.Subscribe(x => {
            MessageBroker.Default.Publish(new SocketMessageEventArgs()
            {
                Message = "Sent from loop " + DateTime.Now.Ticks
            });
        }, e => {
            Debug.Log("Error: " + e.ToString());
        }, () => {
            Debug.Log("Loop completed.");
        });
    }
}
