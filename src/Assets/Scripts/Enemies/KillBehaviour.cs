using UnityEngine;


/// <summary>
///     Executes death logic on colliding object if the object contains the <see cref="CanDieBehaviour" />-derived component
/// </summary>
public class KillBehaviour : MonoBehaviour {
    private void OnCollisionEnter (Collision collision) {
        // check for the 'can die' component
        GameObject collidingGameObject = collision.gameObject;
        CanDieBehaviour dieComponent = collidingGameObject.GetComponent<CanDieBehaviour>();

        if (dieComponent == null) {
            return;
        }

        // execute death logic
        dieComponent.Die(this.gameObject);
    }

    private void OnTriggerEnter (Collider contextCollider) {
        // check for the 'can die' component
        GameObject collidingGameObject = contextCollider.gameObject;
        CanDieBehaviour dieComponent = collidingGameObject.GetComponent<CanDieBehaviour>();

        if (dieComponent == null) {
            return;
        }

        // execute death logic
        dieComponent.Die(this.gameObject);
    }


    private void OnControllerColliderHit(ControllerColliderHit hit) {
        // check for the 'can die' component
        GameObject collidingGameObject = hit.gameObject;
        CanDieBehaviour dieComponent = collidingGameObject.GetComponent<CanDieBehaviour>();

        if (dieComponent == null) {
            return;
        }

        // execute death logic
        dieComponent.Die(this.gameObject);
    }
}