using System;
using UnityEngine;


/// <summary>
///     Implements specific logic for the sheep when death is triggered
/// </summary>
/// <dependency cref="LookatShepherdBehaviour"/>
/// <dependency cref="CanDieBehaviour"/>
/// <dependend cref="KillBehaviour"/>
public class SheepDeathBehaviour : CanDieBehaviour {

    public DeathEffects.DeathEffectConfiguration WaterDeathEffect = new DeathEffects.DeathEffectConfiguration(0.5f, true, 1f);

    public DeathEffects.DeathEffectConfiguration ElecticFenceDeathEffect = new DeathEffects.DeathEffectConfiguration(4f, true, 5f);

    protected override void OnExecuteDeath (GameObject causeOfDeath) {
        LevelBehaviour.Instance.OnSheepDeath();
        Destroy(this.gameObject);

        // execute object specific behaviour
        // ... electric fence
        if (causeOfDeath.name.IndexOf("fence", StringComparison.InvariantCultureIgnoreCase) != -1) {
            DeathEffects.RagdollFenceTouchDeathEffect.Execute(this.gameObject, causeOfDeath, this.ElecticFenceDeathEffect);
        }
    }

    protected override void OnStartDying (GameObject causeOfDeath) {
        if (causeOfDeath.layer != Layers.Water) {
            this.ExecuteDirectDeath();
        } else {
            DeathEffects.WaterDeathEffect.Execute(this.gameObject, causeOfDeath, this.ElecticFenceDeathEffect);
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