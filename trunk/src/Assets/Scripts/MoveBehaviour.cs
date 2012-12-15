using System;
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

    /// <summary>
    /// Defines if this script should ignore the Y-axis for moving
    /// </summary>
    public bool IgnoreYAxis = true;

    /// <summary>
    /// Gets the current direction of movement. See also <see cref="GameObject.transform"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">This object is currently moving towards an target and not towards a direction</exception>
    public Quaternion MovementDirection {
        get {
            if (!this.targetAsDirection) {
                throw new InvalidOperationException("This object is currently moving towards an target and not towards a direction");
            }
            
            return this.transform.rotation;
        }
    }

    /// <summary>
    /// Gets if this object is moving towards a point and not towards a direction.
    /// </summary>
    /// <remarks>
    /// If this property returns <c>true</c>, the <see cref="MovementDirection"/> property is not usable.
    /// If this property returns <c>false</c>, the <see cref="MovementTarget"/> property is not usable.
    /// </remarks>
    public bool IsMovingToPoint {
        get { return !this.targetAsDirection; }
    }

    /// <summary>
    /// Gets the current movement target
    /// </summary>
    /// <exception cref="InvalidOperationException">This object is currently moving towards an direction and not towards a target</exception>
    public Vector3 MovementTarget {
        get {
            if (this.targetAsDirection) {
                throw new InvalidOperationException("This object is currently moving towards an direction and not towards a target");
            }

            return this.target;
        }
    }

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	    this.MoveSingleStep();
	}

    /// <summary>
    /// Executes a single movement after configuration via <see cref="MoveTo"/>, <see cref="MoveInCurrentDirection"/> or <see cref="MoveToDirection"/>
    /// </summary>
    public void MoveSingleStep() {
        if (this.targetAsDirection) {
            this.MoveTowardsDirection(false);
        } else {
            this.MoveTowardsPoint(false);
        }
    }

    /// <summary>
    /// Executes a single movement after configuration via <see cref="MoveTo"/>
    /// </summary>
    public void MoveSingleStepBackup() {
        if (this.targetAsDirection) {
            this.MoveTowardsDirection(true);
        } else {
            this.MoveTowardsPoint(true);
        }
    }

    /// <summary>
    /// Executes movement logic to move to a specific point as set in the <see cref="MoveTo"/> method (<see cref="target"/>)
    /// </summary>
    private void MoveTowardsPoint(bool inverse) {
        Vector3 currentPosition = this.gameObject.transform.position;

        // check if we reached the target
        if ((!IgnoreYAxis && Vector3.Distance(currentPosition, target) < TargetRadius) ||
            (IgnoreYAxis && Vector2.Distance(new Vector2(currentPosition.x, currentPosition.z), new Vector2(target.x, target.z)) < TargetRadius)) {
            // we're done
            this.Stop();
            return;
        }

        // calculate the movement 
        Vector3 difference = target - currentPosition;
        Vector3 currentMovement = difference * Time.fixedDeltaTime * Speed;

        // check if we will not cross over the target
        Vector3 targetAfterMovement = currentPosition + currentMovement;

        if (inverse) {
            targetAfterMovement *= -1;
        }

        if (this.IgnoreYAxis) {
            targetAfterMovement.y = this.transform.position.y;
        }

        this.transform.position = targetAfterMovement;
    }

    /// <summary>
    /// Executes movement logic to move to a specific direction as set in the <see cref="MoveToDirection"/> method (<see cref="target"/>)
    /// </summary>
    private void MoveTowardsDirection(bool inverse) {
        float finalSpeed = this.Speed * Time.fixedDeltaTime;

        if (inverse) {
            finalSpeed *= -1;
        }

        // calculate movement
        this.gameObject.transform.Translate(0, 0, finalSpeed);
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
