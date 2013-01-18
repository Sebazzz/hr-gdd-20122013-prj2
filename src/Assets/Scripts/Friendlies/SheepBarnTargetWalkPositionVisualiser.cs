using UnityEngine;

/// <summary>
/// Draws a simple design-time visualisation of where object will walk to in the barn
/// </summary>
public sealed class SheepBarnTargetWalkPositionVisualiser : MonoBehaviour {
    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(this.transform.position, 0.5f);
    }
}