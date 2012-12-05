using System;
using UnityEngine;
using Object = UnityEngine.Object;


/// <summary>
/// Lets the current object die when colliding with an object of tag '<see cref="Tags.Foe"/>', thus: enemies.
/// </summary>
public class DeathOnEnemyCollisionBehaviour : RemoveCollidersBehaviour {

	void Start () {
	    this.TagToMatch = Tags.Foe;
	}

    protected override void OnObjectCollision (GameObject collidingObject) {
        // in this case, let the current object die because this behaviour
        // is attached to object vulnerable to enemies
        Object.Destroy(this.gameObject);
    }
}
