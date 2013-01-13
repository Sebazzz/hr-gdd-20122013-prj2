using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public sealed class SheepBarnTargetWalkPositionVisualiser : MonoBehaviour {
    void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(this.transform.position, 0.5f);
    }
}
