using System;
using UnityEngine;
using Object = UnityEngine.Object;


/// <summary>
///     Implements specific logic for the sheep when death is triggered
/// </summary>
/// <dependency cref="LookatShepherdBehaviour"/>
/// <dependency cref="CanDieBehaviour"/>
/// <dependend cref="KillBehaviour"/>
public class SheepDeathBehaviour : CanDieBehaviour {
    private SheepAudioController audioController;

    public GameObject GenericDeadSheep = null;

    public DeathEffects.DeathEffectConfiguration WolfDeathEffect = new DeathEffects.DeathEffectConfiguration(0.5f, true, 1f);

    public DeathEffects.DeathEffectConfiguration WaterDeathEffect = new DeathEffects.DeathEffectConfiguration(0.5f, true, 2f);

    public DeathEffects.DeathEffectConfiguration HoleDeathEffect = new DeathEffects.DeathEffectConfiguration(0.5f, true, 2f);

    public DeathEffects.DeathEffectConfiguration ElecticFenceDeathEffect = new DeathEffects.DeathEffectConfiguration(4f, true, 5f);

    public DeathEffects.DeathEffectConfiguration FireDeathEffect = new DeathEffects.DeathEffectConfiguration(6f, true, 3f);

    void Awake() {
        this.audioController = this.GetComponent<SheepAudioController>();
    }

    protected override void OnExecuteDeath (GameObject causeOfDeath) {
        Object.Destroy(this.gameObject);
    }

    protected override void OnStartDying (GameObject causeOfDeath) {
        if (causeOfDeath.layer != Layers.Water) {
            this.ExecuteDirectDeath();

            // execute object specific behaviour
            // ... electric fence
            if (causeOfDeath.name.IndexOf("fence", StringComparison.InvariantCultureIgnoreCase) != -1) {
                this.audioController.ElectricFenceTouchSound.Play();
                DeathEffects.RagdollTouchDeathEffect.Execute(this.gameObject, causeOfDeath, this.ElecticFenceDeathEffect);
            }

            // ... fire
            if (causeOfDeath.name.IndexOf("fire", StringComparison.InvariantCultureIgnoreCase) != -1 ||
                causeOfDeath.name.IndexOf("flame", StringComparison.InvariantCultureIgnoreCase) != -1) {
                DeathEffects.RagdollTouchDeathEffect.Execute(this.gameObject, causeOfDeath, this.FireDeathEffect);
            }

            // ... hole
            if (causeOfDeath.name.IndexOf("hole", StringComparison.InvariantCultureIgnoreCase) != -1) {
                this.audioController.FallInHoleSound.Play();
                DeathEffects.RagdollTouchDeathEffect.Execute(this.gameObject, causeOfDeath, this.HoleDeathEffect);
            }


            // execute enemy behaviour
            if (causeOfDeath.tag == Tags.Enemy) {
                this.audioController.KilledSound.Play();
                DeathEffects.EnemyTouchDeadEffect.Execute(this.gameObject, causeOfDeath, this.WolfDeathEffect);
            }
        } else {
            // execute water behaviour
            if (causeOfDeath.layer == Layers.Water) {
                this.audioController.FallInWaterSound.Play();
                this.audioController.DrowningSound.Play();
                DeathEffects.WaterDeathEffect.Execute(this.gameObject, causeOfDeath, this.WaterDeathEffect);

                if (this.GenericDeadSheep != null) {
                    DeathEffects.WaterDeathEffect.ExecuteExtra(this.gameObject, causeOfDeath, this.WaterDeathEffect, this.GenericDeadSheep);
                }
            }

        }

        LevelBehaviour.Instance.OnSheepDeath();
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