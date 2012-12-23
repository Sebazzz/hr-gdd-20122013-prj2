using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Causes the object this script is attached to, to have a magnetic behaviour.
/// </summary>
public sealed class MagneticBehaviour : MonoBehaviour {
    /// <summary>
    /// Specifies the tag to attract. If set to null or an empty value, any object with an non-kinematic <see cref="Rigidbody"/> is attracted.
    /// </summary>
    public string TagToAttract = null;

    /// <summary>
    /// Specifies the magnetic force to apply within the specified <see cref="Radius"/>
    /// </summary>
    public float MagneticForce = 5f;

    /// <summary>
    /// Specifies the radius on which the magnetic force attracts objects
    /// </summary>
    public float Radius = 10f;


    void FixedUpdate() {
        Collider[] collidersInRadius = Physics.OverlapSphere(this.transform.position, this.Radius, Layers.Default);

        // process each collider that meet the condition
        foreach (Collider col in collidersInRadius) {
            // check for the correct tag
            if (!String.IsNullOrEmpty(this.TagToAttract) && col.gameObject.tag != this.TagToAttract) {
                continue;
            }

            // we require a rigid body
            Rigidbody collidingBody = col.gameObject.GetComponent<Rigidbody>();
            if (collidingBody == null) {
                continue;
            }

            this.AttractObject(col.gameObject, collidingBody);
        }
    }

    private void AttractObject (GameObject collidingObject, Rigidbody collidingBody) {
        Vector3 targetPosition = this.transform.position - collidingObject.transform.position;
        targetPosition.y = -0.1f;

        collidingBody.AddForce(targetPosition * this.MagneticForce, ForceMode.Impulse);
    }

    void OnDrawGizmosSelected() {
        // draw radius gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, this.Radius);

        // draw marker gizmo
        Color c = Color.red;
        c.a = 0.5f;
        Gizmos.color = c;
        Gizmos.DrawSphere(this.transform.position, 0.25f);
    }
}