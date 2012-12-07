using UnityEngine;

/// <summary>
/// Enables movement of the object
/// </summary>
public class MoveBehaviour : MonoBehaviour {
    /// <summary>
    /// Target of the movement. Only the <see cref="Vector3.x"/> and <see cref="Vector3.z"/> components will be used.
    /// </summary>
    private Vector3 target;

    /// <summary>
    /// Takes the target as direction
    /// </summary>
    private bool targetAsDirection;

    /// <summary>
    /// The number of units the object will move per time
    /// </summary>
    public float Speed = 5;

    /// <summary>
    /// Defines the number of units to 
    /// </summary>
    public float TargetRadius = 2;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	   if (targetAsDirection) {
	       this.MoveTowardsDirection();
	   } else {
	       this.MoveTowardsPoint();
	   }
	}

    /// <summary>
    /// Executes movement logic to move to a specific point as set in the <see cref="MoveTo"/> method (<see cref="target"/>)
    /// </summary>
    private void MoveTowardsPoint() {
        Vector3 currentPosition = this.gameObject.transform.position;

        // check if we reached the target
        if (Vector3.Distance(currentPosition, target) < TargetRadius) {
            // we're done
            this.Stop();
            return;
        }

        // calculate the movement 
        Vector3 difference = target - currentPosition;
        Vector3 currentMovement = difference * Time.deltaTime * Speed;

        // check if we will not cross over the target
        Vector3 targetAfterMovement = currentPosition + currentMovement;

        this.transform.position = targetAfterMovement;
    }

    /// <summary>
    /// Executes movement logic to move to a specific direction as set in the <see cref="MoveToDirection"/> method (<see cref="target"/>)
    /// </summary>
    private void MoveTowardsDirection() {
        // calculate movement
        this.gameObject.transform.Translate(0, 0, this.Speed * Time.deltaTime);
    }

    /// <summary>
    /// Enables the script for movement towards the specified target. Only the <see cref="Vector3.x"/> and <see cref="Vector3.z"/> components will be used.
    /// </summary>
    /// <param name="targetPoint"></param>
    public void MoveTo (Vector3 targetPoint) {
        this.enabled = true;

        this.target = targetPoint;
        this.targetAsDirection = false;

        this.transform.LookAt(targetPoint);
    }

    /// <summary>
    /// Enables the script for movement in the direction of the current game object
    /// </summary>
    /// <param name="speed"></param>
    public void MoveInCurrentDirection (float speed) {
        this.Speed = speed;

        this.MoveToDirection(this.transform.rotation);
    }

    /// <summary>
    /// Enables the script for movement in the direction of the specified target.
    /// </summary>
    /// <param name="directionTarget"></param>
    public void MoveToDirection(Quaternion directionTarget) {
        MoveToDirection(directionTarget, this.Speed);
    }

    /// <summary>
    /// Enables the script for movement in the direction of the specified target.
    /// </summary>
    /// <param name="directionTarget"></param>
    /// <param name="speed"></param>
    public void MoveToDirection(Quaternion directionTarget, float speed) {
        this.enabled = true;

        this.targetAsDirection = true;
        this.Speed = speed;

        this.transform.rotation = directionTarget;
    }

    /// <summary>
    /// Stops the movement
    /// </summary>
    public void Stop() {
        this.enabled = false;
    }

    /// <summary>
    /// Executes movement in the rotation of the current object, moves in the current speed
    /// </summary>
    public void MoveInCurrentDirection() {
        this.MoveInCurrentDirection(this.Speed);
    }
}
