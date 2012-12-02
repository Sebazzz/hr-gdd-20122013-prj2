using System;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Removes colliding objects with the configured tag. If no tag has been configured, any object colliding is destroyed.
/// </summary>
public class RemoveCollidersBehaviour : MonoBehaviour {

    /// <summary>
    /// Specifies the tag to match on colliding objects. 
    /// </summary>
    public string TagToMatch = null;

    void OnTriggerEnter (Collider collidingObject) {
        this.KillObjectIfRequired(collidingObject.gameObject);
    }

    private void KillObjectIfRequired (GameObject gameObjectToCheck) {
        // determine if we should remove the object from the scene
        bool removeFromScene;

        if (!String.IsNullOrEmpty(this.TagToMatch)) {
            string collidingObjectTag = gameObjectToCheck.tag;

            removeFromScene = collidingObjectTag == this.TagToMatch;
        } else {
            removeFromScene = true;
        }

        // kill if required
        if (removeFromScene) {
            Object.Destroy(gameObjectToCheck);
        }
    }
}
