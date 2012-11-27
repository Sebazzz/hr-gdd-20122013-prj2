using UnityEngine;
using System.Collections;

public class ChangeShaderColor : MonoBehaviour {
    private Timer colorChangerTimer;

	// Use this for initialization
	void Start () {
	    colorChangerTimer = new Timer(20);
	}
	
	// Update is called once per frame
	void Update () {
	    colorChangerTimer.Update();

        if (colorChangerTimer.IsTriggered) {
            colorChangerTimer.Reset();

            this.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0);
        }
	}
}
