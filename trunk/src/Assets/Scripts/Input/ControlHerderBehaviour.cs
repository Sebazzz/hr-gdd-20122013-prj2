using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Script with the purpose of enabling a player to control a dog. Whether the dog runs at a slow/normal/fast or a single speed is determined by <see cref="ControlMode"/>.
/// </summary>
/// <dependency cref="HerderLoopBehaviour" />
/// <dependend cref="HerderLoopBehaviour" />
public class ControlHerderBehaviour : MonoBehaviour {
    public AudioClip SOUND_DOGBARK;

    /// <summary>
    /// Specifies the time in which the path is completely redrawn. This makes sure the speed material of the path is properly updated.
    /// </summary>
    private const float RedrawPathTime = 0.100f;
    
    /// <summary>
    /// Specifies the drawing mode for the herder
    /// </summary>
    public enum DrawMode {
        SingleSpeed = 0,
        MultipleSpeed = 1
    }

    /// <summary>
    /// Defines the number of different shaders used for drawing the road
    /// </summary>
    public const int NumberOfWayShaders = 3;

    private LineRenderer lineRenderer;
    private HerderLoopBehaviour herderLoopController;
    private Timer redrawPathTimer;

    /// <summary>
    /// Specifies the mouse button to check. By default, the left mouse button.
    /// </summary>
    /// <seealso cref="MouseManager.MouseButton"/>
    public MouseManager.MouseButton MouseButtonToCheck = MouseManager.MouseButton.Left;

    /// <summary>
    /// Specifies the control mode for the dog
    /// </summary>
    public DrawMode ControlMode = DrawMode.MultipleSpeed;

    /// <summary>
    /// Specifies the minimum distance in units for each individual waypoint
    /// </summary>
	public float WaypointSpacing = 5f;

    /// <summary>
    /// Specifies the material used for drawing the 'slow' path.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public Material SlowPathMaterial;

    /// <summary>
    /// Specifies the material used for drawing the 'normal' speed path.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public Material NormalPathMaterial;

    /// <summary>
    /// Specifies the material used for drawing the 'fast' path.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public Material FastPathMaterial;

    /// <summary>
    /// Specifies the material used for drawing the single speed path
    /// </summary>
    public Material SingleSpeedPathMaterial;

    /// <summary>
    /// Specifies the radius around the dog that allows the user to select the dog. 
    /// </summary>
    /// <remarks>The dog is usually to small in the camera view. This allows an greater range for the dog to be selected.</remarks>
    public float SelectRadius = 2f; // Sphere radius for the dog selector.

    /// <summary>
    /// Specifies the sphere casting range for selecting the dog. The length of the cast.
    /// </summary>
    /// <remarks>Sphere casting is heavy, so keep the range low for a better performance</remarks>
    public float SelectionRange = 150f;

    /// <summary>
    /// Specifies the layer the path is drawn on
    /// </summary>
    private const int LayerToCheck = 1 << 8; // Layer 8 is de layer waar hij mee raycast.

    /// <summary>
    /// Specifies the fail safe value used for detecting buggy paths, this is the maximum distance for two waypoints.
    /// </summary>
    private const float WaypointDistanceFailsafeThreshold = 15f;

    /// <summary>
    /// The base object to draw a path with. Should be an object with a LineRenderer.
    /// </summary>
    public GameObject ShepherdPathPrefab;
	
    /// <summary>
    /// Specifies if we're currently drawing a path
    /// </summary>
    private bool isCurrentlyDrawing = false;

    /// <summary>
    /// Specifies the last added point of the path
    /// </summary>
    private Vector3 lastAddedPosition = Vector3.zero;

    /// <summary>
    /// Specifies the current trajectory of the path
    /// </summary>
    private Queue<Vector3> currentTrajectory;
	
    /// <summary>
    /// Absolute time in seconds of starting the path drawing
    /// </summary>
    private float startTime;

    private float totalPathLength;

    // Maximum height
    private float maxHeight;

    void Start(){
        // get a line renderer
		GameObject pathRenderingObject = (GameObject) Instantiate(this.ShepherdPathPrefab);
        this.lineRenderer = pathRenderingObject.GetComponent<LineRenderer>();

        // get our HerderLoopBehaviour component
	    this.herderLoopController = this.gameObject.GetComponent<HerderLoopBehaviour>();
        if (this.herderLoopController == null) {
            throw new Exception("This component cannot function without a HerderLoopBehaviour");
        }

        this.Halo.enabled = false;

        // set-up path redraw timer
        this.redrawPathTimer = new Timer(RedrawPathTime);
	}

    void OnMouseOver() {
        this.Halo.enabled = true;
    }

    private Behaviour Halo {
        get { return ((Behaviour) this.gameObject.GetComponent("Halo")); }
    }

    void OnMouseExit() {
        this.Halo.enabled = false;
    }

	void Update () {
        // end the draw of an existing path
        if (this.isCurrentlyDrawing && this.IsMouseButtonUp()) {
            (gameObject.GetComponent("Halo") as Behaviour).enabled = false;
            this.isCurrentlyDrawing = false;
            this.herderLoopController.StartWalking(this.ControlMode, this.currentTrajectory, this.CalculateTotalDrawTime(), this.totalPathLength);
            MouseManager.ReleaseLock(this);
            return;
        }

        // start drawing of a new path
        if (!MouseManager.IsMouseLocked && !this.isCurrentlyDrawing && this.IsMouseButtonDown() && this.herderLoopController.AcceptsNewPath()) {
            // get the position of the mouse in the world and check if we are hit
            RaycastHit? hit = this.GetMousePosition();

            if (hit != null && hit.Value.collider.gameObject == this.gameObject && hit.Value.point.y < maxHeight) {
                // acquire mouse lock and enable drawing state
                if (MouseManager.TryAcquireLock(this)) {
                    (gameObject.GetComponent("Halo") as Behaviour).enabled = true;
                    this.herderLoopController.CancelWalk();

                    this.isCurrentlyDrawing = true;
                    this.currentTrajectory = new Queue<Vector3>();
                    this.startTime = Time.time;
                    this.totalPathLength = 0;

                    this.redrawPathTimer.Reset();
                    audio.PlayOneShot(SOUND_DOGBARK);
                }
            }
        }

        // continue drawing of current path
        if (this.isCurrentlyDrawing) {
            this.redrawPathTimer.Update();
            (gameObject.GetComponent("Halo") as Behaviour).enabled = true;

            // get the current mouse position, and seed the path if there is none
            Vector3 position = this.GetTerrainMousePosition();

            // Check if the position is above height treshold
            if (position.y > maxHeight) {
                //this.herderLoopController.StartWalking(this.ControlMode, this.currentTrajectory, this.CalculateTotalDrawTime(), this.totalPathLength);
                return;

            }
            if (this.currentTrajectory.Count == 0) {
                this.currentTrajectory.Enqueue(position);
                this.lastAddedPosition = position;
                return;
            }

            // check the distance and add the path to the list
            bool isPathUpdated = false;
            float positionDistance = Vector3.Distance(position, this.lastAddedPosition);
            if (positionDistance > this.WaypointSpacing && 
                positionDistance < WaypointDistanceFailsafeThreshold) {
                this.currentTrajectory.Enqueue(position);
                this.lastAddedPosition = position;
                this.totalPathLength += positionDistance;

                isPathUpdated = true;
            }

            // redraw the path, either on timer basis or when the path is updated
            if (isPathUpdated || this.redrawPathTimer.IsTriggered) {
                this.redrawPathTimer.Reset();

                this.DrawPath();
            }
        }

	}

    private float CalculateTotalDrawTime() {
        float endTime = Time.time;

        return (endTime - this.startTime);
    }

	private void DrawPath(){
        // set the positions of the line points
		Vector3[] t = this.currentTrajectory.ToArray();
		this.lineRenderer.SetVertexCount(t.Length);
		for(int i = 0; i < t.Length; i++) {
			Vector3 p = t[i];
			p.y += 0.2f;
        	this.lineRenderer.SetPosition(i, p);
    	}

        // determine which shader to use for drawing the line
        if (this.ControlMode == DrawMode.SingleSpeed) {
            this.lineRenderer.renderer.material = this.SingleSpeedPathMaterial;
            return;
        }

        // ... calculate speed per unit
        float drawTime = this.CalculateTotalDrawTime() / this.herderLoopController.TimeDivider;
        float speedPerUnit = this.totalPathLength / drawTime;
	    speedPerUnit *= this.herderLoopController.MovementSpeedFactor;
        speedPerUnit = Math.Max(speedPerUnit, this.herderLoopController.MinimumSpeedPerUnit);
        speedPerUnit = Math.Min(speedPerUnit, this.herderLoopController.MaximumSpeedPerUnit);

        // ... calculate the division scale and the _lower_ bounds of the normal and fast speed
	    float diff = this.herderLoopController.MaximumSpeedPerUnit - this.herderLoopController.MinimumSpeedPerUnit;
	    float division = diff / NumberOfWayShaders;

	    float normalShader = this.herderLoopController.MinimumSpeedPerUnit + division;
        float fastShader = normalShader + division;
        
        if (speedPerUnit > fastShader) {
            this.lineRenderer.renderer.material = this.FastPathMaterial;
        } else if (speedPerUnit > normalShader) {
            this.lineRenderer.renderer.material = this.NormalPathMaterial;
        } else {
            this.lineRenderer.renderer.material = this.SlowPathMaterial;
        }
	}
	
    /// <summary>
    /// Signals the script that the path drawn has been completed by either an succesfull finish or an abortion
    /// </summary>
	public void OnPathFinished(bool cancelled){
		this.lineRenderer.SetVertexCount(0);
	}

    #region Input Helpers

    /// <summary>
    /// Gets the current position of the mouse on the terrain
    /// </summary>
    /// <returns></returns>
    private Vector3 GetTerrainMousePosition() {
        RaycastHit? hit = this.GetMousePosition(LayerToCheck);

        if (hit != null) {
            return hit.Value.point;
        }

        return Vector3.zero;
    }

    private RaycastHit? GetMousePosition() {
        return this.GetMousePosition(-1, true);
    }

    private RaycastHit? GetMousePosition (int layerToCheck) {
        return this.GetMousePosition(layerToCheck, false);
    }

    private RaycastHit? GetMousePosition(int layerToCheck, bool sphere) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
       
        RaycastHit hit;
        if (sphere) {
            RaycastHit[] hits = Physics.SphereCastAll(ray, this.SelectRadius, SelectionRange, layerToCheck);
            if (hits.Length != 0) {
                foreach (RaycastHit hitTest in hits) {
                    if (hitTest.collider.CompareTag(Tags.Shepherd)) {
                        return hitTest;
                    }
                }
            }
        } else {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerToCheck)) {
                return hit;
            }
        }
        return null;
    }

    private bool IsMouseButtonDown() {
        return Input.GetMouseButton((int)this.MouseButtonToCheck);
    }

    private bool IsMouseButtonUp() {
        return !Input.GetMouseButton((int)this.MouseButtonToCheck);
    }

    #endregion

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(this.transform.position, this.SelectRadius);
    }

}
