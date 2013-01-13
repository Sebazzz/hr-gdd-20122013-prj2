using System;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;

/// <summary>
///     Executes specific death logic for a shepard dog
/// </summary>
/// <dependency cref="CanDieBehaviour"/>
/// <dependency cref="HerderLoopBehaviour"/>
/// <dependend cref="KillBehaviour"/>
public class DogDeathBehaviour : CanDieBehaviour {
    public DeathEffects.DeathEffectConfiguration WaterDeathEffect = new DeathEffects.DeathEffectConfiguration(0.5f, true, 2f);


    protected override void OnExecuteDeath(GameObject causeOfDeath) {
        // kill the dog
        Object.Destroy(this.gameObject);

        // notify level manager
        LevelBehaviour.Instance.OnDogKilled();
    }

    protected override bool CanDie (GameObject causeOfDeath, string causeOfDeathTag, int causeOfDeathLayer) {
        return causeOfDeathLayer == Layers.Water || causeOfDeathTag == Tags.Trap;
    }

    protected override void OnStartDying (GameObject causeOfDeath) {
        if (causeOfDeath.layer != Layers.Water && causeOfDeath.name.IndexOf("hole", StringComparison.CurrentCultureIgnoreCase) != -1) {
            this.ExecuteDirectDeath();
        } else {
            DeathEffects.WaterDeathEffect.Execute(this.gameObject, causeOfDeath, this.WaterDeathEffect);
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
