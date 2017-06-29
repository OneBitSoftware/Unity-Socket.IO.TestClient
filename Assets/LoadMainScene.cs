using UnityEngine;

public class LoadMainScene : MonoBehaviour {
	void Start () {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainApplicationScene");
    }
}
