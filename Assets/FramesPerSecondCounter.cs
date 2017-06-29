using UnityEngine;
using UnityEngine.UI;

public class FramesPerSecondCounter : MonoBehaviour {

    public Text FpsValueLabel;
    float deltaTime = 0f;
    
	// Update is called once per frame
	void Update () {
        if (FpsValueLabel == null) return;

        deltaTime += (Time.deltaTime - deltaTime) * .1f;

        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;

        FpsValueLabel.text = string.Format("{1:0.} ({0:0.0} ms)", msec, fps);
    }
}
