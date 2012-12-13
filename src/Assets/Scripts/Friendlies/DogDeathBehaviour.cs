using UnityEngine;
using System.Collections;

/// <summary>
///     Executes specific death logic for a shepard dog
/// </summary>
/// <dependency cref="CanDieBehaviour"/>
/// <dependency cref="ControlHerderBehaviour"/>
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
            ControlHerderBehaviour ch = this.gameObject.GetComponent<ControlHerderBehaviour>();
            if (ch != null) {
                ch.done();
                ch.enabled = false;
            }
        }

        base.OnStartDying(causeOfDeath);
    }
}
