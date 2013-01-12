using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Executes delayed removal of objects with particle effects
/// </summary>
public sealed class DeathEffectController : MonoBehaviour {
    private List<DeathRegisteredObject> registeredObjects;

    void Start() {
        this.registeredObjects = new List<DeathRegisteredObject>();
    }

    void Update() {
        // check for and remove expired objects
        DeathRegisteredObject[] registeredObjectsToCheck = this.registeredObjects.ToArray();

        foreach (DeathRegisteredObject deathRegisteredObject in registeredObjectsToCheck) {
            if (deathRegisteredObject.TargetRemovalTime < Time.time) {
                this.registeredObjects.Remove(deathRegisteredObject);

                if (deathRegisteredObject.AnimationConfiguration != null && deathRegisteredObject.AnimationConfiguration.Value.FirstDisableParticleSystem) {
                    // disable emitting
                    RecursiveDisableEmitting(deathRegisteredObject.ObjectToDestroy);

                    // re-register
                    FadeOutAnimationConfiguration animConfig = deathRegisteredObject.AnimationConfiguration.Value;

                    DeathRegisteredObject newReg = deathRegisteredObject;
                    newReg.AnimationConfiguration = null;
                    newReg.TargetRemovalTime = Time.time + animConfig.ParticleFadeTime;

                    this.registeredObjects.Add(newReg);
                    continue;
                }
                
                Object.Destroy(deathRegisteredObject.ObjectToDestroy);
            }
        }
    }

    private static void RecursiveDisableEmitting(GameObject objectToDestroy) {
        Stack<GameObject> gameObjects = new Stack<GameObject>();
        gameObjects.Push(objectToDestroy);

        while (gameObjects.Count > 0) {
            GameObject current = gameObjects.Pop();

            // disable emitter
            ParticleEmitter partEmitter = current.GetComponent<ParticleEmitter>();
            if (partEmitter != null) {
                partEmitter.emit = false;
            }

            // search for additional
            foreach (Transform childTransform in current.transform) {
                GameObject child = childTransform.gameObject;

                gameObjects.Push(child);
            }
        }
    }

    /// <summary>
    /// Registers the specified object to be removed at the specified delay
    /// </summary>
    /// <param name="objectToRegister"></param>
    /// <param name="removalDelay"></param>
    public void Register(GameObject objectToRegister, float removalDelay) {
        Register(objectToRegister, removalDelay, null);
    }

    /// <summary>
    /// Registers the specified object to be removed at the specified delay
    /// </summary>
    /// <param name="objectToRegister"></param>
    /// <param name="removalDelay"></param>
    /// <param name="animationConfiguration"></param>
    public void Register(GameObject objectToRegister, float removalDelay, FadeOutAnimationConfiguration? animationConfiguration) {
        if (objectToRegister == null) {
            throw new ArgumentNullException("objectToRegister");
        }

        objectToRegister.name = "__DeathParticle";

        // register
        float removalTime = Time.time + removalDelay;

        DeathRegisteredObject drp = new DeathRegisteredObject();
        drp.ObjectToDestroy = objectToRegister;
        drp.AnimationConfiguration = animationConfiguration;
        drp.TargetRemovalTime = removalTime;

        this.registeredObjects.Add(drp);
    }

    /// <summary>
    /// Gets the instance. Requires an object with tag <see cref="Tags.World"/> to be present.
    /// </summary>
    public static DeathEffectController Instance {
        get {
            GameObject worldObject = GameObject.FindGameObjectWithTag(Tags.World);

            if (worldObject == null) {
                throw new UnityException("No World found or object with tag 'World'");
            }

            DeathEffectController controller = worldObject.GetComponent<DeathEffectController>();

            if (controller == null) {
                throw new UnityException("World object does not contain 'LevelBehaviour'");
            }

            return controller;
        }
    }

    #region Nested type: DeathRegisteredObject

    private struct DeathRegisteredObject {
        public FadeOutAnimationConfiguration? AnimationConfiguration;
        public GameObject ObjectToDestroy;
        public float TargetRemovalTime;
    }

    #endregion

    #region Nested type: FadeOutAnimationConfiguration

    public struct FadeOutAnimationConfiguration {
        public bool FirstDisableParticleSystem;
        public float ParticleFadeTime;


        public static implicit operator FadeOutAnimationConfiguration(DeathEffects.DeathEffectConfiguration deathEffectControl) {
            FadeOutAnimationConfiguration c = new FadeOutAnimationConfiguration();
            c.FirstDisableParticleSystem = deathEffectControl.AnimateParticlesSmoothlyOut;
            c.ParticleFadeTime = deathEffectControl.AnimateOutTime;

            return c;
        }
    }

    #endregion
}