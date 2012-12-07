using UnityEngine;


/// <summary>
///     Executes specific death logic for a wolf
/// </summary>
public class WolfDeathBehaviour : CanDieBehaviour {
    public override void Die (GameObject causeOfDeath) {
        if (causeOfDeath.tag == Tags.Sheep) {
            Destroy(this.gameObject);
        }
    }
}