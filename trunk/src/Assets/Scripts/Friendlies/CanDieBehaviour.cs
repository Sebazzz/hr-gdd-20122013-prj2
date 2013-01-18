using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for objects that can die. Derived classes implement specific death logic.
/// </summary>
/// <dependend cref="KillBehaviour" />
public abstract class CanDieBehaviour : MonoBehaviour {
    private GameObject currentCauseOfDeath;

    /// <summary>
    /// Defines the delay between being hit and being killed
    /// </summary>
    public float KillDelay = 0;

    /// <summary>
    /// Defines whether or not to disable any (by default movement) scripts when dying
    /// </summary>
    public bool DisableScriptsWhenDying = true;
    
    /// <summary>
    /// Causes the object to which this script is attached to die. Custom logic is implemented in derived classes. 
    /// Don't call this directly, use <see cref="ExecuteDirectDeath"/>
    /// </summary>
    /// <param name="causeOfDeath"></param>
    protected virtual void OnExecuteDeath(GameObject causeOfDeath) {}

    /// <summary>
    /// Returns a value indicating if the object can die with the game object specified
    /// </summary>
    /// <param name="causeOfDeath"></param>
    /// <param name="causeOfDeathTag"></param>
    /// <param name="causeOfDeathLayer"></param>
    /// <returns></returns>
    protected virtual bool CanDie (GameObject causeOfDeath, string causeOfDeathTag, int causeOfDeathLayer) {
        return true;
    }

    /// <summary>
    /// Allows derived scripts to call this for death to execute directly. This is usually called in <see cref="OnStartDying"/>. 
    /// </summary>
    protected void ExecuteDirectDeath() {
        // for the audio of death to be able to play, we need to keep the object alive
        // so we just disable colliders, rigidbody and renderer. it exists, but it doesnt influence the game world anymore
        RecursiveDisableInfluence(this.gameObject);
    }


    private static void RecursiveDisableInfluence(GameObject rootObject) {
        Stack<GameObject> gameObjects = new Stack<GameObject>();
        gameObjects.Push(rootObject);

        while (gameObjects.Count > 0) {
            GameObject current = gameObjects.Pop();

            Rigidbody rb = current.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.isKinematic = true;
            }

            Collider c = current.GetComponent<Collider>();
            if (c != null) {
                c.enabled = false;
            }

            Renderer r = current.GetComponent<Renderer>();
            if (r != null) {
                r.enabled = false;
            }

            // search for additional
            foreach (Transform childTransform in current.transform) {
                GameObject child = childTransform.gameObject;

                gameObjects.Push(child);
            }
        }
    }

    /// <summary>
    /// Notifies derived classes that death is imminent. Is called before death is invoked, and at the start of the <see cref="KillDelay"/>
    /// </summary>
    /// <param name="causeOfDeath"></param>
    protected virtual void OnStartDying (GameObject causeOfDeath) {
        if (this.DisableScriptsWhenDying) {
            this.DisableScriptIfExists<MoveBehaviour>();
        }
    }

    protected void DisableScriptIfExists<T>() where T : MonoBehaviour {
        T scriptObject = this.gameObject.GetComponent<T>();

        if (scriptObject != null) {
            scriptObject.enabled = false;
        }
    }

    /// <summary>
    /// Causes the object to which this script is attached to die or starting to die.
    /// </summary>
    /// <param name="causeOfDeath"></param>
    public void Die (GameObject causeOfDeath) {
        if (!this.CanDie(causeOfDeath, causeOfDeath.tag, causeOfDeath.layer)) {
            return;
        }

        if (this.currentCauseOfDeath != null) {
            Debug.LogWarning("Dying a second time while already dying.");            
        }

        this.currentCauseOfDeath = causeOfDeath;
        this.OnStartDying(causeOfDeath);
        this.currentCauseOfDeath = null;

        if (this.KillDelay < -100) {
            // death already executed
            return;
        }

        // check for instant kill
        if (Mathf.Abs(this.KillDelay) < 0.01) {
            this.OnExecuteDeath(causeOfDeath);
            return;
        }

        // delayed kill
        this.currentCauseOfDeath = causeOfDeath;
    }

    void Update() {
        if (this.currentCauseOfDeath == null || KillDelay < -1000) {
            return;
        }

        KillDelay -= Time.deltaTime;

        if (KillDelay < 0) {
            KillDelay = -1000;
            Debug.Log("KILLL1" + this.currentCauseOfDeath.name);
            this.OnExecuteDeath(this.currentCauseOfDeath);
            Debug.Log("KILLL3" + this.currentCauseOfDeath.name);
            this.currentCauseOfDeath = null;
			
        }
    }

}
