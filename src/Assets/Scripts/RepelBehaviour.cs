using System;
using UnityEngine;

public class RepelBehaviour : MonoBehaviour {
    private DogAudioController audioController;

    private bool hasBarked;
    public float power = 50.0F;
    public float radius = 10.0F;

    private void Awake() {
        this.audioController = this.GetComponent<DogAudioController>();
    }

    private void Update() {
        Vector3 explosionPos = this.transform.position;

        Collider[] colliders = Physics.OverlapSphere(explosionPos, this.radius);

        bool hasSeenSheep = false;
        foreach (Collider hit in colliders) {
            if (hit.tag == Tags.Sheep && Math.Abs(hit.rigidbody.drag - 0) > 0.01) {
                hit.rigidbody.AddExplosionForce(this.power, explosionPos, this.radius, 0);
                hasSeenSheep = true;
            }
        }

        if (this.audioController != null) {
            if (!this.hasBarked && hasSeenSheep) {
                this.audioController.SheepProximitySound.Play();
                this.hasBarked = true;
            }

            if (this.hasBarked && !hasSeenSheep) {
                // Reset bark
                this.hasBarked = false;
            }
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(this.transform.position, this.radius);
    }
}