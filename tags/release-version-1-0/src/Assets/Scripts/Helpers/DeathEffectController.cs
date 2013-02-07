using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Executes delayed removal of objects with particle and other effects
/// </summary>
public sealed class DeathEffectController : MonoBehaviour {
    private static IEnumerator ProcessDeathEffect(GameObject context, DeathEffects.DeathEffectConfiguration animationConfiguration) {
        // first wait for removing object or particles
        yield return new WaitForSeconds(animationConfiguration.InitialDelay);

        // check if we need to remove particles
        if (animationConfiguration.AnimateParticlesSmoothlyOut) {
            // disable emitting
            RecursiveDisableEmitting(context);

            if (animationConfiguration.AnimateOutCallback != null) {
                animationConfiguration.AnimateOutCallback.Invoke();
            }

            // let particles animate out
            yield return new WaitForSeconds(animationConfiguration.AnimateOutTime);
        }

        // execute deletion callback
        if (animationConfiguration.DeleteCallback != null) {
            animationConfiguration.DeleteCallback.Invoke();
        }

        // check for fade out or execute direct destroy
        FadeOutOfSceneAbility fadeOutScript =
            context.GetComponent<FadeOutOfSceneAbility>();
        if (fadeOutScript != null) {
            fadeOutScript.Enable(context);
        } else {
            Object.Destroy(context);
        }

        yield break;
    }

    /// <summary>
    /// Recursively walks the object graph to diable any <see cref="ParticleEmitter"/>s.
    /// </summary>
    /// <param name="objectToDestroy"></param>
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
    /// <param name="animationConfiguration"></param>
    public void Register(GameObject objectToRegister,DeathEffects.DeathEffectConfiguration animationConfiguration) {
        if (objectToRegister == null) {
            throw new ArgumentNullException("objectToRegister");
        }

        objectToRegister.name = "__DeathParticle";

        if (!CheatVariables.LoadDeathEffectsOnDemand) {
            objectToRegister.transform.parent = null;
            objectToRegister.SetActive(true);
        }

        this.StartCoroutine(ProcessDeathEffect(objectToRegister, animationConfiguration));
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
}