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
    private DogAudioController audioController;

    void Awake() {
        this.audioController = this.GetComponent<DogAudioController>();
    }

    public GameObject GenericDeadDog = null;

    public DeathEffects.DeathEffectConfiguration WaterDeathEffect = new DeathEffects.DeathEffectConfiguration(0.5f, true, 2f);

    public DeathEffects.DeathEffectConfiguration HoleDeathEffect = new DeathEffects.DeathEffectConfiguration(0.5f, false, 0.5f);

    public DeathEffects.DeathEffectConfiguration FireDeathEffect = new DeathEffects.DeathEffectConfiguration(0.5f, false, 0.5f);

    public DeathEffects.DeathEffectConfiguration ElecticFenceDeathEffect = new DeathEffects.DeathEffectConfiguration(0.5f, false, 0.5f);

    protected override void OnExecuteDeath(GameObject causeOfDeath) {
        // kill the dog
        Object.Destroy(this.gameObject);

        // notify level manager
        LevelBehaviour.Instance.OnDogKilled();

        // try to release the dog
        if (MouseManager.CurrentLockOwner == this.GetComponent<ControlHerderBehaviour>()) {
            MouseManager.ReleaseLock(this.GetComponent<ControlHerderBehaviour>());
        }
    }

    protected override bool CanDie (GameObject causeOfDeath, string causeOfDeathTag, int causeOfDeathLayer) {
        return causeOfDeathLayer == Layers.Water || causeOfDeathTag == Tags.Trap;
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

        } else {
            // execute water behaviour
            if (causeOfDeath.layer == Layers.Water) {
                this.audioController.FallInWaterSound.Play();
                this.audioController.DrowningSound.Play();
                DeathEffects.WaterDeathEffect.Execute(this.gameObject, causeOfDeath, this.WaterDeathEffect);

                if (this.GenericDeadDog != null) {
                    DeathEffects.WaterDeathEffect.ExecuteExtra(this.gameObject, causeOfDeath, this.WaterDeathEffect, this.GenericDeadDog);
                }
            }

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
