using System;
using UnityEngine;
using System.Collections;

public class RepelBehaviour : MonoBehaviour {
    public AudioClip SOUND_DOGBARK;

    public float radius = 10.0F;
    public float power = 50.0F;

    private bool hasBarked = false;
    void Update() {

        Vector3 explosionPos = transform.position;

        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);

        bool hasSeenSheep = false;
        foreach (Collider hit in colliders) {

            if (hit.tag == Tags.Sheep && Math.Abs(hit.rigidbody.drag - 0) > 0.01) {
                hit.rigidbody.AddExplosionForce(power, explosionPos, radius, 0);
                hasSeenSheep = true;
            }
        }

        if (!hasBarked && hasSeenSheep) {
            audio.PlayOneShot(SOUND_DOGBARK);
            hasBarked = true;
        }

        if (hasBarked && !hasSeenSheep) { // Reset bark
            hasBarked = false;
        }

    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(this.transform.position, this.radius);
    }
}

