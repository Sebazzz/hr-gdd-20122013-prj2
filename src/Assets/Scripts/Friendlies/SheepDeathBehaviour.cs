using UnityEngine;


/// <summary>
///     Implements specific logic for the sheep when death is triggered
/// </summary>
public class SheepDeathBehaviour : CanDieBehaviour {
    public override void Die (GameObject causeOfDeath) {
        if (causeOfDeath.tag != Tags.Enemy) {
            return;
        }

        Destroy(this.gameObject);
    }
}