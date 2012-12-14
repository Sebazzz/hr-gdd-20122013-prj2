using UnityEngine;
public class SheepBarnTouchBehaviour : RemoveCollidersBehaviour {

	// Use this for initialization
	void Start () {
	    this.TagToMatch = Tags.Sheep;
	}

    protected override void OnObjectCollision (GameObject collidingObject) {
        AudioSource audioSource = this.GetComponent<AudioSource>();
        if (audioSource != null) {
            audioSource.Play();
        }

        LevelBehaviour.Instance.OnSheepCollected();

        // call base logic
        base.OnObjectCollision(collidingObject);
    }
}
