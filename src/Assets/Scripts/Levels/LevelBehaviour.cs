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
    private int minimumNumberOfSheepToCollect;
    private int numberOfDogs;
    private int numberOfSheep;

    /// <summary>
    /// Gets the number of sheep originally present in the scene
    /// </summary>
    public int NumberOfSheep {
        get { return this.numberOfSheep; }
    }
    
    /// <summary>
    /// Gets the number of sheep left
    /// </summary>
    public int MinimumNumberOfSheepLeft {
        get { return this.minimumNumberOfSheepToCollect; }
    }

    /// <summary>
    /// Defines the minimum number of sheep to collect in the level
    /// </summary>
    public int NumberOfSheepToCollect = 1;


	// Use this for initialization
	void Start () {
	    this.minimumNumberOfSheepToCollect = this.NumberOfSheepToCollect;

	    // sanity check
	    GameObject[] sheep = GameObject.FindGameObjectsWithTag(Tags.Sheep);

        if (sheep.Length < NumberOfSheepToCollect) {
            throw new UnityException("There are more sheep to collect than sheep available");
        }

        // count our objects
	    this.numberOfDogs = GameObject.FindGameObjectsWithTag(Tags.Shepherd).Length;
	    this.numberOfSheep = GameObject.FindGameObjectsWithTag(Tags.Sheep).Length;

        Debug.Log(String.Format("Initialized World. Number of dogs: {0}, number of sheep: {1}, minimum number to collect: {2}", this.numberOfDogs, this.numberOfSheep, this.minimumNumberOfSheepToCollect));
	}


    void OnLevelWasLoaded(int index) {
        // release any existing lock on the mouse
        MouseManager.ReleaseLock(this);
    }
	
    /// <summary>
    /// Call this when a sheep has been collected
    /// </summary>
	public void OnSheepCollected() {
        this.minimumNumberOfSheepToCollect--;

        if (this.minimumNumberOfSheepToCollect <= (this.NumberOfSheepToCollect - this.numberOfSheep)) {
            // end level
            this.OnLevelCompleted();
        } else {
            // check if all other sheep are dead
            GameObject[] sheep = GameObject.FindGameObjectsWithTag(Tags.Sheep);

            // count the number of active sheep
            int count = 0;
            foreach (GameObject o in sheep) {
                if (!o.rigidbody.isKinematic) {
                    count++;
                }
            }

            if (count <= 0) {
                // since there are sheep left to collect and no sheep are alive, we're game over
                this.OnGameOver();
            }
        }
    }

    /// <summary>
    /// Call this when a dog/shepard has been killed
    /// </summary>
    public void OnDogKilled() {
        this.numberOfDogs--;

        if (this.numberOfDogs <= 0) {
            // since no dogs can be used anymore, the level is over
            this.OnGameOver();
        }
    }

    /// <summary>
    /// Call this when a dog has entered the barn
    /// </summary>
    public void OnDogBarnEntered() {
        // sanity check
        if (this.minimumNumberOfSheepToCollect > 0) {
            throw new Exception("Still sheep left.. programming error?");
        }

        this.OnLevelCompleted();
    }

    /// <summary>
    /// Call this when a sheep has died
    /// </summary>
    public void OnSheepDeath() {
        if (this.minimumNumberOfSheepToCollect <= (this.NumberOfSheepToCollect - this.numberOfSheep)) {
            this.OnLevelCompleted();
        } else {
            // check if all other sheep are dead
            GameObject[] sheep = GameObject.FindGameObjectsWithTag(Tags.Sheep);

            if (sheep.Length - 1 == 0 && this.minimumNumberOfSheepToCollect > 0) {
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
