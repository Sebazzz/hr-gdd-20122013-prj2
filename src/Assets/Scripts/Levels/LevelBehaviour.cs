using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms.GameCenter;

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
    /// Specifies the time used for blurring after level end
    /// </summary>
    public float BlurTime = 1;

    /// <summary>
    /// Specifies a time for the level. 
    /// </summary>
    public TimeDef LevelTime = new TimeDef(15,0);

    /// <summary>
    /// Specifies the global sound volume for this level. Is a value between 0 and 1.
    /// </summary>
    public float GlobalSoundVolume = 1f;

    void Awake() {
        this.OnLevelWasLoaded(Application.loadedLevel);

        DialogController.HideDialogs();
    }


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
        HUD.Instance.SetGoal(this.sheepCounter.MinimumSafeCount);
        HUD.Instance.SetNumberCollected(this.sheepCounter.CurrentSafeCount);
        HUD.Instance.SetMaxCollected(this.sheepCounter.CurrentCount);
	    HUD.Instance.LevelTime = (float) this.LevelTime;

        Debug.Log(String.Format("Initialized World. Number of dogs: {0}, number of sheep: {1}, minimum number to collect: {2}", this.dogCounter.StartCount,        this.sheepCounter.CurrentCount, this.sheepCounter.MinimumSafeCount));
	}

    internal void RecountSheep() {
        int sheepCount = GameObject.FindGameObjectsWithTag(Tags.Sheep).Length;
        this.sheepCounter.SetRecountValue(sheepCount);
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

        HUD.Instance.SetNumberCollected(this.sheepCounter.CurrentSafeCount);
    }

    /// <summary>
    /// Call this when a dog/shepard has been killed
    /// </summary>
    public void OnDogKilled() {
        this.dogCounter.IncreaseDeadCount();

        if (this.dogCounter.CurrentCount == 0) {
            // since no dogs can be used anymore, the level is over
            this.StartCoroutine(this.OnGameOver());
        }
    }

    /// <summary>
    /// Call this when a dog has entered the barn
    /// </summary>
    public void OnDogBarnEntered() {
        // sanity check
        if (this.sheepCounter.CurrentSafeCount < this.sheepCounter.MinimumSafeCount) {
            Debug.LogError("Still sheep left.. programming error?");
        }

        this.StartCoroutine(this.OnLevelCompleted());
    }

    /// <summary>
    /// Call this when a sheep has died
    /// </summary>
    public void OnSheepDeath() {
        this.sheepCounter.IncreaseDeadCount();
        HUD.Instance.SetMaxCollected(this.sheepCounter.CurrentCount);

        // check if all other sheep are dead
        if (this.sheepCounter.CurrentSafeCount + this.sheepCounter.CurrentCount < this.sheepCounter.MinimumSafeCount) {
            // since there are sheep left to collect and no sheep are alive, we're game over
            this.StartCoroutine(this.OnGameOver());
        }
    }

    private IEnumerator OnLevelCompleted() {
        // Save level state
        Level current = Levels.GetLevelByName(Application.loadedLevelName);
        current.SetFinished();

        bool minsheep = false;
        bool maxsheep = false;
        bool maxsheeptime = false;

        // MinSheep?
        if (this.sheepCounter.CurrentSafeCount >= NumberOfSheepToCollect) {
            current.AddScoreFlag(Level.Score.MinSheep);
            minsheep = true;
        }

        // MaxSheep?
        if (this.sheepCounter.CurrentSafeCount >= this.sheepCounter.StartCount) {
            current.AddScoreFlag(Level.Score.MaxSheep);
            maxsheep = true;

            // Within time?
            if (HUD.Instance.LevelTime > 0) {
                current.AddScoreFlag(Level.Score.MaxSheepWithinTime);
                maxsheeptime = true;
            }
        }

        // Show end dialog
        BlurEffect blurEffect = Camera.mainCamera.GetComponent<BlurEffect>();
        if (blurEffect != null) {
            blurEffect.enabled = true;
        }

        HUD.Instance.EnableCountDown = false;
        HUD.Instance.Show = false;
        HUD.Instance.DisplayScoreDialog(minsheep, maxsheep, maxsheeptime);

        // Unlock new level
        Levels.GetNextLevel(current).Unlock();

        this.audioController.GameWonSound.Play();

        yield return null;
    }

    private IEnumerator OnGameOver() {
        this.audioController.GameLostSound.Play();

        // Show dialog
        BlurEffect blurEffect = Camera.mainCamera.GetComponent<BlurEffect>();
        if (blurEffect != null) {
            blurEffect.enabled = true;
        }

        HUD.Instance.Show = false;
        HUD.Instance.EnableCountDown = false;

        if (this.dogCounter.DeadCount >= this.dogCounter.StartCount) {
            HUD.Instance.DisplayDeathDialog("You failed! All your dogs died!");            
        } else {
            HUD.Instance.DisplayDeathDialog("You failed! You didn't save enough sheep!");
        }

        yield return null;
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

        public void SetRecountValue(int value) {
            this.currentCount = value;
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

    [Serializable]
    public class TimeDef {
        public int Minutes;
        public int Seconds;

        public static explicit operator float(TimeDef t) {
            return t.Minutes*60f + t.Seconds;
        }

        public TimeDef() {}

        public TimeDef(int minutes, int seconds) {
            this.Minutes = minutes;
            this.Seconds = seconds;
        }
    }
}
