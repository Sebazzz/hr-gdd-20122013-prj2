using System;
using UnityEngine;
using System.Collections;

public class RepelBehaviour : MonoBehaviour {

    public float radius = 10.0F;
    public float power = 50.0F;

    void Update() {

        Vector3 explosionPos = transform.position;

        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);

        foreach (Collider hit in colliders) {

            if (hit.tag == Tags.Sheep && Math.Abs(hit.rigidbody.drag - 0) > 0.01)

                hit.rigidbody.AddExplosionForce(power, explosionPos, radius, 0);



        }

    }


    //void Update() {
    //    // Get average sheep position
    //    Vector3 average = new Vector3();
    //    int amount = 0;

    //    Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
    //    foreach (Collider hit in colliders) {
    //        if (hit.tag == Tags.Sheep) {
    //            average += hit.transform.position;
    //            amount++;
    //        }
    //    }

    //    // If no sheep are found, we can stop here
    //    if (amount == 0) {
    //        return;
    //    }

    //    // Calculate direction
    //    average /= amount;
    //    Vector3 dir = (average - transform.position).normalized;
    //    dir.y = 0;
    //    // Apply forces
    //    foreach (Collider hit in colliders) {
    //        if (hit.tag == Tags.Sheep) {
    //            applyForce(dir * power, hit.rigidbody);
    //        }
    //    }
    //}

    //private void applyForce(Vector3 direction, Rigidbody body) {
    //    body.AddForce(direction);
    //}

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(this.transform.position, this.radius);
    }
}

