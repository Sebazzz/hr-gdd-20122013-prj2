using UnityEngine;
using System.Collections;

/// <summary>
///     Executes specific death logic for a shepard dog
/// </summary>
/// <dependency cref="CanDieBehaviour"/>
/// <dependency cref="HerderLoopBehaviour"/>
/// <dependend cref="KillBehaviour"/>
public class DogDeathBehaviour : CanDieBehaviour {
    protected override void OnExecuteDeath (GameObject causeOfDeath) {
        // kill the dog
        Destroy(this.gameObject);
    }

    protected override bool CanDie (GameObject causeOfDeath, string causeOfDeathTag, int causeOfDeathLayer) {
        return causeOfDeathLayer == Layers.Water;
    }

    protected override void OnStartDying (GameObject causeOfDeath) {
        if (causeOfDeath.layer != Layers.Water) {
            this.ExecuteDirectDeath();
        }

        if (this.DisableScriptsWhenDying) {
            // kill any path
            HerderLoopBehaviour ch = this.gameObject.GetComponent<HerderLoopBehaviour>();
            if (ch != null) {
                ch.CancelWalk();
            }
        }

        base.OnStartDying(causeOfDeath);
    }
}
