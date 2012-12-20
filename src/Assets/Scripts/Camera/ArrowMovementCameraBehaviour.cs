using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// Allows camera movement by a specified dragging using a mouse button and arrow keys
/// </summary>
[RequireComponent(typeof(Camera))]
public class ArrowMovementCameraBehaviour : MonoBehaviour {
    private Vector3 onMouseDownCursorPoint = new Vector3();
	private Vector3 onMouseDownCameraPosition = new Vector3();
    private bool isCurrentlyDragging = false;

    private static readonly Dictionary<KeyCode, Vector3> MovementMultipliers = new Dictionary<KeyCode, Vector3>() {
        {KeyCode.UpArrow, new Vector3(0, 0, 1)},
        {KeyCode.DownArrow, new Vector3(0, 0, -1)},
        {KeyCode.LeftArrow, new Vector3(-1, 0, 0)},
        {KeyCode.RightArrow, new Vector3(1, 0, 0)}
    }; 

    /// <summary>
    /// Specifies the bounds of the camera movement bounding box
    /// </summary>
	public Vector2 BoundingBoxBottomLeft;

    /// <summary>
    /// Specifies the bounds of the camera movement bounding box
    /// </summary>
	public Vector2 BoundingBoxTopRight;

    /// <summary>
    /// Specifies the mouse button used for dragging
    /// </summary>
    public MouseManager.MouseButton MouseButtonToCheck = MouseManager.MouseButton.Left;

    /// <summary>
    /// Specifies the movement speed for the arrows
    /// </summary>
    public float MovementSpeed = 5f;

	/// <summary>
	/// Use this for initialization
	/// </summary>
	private void Start () {

	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	private void Update () {
        // execute mouse movement
        //if (this.IsMouseButtonDown() && !MouseManager.TryAcquireLock(this)) {
        //} else {
        //    if (this.IsMouseButtonDown()) OnMouseDown();

        //    if (this.IsMouseButton() && this.onMouseDownCameraPosition != Vector3.zero) OnMouse();

        //    if (this.IsMouseButtonUp()) OnMouseUp();
        //}

	    if (Input.GetMouseButtonDown((int) this.MouseButtonToCheck) && !MouseManager.TryAcquireLock(this)) {

	    } else {

	        if (this.IsMouseButtonDown())
	            OnMouseDown();
	        if (this.IsMouseButton() && onMouseDownCameraPosition != Vector3.zero)
	            OnMouse();
	        if (this.IsMouseButtonUp())
	            OnMouseUp();
	    }

        // execute keyboard movement
        if (!this.isCurrentlyDragging) {
            foreach (KeyValuePair<KeyCode, Vector3> controlPair in MovementMultipliers) {
                KeyCode code = controlPair.Key;

                if (Input.GetKey(code)) {
                    Vector3 movementSpeed = controlPair.Value * this.MovementSpeed;

                    this.transform.Translate(movementSpeed, Space.World);
                }
            }
        }

	    // clamp our position
		float x = transform.position.x;
		float z = transform.position.z;

		if (transform.position.x < BoundingBoxBottomLeft.x) x = BoundingBoxBottomLeft.x;
		if (transform.position.x > BoundingBoxTopRight.x) x = BoundingBoxTopRight.x;
		if (transform.position.z < BoundingBoxBottomLeft.y) z = BoundingBoxBottomLeft.y;
		if (transform.position.z > BoundingBoxTopRight.y) z = BoundingBoxTopRight.y;

		transform.position = new Vector3(x, transform.position.y, z);

	}

	/// <summary>
	/// Called when MouseButton is pressed for the first time
	/// Creates an offset with which you can calculate the camera movement
	/// </summary>
	private void OnMouseDown() {
		this.onMouseDownCursorPoint = Input.mousePosition;
		this.onMouseDownCameraPosition = transform.position;

	    this.isCurrentlyDragging = true;
	}

	/// <summary>
	/// Called when MouseButton is held down
	/// Used to calculate the movement of the camera
	/// </summary>
	private void OnMouse() {
		transform.position = new Vector3(
			this.onMouseDownCameraPosition.x + ((Input.mousePosition.x - this.onMouseDownCursorPoint.x) * -0.1f),
			this.onMouseDownCameraPosition.y,
			this.onMouseDownCameraPosition.z + ((Input.mousePosition.y - this.onMouseDownCursorPoint.y) * -0.1f));
	}

	/// <summary>
	/// Reset OnMouseDownCameraPosition, so it can't effect the following touchdown
	/// </summary>
	private void OnMouseUp() {
		this.onMouseDownCameraPosition = Vector3.zero;
	    this.isCurrentlyDragging = false;
		
		if(MouseManager.CurrentLockOwner == this)
			MouseManager.ReleaseLock(this);
	}

    private bool IsMouseButtonDown() {
        return Input.GetMouseButtonDown((int)this.MouseButtonToCheck);
    }

    private bool IsMouseButton() {
        return Input.GetMouseButton((int)this.MouseButtonToCheck);
    }

    private bool IsMouseButtonUp() {
        return Input.GetMouseButtonUp((int)this.MouseButtonToCheck);
    }

    void OnDrawGizmosSelected() {
        // draw the bounding box for the camera
        // ... calculate center and size
        Vector3 center = (this.BoundingBoxTopRight / 2f) + this.BoundingBoxBottomLeft;
        Vector3 size = this.BoundingBoxTopRight - this.BoundingBoxBottomLeft;

        // ... actually the Y is the Z-axis, so swap it 
        center.z = center.y;
        size.z = size.y;

        // ... calculate y size by finding the terrain
        Terrain t = FindObjectOfType(typeof (Terrain)) as Terrain;
        if (t != null) {
            size.y = 50;  //this.transform.position.y - t.transform.position.y;
        } else {
            size.y = 50;
        }

        center.y = this.transform.position.y - (size.y / 2f);

        Gizmos.DrawWireCube(center, size);
        Gizmos.DrawWireCube(this.transform.position, new Vector3(6,6,6));

        // draw the rays for each of the sides
        // TODO
    }

    
}
