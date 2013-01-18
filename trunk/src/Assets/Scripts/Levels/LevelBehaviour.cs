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
    private SheepLevelCounter sheepCounter;
    private WorldAudioController audioController;
    private LevelCounter dogCounter;

    /// <summary>
    /// Gets if the level can be completed
    /// </summary>
    public bool CanLevelBeCompleted {
        get { return this.sheepCounter.CurrentSafeCount >= this.sheepCounter.MinimumSafeCount; }
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
        int dogCount = GameObject.FindGameObjectsWithTag(Tags.Shepherd).Length;
        this.dogCounter = new LevelCounter(dogCount);
	    
        int sheepCount = GameObject.FindGameObjectsWithTag(Tags.Sheep).Length;
        this.sheepCounter = new SheepLevelCounter(sheepCount, this.NumberOfSheepToCollect);

        // Set the HUD
        HUD.Instance.setGoal(this.sheepCounter.MinimumSafeCount);
        HUD.Instance.setCollected(this.sheepCounter.CurrentSafeCount);
        HUD.Instance.setMaxCollected(this.sheepCounter.CurrentCount);

        Debug.Log(String.Format("Initialized World. Number of dogs: {0}, number of sheep: {1}, minimum number to collect: {2}", this.dogCounter.StartCount,        this.sheepCounter.CurrentCount, this.sheepCounter.MinimumSafeCount));
	}


    void OnLevelWasLoaded(int index) {
        // release any existing lock on the mouse
        MouseManager.ReleaseLock(this);
    }
	
    /// <summary>
    /// Call this when a sheep has been collected
    /// </summary>
	public void OnSheepCollected() {
        this.sheepCounter.IncreaseSafeCount();

        HUD.Instance.setCollected(this.sheepCounter.CurrentSafeCount);
    }

    /// <summary>
    /// Call this when a dog/shepard has been killed
    /// </summary>
    public void OnDogKilled() {
        this.dogCounter.IncreaseDeadCount();

        if (this.dogCounter.CurrentCount == 0) {
            // since no dogs can be used anymore, the level is over
            this.OnGameOver();
        }
    }

    /// <summary>
    /// Call this when a dog has entered the barn
    /// </summary>
    public void OnDogBarnEntered() {
        // sanity check
        if (this.sheepCounter.CurrentSafeCount > this.sheepCounter.MinimumSafeCount) {
            throw new Exception("Still sheep left.. programming error?");
        }

        this.OnLevelCompleted();
    }

    /// <summary>
    /// Call this when a sheep has died
    /// </summary>
    public void OnSheepDeath() {
        this.sheepCounter.IncreaseDeadCount();
        HUD.Instance.setMaxCollected(this.sheepCounter.CurrentCount);

        // check if all other sheep are dead
        if (this.sheepCounter.CurrentSafeCount + this.sheepCounter.CurrentCount < this.sheepCounter.MinimumSafeCount) {
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

        this.audioController.GameWonSound.Play();
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


    private class LevelCounter {
        private readonly int startCount;
        private int currentCount;

        public int StartCount {
            get { return this.startCount; }
        }

        public int CurrentCount {
            get { return this.currentCount; }
        }

        public int DeadCount {
            get { return this.startCount - this.currentCount; }
        }

        public void IncreaseDeadCount() {
            this.currentCount--;
        }

        public LevelCounter(int startCount) {
            this.startCount = startCount;
            this.currentCount = startCount;
        }
    }

    private sealed class SheepLevelCounter : LevelCounter {
        private readonly int minimumSafeCount;
        private int currentSafeCount;

        public int MinimumSafeCount {
            get { return this.minimumSafeCount; }
        }

        public int CurrentSafeCount {
            get { return this.currentSafeCount; }
        }

        public int FreeSheepLeft {
            get { return this.CurrentSafeCount - this.CurrentCount; }
        }

        public void IncreaseSafeCount() {
            this.currentSafeCount++;
        }

        public SheepLevelCounter(int startCount, int minimumSafeCount) : base(startCount) {
            this.minimumSafeCount = minimumSafeCount;
            this.currentSafeCount = 0;
        }
    }
}
