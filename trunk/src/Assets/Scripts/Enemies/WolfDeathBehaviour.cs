using UnityEngine;


/// <summary>
///     Executes specific death logic for a wolf
/// </summary>
/// <dependency cref="CanDieBehaviour"/>
/// <dependend cref="KillBehaviour"/>
public class WolfDeathBehaviour : CanDieBehaviour {
    protected override void OnExecuteDeath (GameObject causeOfDeath) {
        Destroy(this.gameObject);
    }

    protected override void OnStartDying(GameObject causeOfDeath) {
        if (causeOfDeath.layer != Layers.Water) {
            this.ExecuteDirectDeath();
        }

        if (this.DisableScriptsWhenDying) {
            this.DisableScriptIfExists<FoxMoveBehaviour>();
        }

        base.OnStartDying(causeOfDeath);
    }

    protected override bool CanDie (GameObject causeOfDeath, string causeOfDeathTag, int causeOfDeathLayer) {
        return causeOfDeath.tag == Tags.Sheep || causeOfDeath.layer == Layers.Water;
    }
}