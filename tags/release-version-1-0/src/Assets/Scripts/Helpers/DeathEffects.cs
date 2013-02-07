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
            //pos += myBody.velocity;
        }

        return pos;
    }


    private static GameObject LoadDeathEffect(DeathEffectConfiguration deathEffect) {
        if (CheatVariables.LoadDeathEffectsOnDemand) {
            return (GameObject)Object.Instantiate(deathEffect.EffectTemplate);
        }

        deathEffect.EffectTemplate.transform.parent = null;
        return deathEffect.EffectTemplate;
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
            GameObject splashObject = LoadDeathEffect(deathEffect);
            splashObject.transform.localScale = context.transform.localScale;

            // set positional information
            Quaternion targetRotation = causeOfDeath.transform.rotation;
            splashObject.transform.rotation = targetRotation;

            // ... add information of velocity, makes sure the effect is placed on the proper position
            Vector3 targetPosition = CalculatePositionWithBodyVelocity(context);
            targetPosition.y = causeOfDeath.transform.position.y;

            splashObject.transform.position = targetPosition;
            splashObject.transform.Translate(Vector3.up * 0.1f, Space.Self);

            DeathEffectController.Instance.Register(splashObject, deathEffect);
        }

        public static void ExecuteExtra(GameObject context, GameObject causeOfDeath, DeathEffectConfiguration deathEffect, GameObject extraTemplate) {
            // checks if the death effect is actually enabled
            if (deathEffect == null || deathEffect.EffectTemplate == null) {
                return;
            }

            // instantiate the template
            GameObject splashObject = (GameObject) Object.Instantiate(extraTemplate);
            splashObject.transform.localScale = context.transform.localScale;

            // set positional information
            Quaternion targetRotation = causeOfDeath.transform.rotation;
            splashObject.transform.rotation = targetRotation;

            // ... add information of velocity, makes sure the effect is placed on the proper position
            Vector3 targetPosition = CalculatePositionWithBodyVelocity(context);
            targetPosition.y = causeOfDeath.transform.position.y- 2f;

            splashObject.transform.position = targetPosition;
            splashObject.transform.Translate(Vector3.up * 0.1f, Space.Self);

            DeathEffectController.Instance.Register(splashObject, deathEffect);
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
            GameObject splashObject = LoadDeathEffect(deathEffect);
            splashObject.transform.localScale = context.transform.localScale;

            // set positional information
            Quaternion targetRotation = context.transform.rotation;
            splashObject.transform.rotation = targetRotation;

            // ... add information of velocity, makes sure the effect is placed on the proper position
            Vector3 targetPosition = context.transform.position;

            splashObject.transform.position = targetPosition;

            DeathEffectController.Instance.Register(splashObject, deathEffect);
        }
    }

    /// <summary>
    /// Logic for death by touching an enemy
    /// </summary>
    public static class EnemyTouchDeadEffect {
        public static void Execute(GameObject context, GameObject causeOfDeath, DeathEffectConfiguration deathEffect) {
            // checks if the death effect is actually enabled
            if (deathEffect == null || deathEffect.EffectTemplate == null) {
                return;
            }

            // instantiate the template
            GameObject splashObject = LoadDeathEffect(deathEffect);
            splashObject.transform.localScale = context.transform.localScale;

            // set positional information
            Quaternion targetRotation = context.transform.rotation;
            splashObject.transform.rotation = targetRotation;

            // ... add information of velocity, makes sure the effect is placed on the proper position
            Vector3 targetPosition = context.transform.position;

            if (!deathEffect.OffsetAsForce) {
                targetPosition += deathEffect.EffectOffset;
            }

            // ... disable wolf movement
            MonoBehaviour beh = causeOfDeath.GetComponent<MoveBehaviour>();
            if (beh != null) {
                beh.enabled = false;
            }
            MonoBehaviour beh2 = causeOfDeath.GetComponent<FoxMoveBehaviour>();
            if (beh2 != null) {
                beh2.enabled = false;
            }

            // ... disable sheep movement
            Rigidbody rb = context.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.isKinematic = true;
            }

            rb = splashObject.GetComponent<Rigidbody>();
            if (rb != null && deathEffect.OffsetAsForce) {
                rb.AddForce(deathEffect.EffectOffset, ForceMode.Impulse);
            }

            splashObject.transform.position = targetPosition;

            deathEffect.AnimateOutCallback = delegate {
                if (beh != null) {
                    beh.enabled = true;
                }
                if (beh2 != null) {
                    beh2.enabled = true;
                }
              
           };

            DeathEffectController.Instance.Register(splashObject, deathEffect);
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
        /// Defines the callback that is executed when the particles are animated out
        /// </summary>
        [NonSerialized]
        public Action AnimateOutCallback;

        /// <summary>
        /// Defines the callback that is executed when the object is destroyed
        /// </summary>
        [NonSerialized]
        public Action DeleteCallback;

        /// <summary>
        /// Defines the object to spawn
        /// </summary>
        public GameObject EffectTemplate;

        /// <summary>
        /// Gets the offset of the main effect template
        /// </summary>
        public Vector3 EffectOffset;

        /// <summary>
        /// If <c>true</c>, <see cref="EffectOffset"/> is applied as a force to the rigid body
        /// </summary>
        public bool OffsetAsForce = false;

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