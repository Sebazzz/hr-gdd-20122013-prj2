using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Represents a script that draws a design-time script for level bounds
/// </summary>
public sealed class LevelBoundsDesignTimeController : MonoBehaviour {

    void OnDrawGizmos() {
        BoxCollider theCollider = this.GetComponent<BoxCollider>();

        // draw the collider
        Gizmos.color = new Color(1, 1, 1, 0.5f);

        Vector3 cubeSize = theCollider.size;
        if (Mathf.Approximately(Mathf.Abs(this.transform.rotation.eulerAngles.y % 180f), 90f)) {
            float z = cubeSize.z;
            cubeSize.z = cubeSize.x;
            cubeSize.x = z;
        }

        Gizmos.DrawCube(this.transform.position + theCollider.center, cubeSize);
    }
}
