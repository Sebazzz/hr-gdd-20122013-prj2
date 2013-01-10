using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Helper class for level-global behaviour. Also handles cleaning up resources.
/// </summary>
/// <remarks>
/// This script should be executed as last.
/// </remarks>
public class LevelBehaviour : MonoBehaviour {
    /// <summary>
    /// Defines the number of sheep to collect in the level
    /// </summary>
    public int NumberOfSheepToCollect = 1;

    /// <summary>
    /// Defines the maximum height for drawing
    /// </summary>
    public float MaxDogHeight = 10f;

	// Use this for initialization
	void Start () {
	    // sanity check
	    GameObject[] sheep = GameObject.FindGameObjectsWithTag(Tags.Sheep);

        if (sheep.Length < NumberOfSheepToCollect) {
            throw new UnityException("There are more sheep to collect than sheep available");
        }
	}


    void OnLevelWasLoaded(int index) {
        // release any existing lock on the mouse
        MouseManager.ReleaseLock(this);
    }
	
    /// <summary>
    /// Call this when a sheep has been collected
    /// </summary>
	public void OnSheepCollected() {
        this.NumberOfSheepToCollect--;

        if (this.NumberOfSheepToCollect <= 0) {
            // end level
            this.OnLevelCompleted();
        } else {
            // check if all other sheep are dead
            GameObject[] sheep = GameObject.FindGameObjectsWithTag(Tags.Sheep);

            if (sheep.Length == 0) {
                // since there are sheep left to collect and no sheep are alive, we're game over
                this.OnGameOver();
            }
        }
    }

    private void OnLevelCompleted() {
        // TODO: show level end
        Application.LoadLevel(Scenes.MainMenu); // restart level
    }

    private void OnGameOver() {
        // TODO: show level failure end
        Application.LoadLevel(Application.loadedLevel); // restart level
    }

    /// <summary>
    /// Call this when a sheep has died
    /// </summary>
    public void OnSheepDeath() {
        if (this.NumberOfSheepToCollect <= 0) {
            this.OnLevelCompleted();
        } else {
            // check if all other sheep are dead
            GameObject[] sheep = GameObject.FindGameObjectsWithTag(Tags.Sheep);

            if (sheep.Length == 0 && this.NumberOfSheepToCollect > 0) {
                // since there are sheep left to collect and no sheep are alive, we're game over
                this.OnGameOver();
            }
        }
    }

    /// <summary>
    /// Gets the current level behaviour instance
    /// </summary>
    public static LevelBehaviour Instance {
        get {
            GameObject worldObject = GameObject.FindGameObjectWithTag(Tags.World);

            if (worldObject == null) {
                throw new UnityException("No World found or object with tag 'World'");
            }

            LevelBehaviour levelBehaviourScript = worldObject.GetComponent<LevelBehaviour>();

            if (levelBehaviourScript == null) {
                throw new UnityException("World object does not contain 'LevelBehaviour'");
            }

            return levelBehaviourScript;
        }
    }
}
