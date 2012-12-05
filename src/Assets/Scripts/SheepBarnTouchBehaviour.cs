using UnityEngine;
using System.Collections;

public class SheepBarnTouchBehaviour : RemoveCollidersBehaviour {

	// Use this for initialization
	void Start () {
	    this.TagToMatch = Tags.Sheep;
	}

    protected override void OnObjectCollision (GameObject collidingObject) {
        // TODO: do something like ending the level

        AudioSource audioSource = this.GetComponent<AudioSource>();
        if (audioSource != null) {
            audioSource.Play();
        }

        // call base logic
        base.OnObjectCollision(collidingObject);
    }
}
