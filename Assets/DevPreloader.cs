using UnityEngine;
#if UNITY_EDITOR
 using UnityEditor;
#endif
public class DevPreloader : MonoBehaviour {

	// Use this for initialization
	void Start () {
#if UNITY_EDITOR
            //UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScene");
#endif

    }
}
