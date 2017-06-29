public class DontDestroyOnLoadScript : UnityEngine.MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
