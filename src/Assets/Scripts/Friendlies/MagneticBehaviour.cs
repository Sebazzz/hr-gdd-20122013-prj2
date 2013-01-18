using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Causes the object this script is attached to, to have a magnetic behaviour.
/// </summary>
public sealed class MagneticBehaviour : MonoBehaviour {
    private const float MinimumInnerRadius = 0.5f;

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

    /// <summary>
    /// Specifies the radius in which magnetic force is no longer applied. A value lower than <see cref="MinimumInnerRadius"/> has no effect here.
    /// </summary>
    public float InnerRadius = 0f;

    /// <summary>
    /// Specifies if the 2D inner radius should be performed in 2d space by comparing X/Z values of each object leaving Y (height) out of the calculation
    /// </summary>
    public bool Perform2DInnerRadiusCheck = true;


    void Update() {
        Collider[] collidersInRadius = Physics.OverlapSphere(this.transform.position, this.Radius, Layers.Default);

        // process each collider that meet the condition
        foreach (Collider col in collidersInRadius) {
            // check for the correct tag
            if (!String.IsNullOrEmpty(this.TagToAttract) && col.gameObject.tag != this.TagToAttract) {
                continue;
            }

            // check if its in the inner radius
            if (this.InnerRadiusCheck(col.transform)) {
                continue;
            }

            // we require a rigid body
            Rigidbody collidingBody = col.gameObject.GetComponent<Rigidbody>();
            if (collidingBody == null) {
                continue;
            }
           
            // check if it has drag, else we cannot do anything
            if (Math.Abs(collidingBody.drag - 0) < 0.001) {
                //Debug.Log("NO "+ collidingBody.drag.ToString());
                continue;
            }

            this.AttractObject(col.gameObject, collidingBody);
        }
    }

    /// <summary>
    /// Checks if the specified transform is in the inner radius of the current object
    /// </summary>
    /// <param name="otherObject"></param>
    /// <returns></returns>
    private bool InnerRadiusCheck(Transform otherObject) {
        if (this.InnerRadius <= MinimumInnerRadius) {
            return false;
        }

        if (this.Perform2DInnerRadiusCheck) {
            return Vector2.Distance(new Vector2(this.transform.position.x, this.transform.position.z),
                                    new Vector2(otherObject.position.x, otherObject.position.z)) < this.InnerRadius;
        }

        return Vector3.Distance(this.transform.position, otherObject.position) < this.InnerRadius;
    }

    private void AttractObject (GameObject collidingObject, Rigidbody collidingBody) {
        Vector3 targetPosition = this.transform.position - collidingObject.transform.position;
        targetPosition.y = -0.25f;

        collidingBody.AddForce(targetPosition * this.MagneticForce, ForceMode.Impulse);
    }

    void OnDrawGizmosSelected() {
        if (!this.enabled) {
            return;
        }

        // draw radius gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, this.Radius);

        // draw inner radius
        if (this.InnerRadius >= MinimumInnerRadius) {
            Color c2 = Color.black;
            c2.a = 0.5f;
            Gizmos.color = c2;
            Gizmos.DrawWireSphere(this.transform.position, this.InnerRadius);
        }

        // draw marker gizmo
        Color c = Color.red;
        c.a = 0.5f;
        Gizmos.color = c;
        Gizmos.DrawSphere(this.transform.position, 0.25f);

        
    }
}