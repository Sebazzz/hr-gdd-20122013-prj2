using UnityEngine;


/// <summary>
///     Implements specific logic for the sheep when death is triggered
/// </summary>
public class SheepDeathBehaviour : CanDieBehaviour {
    public override void Die (GameObject causeOfDeath) {
        // we only die because of traps or enemies
        if (causeOfDeath.tag != Tags.Enemy) {
            return;
        }

        LevelBehaviour.Instance.OnSheepDeath();
        Destroy(this.gameObject);
    }
}