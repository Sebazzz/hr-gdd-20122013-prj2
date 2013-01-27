using UnityEngine;

/// <summary>
/// Provides configuration of a jump zone. Design-time gizmos indicate the 2d size of the jump zone.
/// </summary>
public class JumpZoneConfiguration : MonoBehaviour {
    /// <summary>
    /// Defines how much force is applied up once the object enters the jump zone
    /// </summary>
    public float ForceUp = 1000;

    /// <summary>
    /// Defines how much force is applied forward once the object enters the jump zone
    /// </summary>
    public float ForceForward = 100f;

    void OnDrawGizmos() {
        const int numberOfSpheres = 10;
        const float sphereRadius = 0.75f;
        Gizmos.color = Color.grey;

        // get current collider and determine center and size to draw
        Collider currentCollider = this.gameObject.collider;
        
        Vector3 center = new Vector3(5,5,5);
        Vector3 size = new Vector3(5,5,5);

        CapsuleCollider cc = currentCollider as CapsuleCollider;
        if (cc != null) {
            center = cc.center;
            size = new Vector3(cc.height, cc.radius, cc.radius);
        }

        BoxCollider bc = currentCollider as BoxCollider;
        if (bc != null) {
            center = bc.center;
            size = bc.size;
        }

        // calculate the positions for the spheres
        // ... calculate the bottom y for the spheres to stick into the ground
        Vector3 centerPosition = this.transform.position + center;
        Vector3 bottom = centerPosition;
        bottom.y -= center.y;
        bottom.y -= size.y;

        // ... calculate the begin and end of our virtual line
        this.DrawFloorLine(bottom, size, this.transform.TransformDirection(Vector3.back) * sphereRadius, numberOfSpheres, sphereRadius);
        this.DrawFloorLine(bottom, size, this.transform.TransformDirection(Vector3.forward) * sphereRadius, numberOfSpheres, sphereRadius);
    }

    private void DrawFloorLine (Vector3 bottom, Vector3 size, Vector3 add, int numberOfSpheres, float sphereRadius) {
        Vector3 begin = bottom + add+ (this.transform.TransformDirection(Vector3.left) * (size.x / 2f));
        Vector3 end = bottom + add +  (this.transform.TransformDirection(Vector3.right) * (size.x / 2f));

        Vector3 diff = end - begin;
        Vector3 diffPerSphere = diff / numberOfSpheres;

        for (int i = 0; i < numberOfSpheres; i++) {
            Vector3 newPosition = (begin + this.transform.TransformDirection(Vector3.right)) + (diffPerSphere * i);
            newPosition.y = bottom.y;

            Gizmos.DrawSphere(newPosition, sphereRadius);
        }
    }
}
