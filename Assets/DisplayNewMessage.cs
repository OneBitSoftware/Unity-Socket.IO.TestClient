using Assets.EventArgs;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DisplayNewMessage : MonoBehaviour {

    public GameObject NewMessagePrefab;
    public Transform ContentContainer;
    
    // Use this for initialization
    void Start () {
        // Capture/subscripte to the global message event
        MessageBroker.Default.Receive<MessageEventArgs>().Subscribe(args =>
        {
            if (string.IsNullOrEmpty(args.Message)) return;
            if (NewMessagePrefab == null) return;
            if (ContentContainer == null) return;

            // Queue/schedule a UI update with the message
            MainThreadDispatcher.Instance.Enqueue((message) => {

                // Clone the prefab (must be executed on the main thread
                var clone = (GameObject)Instantiate(NewMessagePrefab);
                clone.transform.SetParent(ContentContainer);

                // Find the child text control and set its message
                var textBox = clone.GetComponentInChildren<Text>();
                if (textBox != null) {
                    textBox.text = String.Format("[{0}]: {1}", DateTime.Now.ToLongTimeString(), message);
                    textBox.transform.localScale = new Vector3(1, 1, 1);
                }
            }, args.Message);
        });
    }
}
