using System;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Helper class for containing custom logic for all death effects
/// </summary>
public static class DeathEffects {
    private static Vector3 CalculatePositionWithBodyVelocity(GameObject context) {
        Vector3 pos = context.transform.position;

        Rigidbody myBody = context.GetComponent<Rigidbody>();
        if (myBody != null) {
            pos += myBody.velocity;
        }

        return pos;
    }

    /// <summary>
    /// Logic for death by water touch
    /// </summary>
    public static class WaterDeathEffect {
        public static void Execute(GameObject context, GameObject causeOfDeath, DeathEffectConfiguration deathEffect) {
            // checks if the death effect is actually enabled
            if (deathEffect == null || deathEffect.EffectTemplate == null) {
                return;
            }

            // instantiate the template
            GameObject splashObject = (GameObject)Object.Instantiate(deathEffect.EffectTemplate);

            // set positional information
            Quaternion targetRotation = causeOfDeath.transform.rotation;
            splashObject.transform.rotation = targetRotation;


            // ... add information of velocity, makes sure the effect is placed on the proper position
            Vector3 targetPosition = CalculatePositionWithBodyVelocity(context);
            targetPosition.y = causeOfDeath.transform.position.y;

            splashObject.transform.position = targetPosition;
            splashObject.transform.Translate(Vector3.up * 0.1f, Space.Self);

            DeathEffectController.FadeOutAnimationConfiguration? animationConfig = deathEffect;
            DeathEffectController.Instance.Register(splashObject, deathEffect.InitialDelay, animationConfig);
        }

        public static void ExecuteExtra(GameObject context, GameObject causeOfDeath, DeathEffectConfiguration deathEffect, GameObject extraTemplate) {
            // checks if the death effect is actually enabled
            if (deathEffect == null || deathEffect.EffectTemplate == null) {
                return;
            }

            // instantiate the template
            GameObject splashObject = (GameObject)Object.Instantiate(extraTemplate);

            // set positional information
            Quaternion targetRotation = causeOfDeath.transform.rotation;
            splashObject.transform.rotation = targetRotation;

            // ... add information of velocity, makes sure the effect is placed on the proper position
            Vector3 targetPosition = CalculatePositionWithBodyVelocity(context);
            targetPosition.y = causeOfDeath.transform.position.y- 2f;

            splashObject.transform.position = targetPosition;
            splashObject.transform.Translate(Vector3.up * 0.1f, Space.Self);

            DeathEffectController.FadeOutAnimationConfiguration? animationConfig = deathEffect;
            DeathEffectController.Instance.Register(splashObject, deathEffect.InitialDelay, animationConfig);
        }
    }

    /// <summary>
    /// Logic for death by fence touch by placing a ragdoll
    /// </summary>
    public static class RagdollTouchDeathEffect {
        public static void Execute(GameObject context, GameObject causeOfDeath, DeathEffectConfiguration deathEffect) {
            // checks if the death effect is actually enabled
            if (deathEffect == null || deathEffect.EffectTemplate == null) {
                return;
            }

            // instantiate the template
            GameObject splashObject = (GameObject)Object.Instantiate(deathEffect.EffectTemplate);

            // set positional information
            Quaternion targetRotation = context.transform.rotation;
            splashObject.transform.rotation = targetRotation;

            // ... add information of velocity, makes sure the effect is placed on the proper position
            Vector3 targetPosition = context.transform.position;

            splashObject.transform.position = targetPosition;

            DeathEffectController.FadeOutAnimationConfiguration? animationConfig = deathEffect;
            DeathEffectController.Instance.Register(splashObject, deathEffect.InitialDelay, animationConfig);
        }
    }

    /// <summary>
    /// Helper class for configuration of death effects
    /// </summary>
    [Serializable]
    public class DeathEffectConfiguration {
        /// <summary>
        /// Specifies if particles should be smoothly animated out before actually destroying the game object
        /// </summary>
        public bool AnimateParticlesSmoothlyOut;
        /// <summary>
        /// Specifies the time frame between stopping particle creation and actual destruction of the game object. 
        /// Has only effect when <see cref="AnimateParticlesSmoothlyOut"/> is <c>true</c>.
        /// </summary>
        public float AnimateOutTime;

        /// <summary>
        /// Specifies the time before the object is destroyed and removed from the scene if <see cref="AnimateParticlesSmoothlyOut"/> is <c>false</c>,
        /// otherwise specifies the time before particle emission is stopped and countdown of <see cref="AnimateOutTime"/> is started.
        /// </summary>
        public float InitialDelay;

        /// <summary>
        /// Defines the object to spawn
        /// </summary>
        public GameObject EffectTemplate;

        public DeathEffectConfiguration() {}

        public DeathEffectConfiguration(float initialDelay, bool animateParticlesSmoothlyOut, float animateOutTime) {
            this.InitialDelay = initialDelay;
            this.AnimateParticlesSmoothlyOut = animateParticlesSmoothlyOut;
            this.AnimateOutTime = animateOutTime;
        }

        public DeathEffectConfiguration(float initialDelay) {
            this.InitialDelay = initialDelay;
            this.AnimateParticlesSmoothlyOut = false;
            this.AnimateOutTime = Single.NaN;
        }
    }
}