using System.Linq;
using UnityEngine;
using System.Collections;

public class EmitterBehaviour : MonoBehaviour {
    private Timer spawnTimer;
    private Timer waveTimer;
    private int currentSpawnCount;

    public int WaveTimeGap = 35;
    public float SpawnTimeGap = 1;
    public int SpawnCountTarget = 10;
    public bool DisableSpawnInCameraView = true;
    public GameObject ObjectToSpawn;

    public Camera SpawnCameraCheck;

	// Use this for initialization
	void Start () {
        waveTimer = new Timer(WaveTimeGap);
	}
	
	// Update is called once per frame
	void Update () {
        // check if seen by camera
        if (SpawnCameraCheck != null && DisableSpawnInCameraView && this.IsSeenInCamera()) {
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
        Vector3 res = SpawnCameraCheck.WorldToViewportPoint(this.transform.position);

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

    public Color markerColor = new Color(255,255,255);
    private void SpawnObject() {
        GameObject newInstance = (GameObject) Instantiate(ObjectToSpawn, this.transform.position, this.transform.rotation);

        // TODO: set any properties on new instance

        newInstance.GetComponentsInChildren<MeshRenderer>().First().material.color = markerColor;
    }

    void OnDrawGizmos() {
        Gizmos.DrawIcon(transform.position, "Emitter-icon.png", true);
    }
}
