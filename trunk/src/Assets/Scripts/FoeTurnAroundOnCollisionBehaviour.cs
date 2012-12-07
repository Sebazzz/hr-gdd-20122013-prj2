using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scripts with one responsibility: turns around the foe if it hits an collision
/// </summary>
public class FoeTurnAroundOnCollisionBehaviour : MonoBehaviour {
    /// <summary>
    /// Specifies the tags to ignore for this behaviour
    /// </summary>
    private readonly IEnumerable<string> ignoreTags = new [] {
            
            Tags.Enemy,
            Tags.Foe,
            Tags.Sheep,
            Tags.Shepherd,
        };

    private readonly IEnumerable<int> ignoreLayers = new [] {
            Layers.Terrain
    }; 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision collisionInfo) {
        string collidingObjectTag = collisionInfo.gameObject.tag;
        int collidingObjectLayer = collisionInfo.gameObject.layer;

        if (!this.ShouldIgnoreObject(collidingObjectTag, collidingObjectLayer)) {
            this.TurnAround();
        }
    }

    private bool ShouldIgnoreObject(string collidingObjectTag, int collidingObjectLayer) {
        foreach (string ignoreTag in ignoreTags) {
            if (collidingObjectTag == ignoreTag) {
                return true;
            }
        }

        foreach (int ignoreLayer in ignoreLayers) {
            if (ignoreLayer == collidingObjectLayer) {
                return true;
            }
        }

        return false;
    }

    private void TurnAround() {
        MoveBehaviour mv = this.gameObject.GetComponent<MoveBehaviour>();
        mv.Stop();

        FoxMoveBehaviour fmv = this.gameObject.GetComponent<FoxMoveBehaviour>();
        fmv.DeltaX *= -1;
        fmv.DeltaY *= -1;
        fmv.DeltaZ *= -1;
    }
}
