using System.Linq;
using UnityEngine;
using System.Collections;

public class EmitterBehaviour : MonoBehaviour {
    private GameObject groupObject;
    private Timer spawnTimer;
    private Timer waveTimer;
    private Camera spawnCameraCheck;
    private int currentSpawnCount;
    
    public int WaveTimeGap = 35;
    public float SpawnTimeGap = 1;
    public int SpawnCountTarget = 10;
    public bool DisableSpawnInCameraView = true;
    public GameObject ObjectToSpawn;


	// Use this for initialization
	void Start () {
        this.waveTimer = new Timer(WaveTimeGap);

	    string groupName = ObjectToSpawn.name + Globals.GroupObjectNameSuffix;
	    this.groupObject = GameObject.Find("/" + groupName);
        if (this.groupObject == null) {
            this.groupObject = new GameObject(groupName);
        }

	    this.spawnCameraCheck = Camera.mainCamera;
	}
	
	// Update is called once per frame
	void Update () {
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
        Gizmos.DrawIcon(transform.position, "Emitter-icon.png", true);
        Gizmos.DrawWireCube(this.transform.position, new Vector3(5, 5));
    }
}
