using System;
using UnityEngine;


/// <summary>
///     Implements specific logic for the sheep when death is triggered
/// </summary>
/// <dependency cref="LookatShepherdBehaviour"/>
/// <dependency cref="CanDieBehaviour"/>
/// <dependend cref="KillBehaviour"/>
public class SheepDeathBehaviour : CanDieBehaviour {
    public AudioClip SOUND_KILL;
    public AudioClip SOUND_FALLHOLE;

    public GameObject GenericDeadSheep = null;

    public DeathEffects.DeathEffectConfiguration WolfDeathEffect = new DeathEffects.DeathEffectConfiguration(0.5f, true, 1f);

    public DeathEffects.DeathEffectConfiguration WaterDeathEffect = new DeathEffects.DeathEffectConfiguration(0.5f, true, 2f);

    public DeathEffects.DeathEffectConfiguration HoleDeathEffect = new DeathEffects.DeathEffectConfiguration(0.5f, true, 2f);

    public DeathEffects.DeathEffectConfiguration ElecticFenceDeathEffect = new DeathEffects.DeathEffectConfiguration(4f, true, 5f);

    public DeathEffects.DeathEffectConfiguration FireDeathEffect = new DeathEffects.DeathEffectConfiguration(6f, true, 3f);


    protected override void OnExecuteDeath (GameObject causeOfDeath) {
        LevelBehaviour.Instance.OnSheepDeath();
        Destroy(this.gameObject);


        // execute object specific behaviour
        // ... electric fence
        if (causeOfDeath.name.IndexOf("fence", StringComparison.InvariantCultureIgnoreCase) != -1) {
            DeathEffects.RagdollTouchDeathEffect.Execute(this.gameObject, causeOfDeath, this.ElecticFenceDeathEffect);
        }

        // ... fire
        if (causeOfDeath.name.IndexOf("fire", StringComparison.InvariantCultureIgnoreCase) != -1 ||
            causeOfDeath.name.IndexOf("flame", StringComparison.InvariantCultureIgnoreCase) != -1) {
            DeathEffects.RagdollTouchDeathEffect.Execute(this.gameObject, causeOfDeath, this.FireDeathEffect);
        }

        // ... hole
        if (causeOfDeath.name.IndexOf("hole", StringComparison.InvariantCultureIgnoreCase) != -1) {
            audio.PlayOneShot(SOUND_FALLHOLE);
            DeathEffects.RagdollTouchDeathEffect.Execute(this.gameObject, causeOfDeath, this.HoleDeathEffect);
        }
    }

    protected override void OnStartDying (GameObject causeOfDeath) {
        if (causeOfDeath.layer != Layers.Water && causeOfDeath.tag != Tags.Enemy) {
            this.ExecuteDirectDeath();
        } else {
            // execute water behaviour
            if (causeOfDeath.layer == Layers.Water) {
                DeathEffects.WaterDeathEffect.Execute(this.gameObject, causeOfDeath, this.WaterDeathEffect);

                if (this.GenericDeadSheep != null) {
                    DeathEffects.WaterDeathEffect.ExecuteExtra(this.gameObject, causeOfDeath, this.WaterDeathEffect, this.GenericDeadSheep);
                }
            }

            // execute enemy behaviour
            if (causeOfDeath.tag == Tags.Enemy) {
                audio.PlayOneShot(SOUND_KILL);
                DeathEffects.EnemyTouchDeadEffect.Execute(this.gameObject, causeOfDeath, this.WolfDeathEffect);
            }
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