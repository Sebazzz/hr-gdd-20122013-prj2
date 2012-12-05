using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Causes objects of this type to die when colliding with enemies. Executes special logic on top of existing object removal code.
/// </summary>
public class SheepDeathOnEnemyCollisionBehaviour : DeathOnEnemyCollisionBehaviour {
    protected override void OnObjectCollision (GameObject collidingObject) {
        // TODO: substract points, detect game over?

        AudioSource audioSource = this.GetComponent<AudioSource>();
        if (audioSource != null) {
            audioSource.Play();
        }

        // let executing logic complete
        base.OnObjectCollision(collidingObject);
    }

}
