using UnityEngine;


/// <summary>
///     Implements specific logic for the sheep when death is triggered
/// </summary>
/// <dependency cref="LookatShepherdBehaviour"/>
/// <dependency cref="CanDieBehaviour"/>
/// <dependend cref="KillBehaviour"/>
public class SheepDeathBehaviour : CanDieBehaviour {
    protected override void OnExecuteDeath (GameObject causeOfDeath) {
        LevelBehaviour.Instance.OnSheepDeath();
        Destroy(this.gameObject);
    }

    protected override void OnStartDying (GameObject causeOfDeath) {
        if (causeOfDeath.layer != Layers.Water) {
            this.ExecuteDirectDeath();
        }

        if (this.DisableScriptsWhenDying) {
            this.DisableScriptIfExists<LookatShepherdBehaviour>();
        }

        base.OnStartDying(causeOfDeath);
    }

    protected override bool CanDie (GameObject causeOfDeath, string causeOfDeathTag, int causeOfDeathLayer) {
        // we only die because of traps or enemies
        if (causeOfDeath.tag != Tags.Enemy &&
            causeOfDeath.tag != Tags.Trap) {
            return false;
        }

        return true;
    }
}