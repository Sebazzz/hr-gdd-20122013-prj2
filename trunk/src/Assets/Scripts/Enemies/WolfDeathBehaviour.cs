using UnityEngine;


/// <summary>
///     Executes specific death logic for a wolf
/// </summary>
/// <dependency cref="CanDieBehaviour"/>
/// <dependend cref="KillBehaviour"/>
public class WolfDeathBehaviour : CanDieBehaviour {
    public DeathEffects.DeathEffectConfiguration WaterDeathEffect = new DeathEffects.DeathEffectConfiguration(0.5f, true, 2f);

    protected override void OnExecuteDeath(GameObject causeOfDeath) {
        Destroy(this.gameObject);
    }

    protected override void OnStartDying(GameObject causeOfDeath) {
        if (causeOfDeath.layer != Layers.Water) {
            this.ExecuteDirectDeath();
        } else {
            // execute water behaviour
            if (causeOfDeath.layer == Layers.Water) {
                DeathEffects.WaterDeathEffect.Execute(this.gameObject, causeOfDeath, this.WaterDeathEffect);
            }
        }

        if (this.DisableScriptsWhenDying) {
            this.DisableScriptIfExists<FoxMoveBehaviour>();
        }

        base.OnStartDying(causeOfDeath);
    }

    protected override bool CanDie (GameObject causeOfDeath, string causeOfDeathTag, int causeOfDeathLayer) {
        return /*causeOfDeath.tag == Tags.Sheep ||*/ causeOfDeath.tag == Tags.Trap || causeOfDeath.layer == Layers.Water;
    }
}