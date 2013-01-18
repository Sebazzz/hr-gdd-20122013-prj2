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
    private WorldAudioController audioController;

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
    /// Number of collected sheep
    /// </summary>
    private int numberOfSheepCollected = 0;
    public int NumberOfSheepCollected {
        get { return numberOfSheepCollected; }
    }

    /// <summary>
    /// Number of sheep that died
    /// </summary>
    private int numberOfDeadSheep = 0;
    public int NumberOfDeadSheep {
        get { return numberOfDeadSheep; }
    }


    /// <summary>
    /// Specifies the minimum number of sheep to collect in the level
    /// </summary>
    public int NumberOfSheepToCollect = 1;

    /// <summary>
    /// Specifies the global sound volume for this level. Is a value between 0 and 1.
    /// </summary>
    public float GlobalSoundVolume = 1f;


	// Use this for initialization
	void Start () {
        this.audioController = this.GetComponent<WorldAudioController>();

	    this.minimumNumberOfSheepToCollect = this.NumberOfSheepToCollect;

	    // sanity check
	    GameObject[] sheep = GameObject.FindGameObjectsWithTag(Tags.Sheep);

        if (sheep.Length < NumberOfSheepToCollect) {
            throw new UnityException("There are more sheep to collect than sheep available");
        }

        if (this.GlobalSoundVolume < 0 || this.GlobalSoundVolume > 1) {
            throw new UnityException("Global Sound Volume should be a number between 0 and 1");
        }

        // make sure the music has the correct priority
	    AudioSource music = Camera.mainCamera.GetComponent<AudioSource>();
        if (music == null || music.clip == null) {
            Debug.LogError("The level doesn't have any music. Please attach a working audio source with some music to the main camera. Is this correct?", Camera.mainCamera);
        } else {
            if (!music.loop || !music.playOnAwake) {
                Debug.LogError("The level music is not configured correctly. Please check 'Loop' and 'Play On Awake'.");
            }

            music.priority = 0;
        }

        // count our objects
	    this.numberOfDogs = GameObject.FindGameObjectsWithTag(Tags.Shepherd).Length;
	    this.numberOfSheep = GameObject.FindGameObjectsWithTag(Tags.Sheep).Length;

        // Set the HUD
        HUD.Instance.setGoal(LevelBehaviour.Instance.NumberOfSheepToCollect);
        HUD.Instance.setCollected(LevelBehaviour.Instance.NumberOfSheepCollected);
        HUD.Instance.setMaxCollected(LevelBehaviour.Instance.NumberOfSheep);

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
        this.numberOfSheepCollected++;

        //Update hud
        HUD.Instance.setCollected(LevelBehaviour.Instance.NumberOfSheepCollected);

        if (this.minimumNumberOfSheepToCollect > (this.NumberOfSheepToCollect - this.numberOfSheep)) {
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
        this.numberOfDeadSheep++;
        // check if all other sheep are dead
        GameObject[] sheep = GameObject.FindGameObjectsWithTag(Tags.Sheep);
        Debug.Log("Sheep killed, left: " + sheep.Length);
        HUD.Instance.setMaxCollected(this.NumberOfSheep - this.NumberOfDeadSheep);

        if (sheep.Length - 1 == 0 && this.minimumNumberOfSheepToCollect > 0) {
            // since there are sheep left to collect and no sheep are alive, we're game over
            this.OnGameOver();
        }
    }

    private void OnLevelCompleted() {
        // TODO: show level end
        // Save level state
        Level current = Levels.GetLevelByName(Application.loadedLevelName);
        current.SetFinished();
        // Unlock new level
        Levels.GetNextLevel(current).Unlock();

        //this.audioController.GameWonSound.Play();
        Application.LoadLevel(Scenes.MainMenu); // Go to main menu
    }

    private void OnGameOver() {
        // TODO: show level failure end
        this.audioController.GameLostSound.Play();
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
