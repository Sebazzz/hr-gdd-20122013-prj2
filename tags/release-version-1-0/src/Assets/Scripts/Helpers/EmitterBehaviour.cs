using System.Linq;
using UnityEngine;
using System.Collections;

/// <summary>
/// Game logic for behaviour of an emitter. This script is responsible for:
/// - Timing waves 
/// - Timing of spawning within waves
/// - Drawing design-time gizmos
/// </summary>
public class EmitterBehaviour : MonoBehaviour {
    /// <summary>Grouping object for objects spawned by this emitter </summary>
    private GameObject groupObject;
    private Timer initialOffsetTimer;
    private Timer spawnTimer;
    private Timer waveTimer;
    private Camera spawnCameraCheck;
    private int currentSpawnCount;


    /// <summary>
    /// Specifies the initial time offset. This allows other emitters with the same settings not to emit at the same time.
    /// </summary>
    public float InitialTimeOffset = 0;

    /// <summary>
    /// Specifies the time gap between waves and also between the start of the game and the first wave
    /// </summary>
    public int WaveTimeGap = 35;

    /// <summary>
    /// Specifies the time gap between each spawn in the wave
    /// </summary>
    public float SpawnTimeGap = 1;

    /// <summary>
    /// Specifies the number of targets to spawn in the wave
    /// </summary>
    public int SpawnCountTarget = 10;

    /// <summary>
    /// Specifies whether to disable the emitter once the emitter is in camera view
    /// </summary>
    public bool DisableSpawnInCameraView = true;

    /// <summary>
    /// Specifies the game object to clone and spawn
    /// </summary>
    public GameObject ObjectToSpawn;


	// Use this for initialization
	void Start () {
        this.initialOffsetTimer = new Timer(this.InitialTimeOffset);
        this.waveTimer = new Timer(WaveTimeGap);

	    string groupName = Globals.GroupObjectPrefix + ObjectToSpawn.name + Globals.GroupObjectNameSuffix;
	    this.groupObject = GameObject.Find("/" + groupName);
        if (this.groupObject == null) {
            this.groupObject = new GameObject(groupName);
        }

	    this.spawnCameraCheck = Camera.mainCamera;
	}
	
	// Update is called once per frame
	void Update () {
        if (this.initialOffsetTimer != null) {
            this.initialOffsetTimer.Update();
            
            if (this.initialOffsetTimer.IsTriggered) {
                this.initialOffsetTimer = null;
            }

            return;
        }

        // check if seen by camera
        if (this.spawnCameraCheck != null && DisableSpawnInCameraView && this.IsSeenInCamera()) {
            if (this.spawnTimer != null) {
                this.FinishWave();
            }
			
			return;
        }

        waveTimer.Update();

        // check if wave spawn is required
        if (waveTimer.IsTriggered) {
            // initialize wave if required
            if (spawnTimer == null) {
                spawnTimer = new Timer(SpawnTimeGap);

                // Play sound
                this.GetComponent<FoeAudioController>().EmitSound.Play();
            }
            
            spawnTimer.Update();

            if (spawnTimer.IsTriggered) {
                spawnTimer.Reset();
                
                this.SpawnObject();
                currentSpawnCount++;

                if (currentSpawnCount >= SpawnCountTarget) {
                    this.FinishWave();
                }
            }
        }
	}

    /// <summary>
    /// Forces a wave to emit
    /// </summary>
    public void ForceWave() {
        this.waveTimer.Trigger();
    }


    // http://answers.unity3d.com/questions/8003/how-can-i-know-if-a-gameobject-is-seen-by-a-partic.html
    private bool IsSeenInCamera() {
        Vector3 res = this.spawnCameraCheck.WorldToViewportPoint(this.transform.position);

        return res.x > 0 && res.x < 1 &&
               res.y > 0 && res.y < 1 &&
               res.z > 0;
    }

    private void FinishWave() {
        // stop the wave
        this.spawnTimer = null;
        this.currentSpawnCount = 0;
        this.waveTimer.Reset();
    }

    private void SpawnObject() {
        GameObject newInstance = (GameObject) Instantiate(ObjectToSpawn, this.transform.position, this.transform.rotation);

        // set object as child of our grouping object
        newInstance.transform.parent = this.groupObject.transform;
    }

    void OnDrawGizmos() {
        const float cubeSizeWidth = 5;
        const float cubeSizeHeight = cubeSizeWidth / 2f;
        const float vectorLength = 2.5f;
        const float sphereSize = 0.5f;
        const float sphereSizeIncrease = 0.25f;
        const int numberOfSpheres = 3;

        // draw basic icon
        Gizmos.DrawIcon(transform.position, "Emitter-icon.png", true);

        // draw begin and end director cubes
        Gizmos.color = Color.red;
        Gizmos.DrawCube(this.transform.position + this.transform.TransformDirection(0, 0, 1), new Vector3(cubeSizeWidth, cubeSizeHeight));

        // draw spheres for direction
        Gizmos.color = Color.red;

        float previousSphereSize = sphereSize + sphereSizeIncrease;
        Vector3 position = this.transform.position - this.transform.TransformDirection(0, 0, previousSphereSize);
        position.y -= 1;
        for (int i = 0; i < numberOfSpheres; i++) {
            position += this.transform.TransformDirection(0, 0, vectorLength);

            previousSphereSize += sphereSizeIncrease;
            Gizmos.DrawSphere(position, previousSphereSize / 2f);

            if (numberOfSpheres == (i + 2)) {
                Gizmos.color = Color.green;
            } else {
                Gizmos.color = Color.white;
            }
        }
    }
}
