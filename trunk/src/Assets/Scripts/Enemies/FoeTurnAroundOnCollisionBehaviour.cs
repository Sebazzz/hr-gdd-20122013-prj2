using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scripts with one responsibility: turns around the foe if it hits an collision
/// </summary>
public class FoeTurnAroundOnCollisionBehaviour : MonoBehaviour {
    /// <summary>
    /// Specifies how long to ignore collision events after colliding with something. This prevents being stuck or still going through an object
    /// </summary>
    public float IgnoreTime = 1.500f;

    /// <summary>
    /// Specifies how many times to step 'back' when colliding
    /// </summary>
    public int NumberOfStepBacks = 4;

    /// <summary>
    /// Specifies the time the thing take to turn around after collision
    /// </summary>
    public float TurnAroundTime = 1;

    private float lastTurnAroundTime;

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
        if ((Time.time - this.lastTurnAroundTime) < this.IgnoreTime) {
            return;
        }
        this.lastTurnAroundTime = Time.time;

        MoveBehaviour mv = this.gameObject.GetComponent<MoveBehaviour>();
        mv.Stop();

        // set the new rotation
        Quaternion rotation = this.gameObject.transform.rotation;
        rotation = Quaternion.Euler(rotation.eulerAngles.x, (rotation.eulerAngles.y + 180) % 360, rotation.eulerAngles.z);
        mv.MoveToDirection(rotation);

        this.transform.Translate(2, 0, 0);

        for (int i = 0; i < this.NumberOfStepBacks; i++) {
            // execute a single step to prevent being stuck
            mv.MoveSingleStep();
        }
    }
}