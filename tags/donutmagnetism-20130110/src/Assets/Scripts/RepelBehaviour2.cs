using UnityEngine;
using System.Collections;

public class RepelBehaviour2 : MonoBehaviour {

    public float width = 10.0f;
    public float height = 2.0f;

    public float power = 50.0F;

    private float radius = 0;
    private int amountOfSpheres = 0;

    void Start() {
        // Sphere needs a radius. get the smallest one.
        // Then height/width amount of spheres are cast
        if (width < height) {
            radius = width;
            amountOfSpheres = (int)Mathf.Ceil(height / width);
        } else {
            radius = height;
            amountOfSpheres = (int)Mathf.Ceil(width / height);
        }


    }

    void Update() {
        // Get average sheep position
        Vector3 average = new Vector3();
        float amount = 0;

        for (int i = -(amountOfSpheres / 2); i < amountOfSpheres / 2; i++) {
            Vector3 position = transform.TransformDirection(i * radius, 0, 0);
            Collider[] colliders = Physics.OverlapSphere(position, radius);
            foreach (Collider hit in colliders) {
                if (hit.tag == Tags.Sheep) {
                    average += hit.transform.position;
                    amount++;
                }
            }
        }

        // If no sheep are found, we can stop here
        if (amount == 0) {
            return;
        }

        // Calculate direction
        average /= amount;
        Vector3 dir = (average - transform.position).normalized;
        dir.y = 0; // Negeer de hoogte
        // Apply forces

        for (int i = -(amountOfSpheres / 2); i < amountOfSpheres / 2; i++) {
            Vector3 position = transform.TransformDirection(i * radius, 0, 0);
            Collider[] colliders = Physics.OverlapSphere(position, radius);
            foreach (Collider hit in colliders) {
                if (hit.tag == Tags.Sheep) {
                    applyForce(dir * power, hit.rigidbody);
                }
            }
        }
    }

    private void applyForce(Vector3 direction, Rigidbody body) {
        body.AddForce(direction);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(this.transform.position, this.radius);
    }
}
