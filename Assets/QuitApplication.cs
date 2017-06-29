using UnityEngine;

public class QuitApplication : MonoBehaviour {

    void OnApplicationQuit()
    {
        SocketService.Instance.DisconnectSocket();
    }
}
