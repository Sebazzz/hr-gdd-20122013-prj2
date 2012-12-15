using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This script executes the walk of a trajectory, when enabled. The script is enabled by calling <see cref="StartWalking"/>, 
///     and can be disabled at any time by calling <see cref="CancelWalk"/>
/// </summary>
/// <remarks>
///     <para>
///         This script executes the walk of the trajectory set by <see cref="ControlHerderBehaviour"/>. The method <see cref="StartWalking"/> will enable the script
///         and execute the trajectory. Once the trajectory is finished the <see cref="ControlHerderBehaviour"/>.<see cref="ControlHerderBehaviour.OnPathFinished"/> method is called
///         and the script will disable itself.
///     </para>
///     <para>
///         The trajectory walked by this script can be disabled by calling <see cref="CancelWalk"/>. Please note the <see cref="ControlHerderBehaviour.OnPathFinished"/> method 
///         will still be called, to make sure any resources from the walking process are properly cleaned up.
///     </para>
/// </remarks>
/// <dependency cref="ControlHerderBehaviour"/>
/// <dependend cref="ControlHerderBehaviour"/>
public class HerderLoopBehaviour : MonoBehaviour {
    private Queue<Vector3> currentTrajectory;
    private Vector3 currentTarget = Vector3.zero;
    private float desiredSpeed;

    private Vector3 lastTarget;
    private float walkStartTime;
    private Timer stuckCheckTimer;
    
    /// <summary>
    /// Sets the minimum number of units that are traversed per second
    /// </summary>
    public float MinimumSpeedPerUnit = 3f;

    /// <summary>
    /// Defines the maximum speed of the dog: number of units traversed per second. If this is above a certain threshold the dog wil be flipping and uncontrollable.
    /// </summary>
    public float MaximumSpeedPerUnit = 25f;

    /// <summary>
    /// Specifies how much to multiply the speed calculated from the draw time
    /// </summary>
    public float MovementSpeedFactor = 1f;

    /// <summary>
    /// Specifies where to divide the time with to correct the speed
    /// </summary>
    public float TimeDivider = 1f;

    /// <summary>
    /// Specifies the radius around waypoints to enter in order to consider the waypoint 'reached'
    /// </summary>
    /// <remarks>
    /// Drawn as a <see cref="Color.cyan"/> gizmo
    /// </remarks>
    public float WaypointAcceptRadius = 2f;

    /// <summary>
    /// Specifies the percentage that is used for checking if the desired walking speed is reached. If the walking speed is under this percentage specified, the path is cancelled.
    /// </summary>
    /// <remarks>
    /// If this value is set too high, the dog may abort any path too soon.
    /// </remarks>
    public float CancelPathSpeedPercentage = 50;

    /// <summary>
    /// Specifies the time frame used for checking if the dog is stuck. 
    /// </summary>
    /// <remarks>
    /// If this value is set too low, the dog may abort any path too soon.
    /// </remarks>
    public float StuckCheckTime = 0.5f;

    /// <summary>
    /// Gets the current target the dog walks to
    /// </summary>
    public Vector3 CurrentTarget {
        get { return this.currentTarget; }
    }

    private void Start() {
        this.stuckCheckTimer = new Timer(StuckCheckTime);
    }

    /// <summary>
    /// Starts walking the specified trajectory. Defines the time used for drawing the path. This will be used to calculate the speed of traversing the path.
    /// </summary>
    /// <param name="trajectory"></param>
    /// <param name="totalDrawingTime"></param>
    /// <param name="totalPathLength"></param>
    public void StartWalking (Queue<Vector3> trajectory, float totalDrawingTime, float totalPathLength) {
        if (this.enabled) {
            throw new Exception("Currently already running a path. Call CancelWalk first.");
        }

        this.enabled = true;
        this.currentTrajectory = trajectory;

        // seed the state
        this.currentTarget = this.currentTrajectory.Dequeue();
        this.lastTarget = this.transform.position;
        this.walkStartTime = Time.time;

        // calculate speed per unit and correct accordingly
        float speedPerUnit = totalPathLength / (totalDrawingTime/this.TimeDivider);
        speedPerUnit *= this.MovementSpeedFactor;
        speedPerUnit = Math.Max(speedPerUnit, this.MinimumSpeedPerUnit);
        speedPerUnit = Math.Min(speedPerUnit, this.MaximumSpeedPerUnit);

        // calculate the final speed
        this.desiredSpeed = speedPerUnit;

    //    Debug.Log(
    //String.Format(
    //    "distance: {0}; time: {1};  init. speed: {3}; recalc speed: {4}; final speed: {2};",
    //    totalPathLength,
    //    totalDrawingTime,
    //    speedPerUnit,
    //    totalPathLength / totalDrawingTime,
    //    totalPathLength / totalDrawingTime * this.MovementSpeedFactor),
    //    this.gameObject);
    }

    /// <summary>
    /// Cancels the current trajectory being walked. Note: this will also call <see cref="ControlHerderBehaviour"/>.<see cref="ControlHerderBehaviour.OnPathFinished"/>.
    /// </summary>
    public void CancelWalk() {
        this.FinishWalk(true);
    }

    private void FinishWalk(bool cancel) {
        this.enabled = false;

        this.currentTrajectory = null;
        this.GetComponent<ControlHerderBehaviour>().OnPathFinished(cancel);
    }
    
    private void Update() {
        // no trajectory to walk, nothing to do
        if (this.currentTrajectory == null) {
            // we even shouldn't be here: if state management were handled properly currentTrajectory would never be null when the script is enabled
            return;
        }

        // do some checks
        bool trajectoryFinished = this.currentTrajectory.Count <= 0;
        bool targetReached = this.CheckReachedTarget();

        if (!targetReached) {
            this.stuckCheckTimer.Update();

            if (this.stuckCheckTimer.IsTriggered) {
                // check if we're stuck
                if (this.CheckStuck()) {
                    Debug.LogWarning(this.gameObject.name + " is stuck. Canceling walk.");
                    this.CancelWalk();
                    return;
                }

                this.stuckCheckTimer.Reset();
            }


            // if we've not reached the target and we're not stuck, FixedUpdate will handle the rest
            return;
        }

        // if we have no targets to 
        if (trajectoryFinished) {
            this.FinishWalk(false);
            return;
        }

        // set a new target, since we have still a target to reach
        this.currentTarget = this.currentTrajectory.Dequeue();
        this.walkStartTime = Time.time;
        this.lastTarget = this.transform.position;
    }
    
    /// <summary>
    /// Returns a value indicating if we are now stuck.
    /// </summary>
    /// <returns></returns>
    private bool CheckStuck() {
        // calculate allowable speed
        float minimalAllowableSpeed = this.desiredSpeed * (this.CancelPathSpeedPercentage / 100f);

        // calculate speed per unit
        float timeDiff = Time.time - this.walkStartTime;
        float distance = GetDistanceWithoutY(this.transform.position, this.lastTarget);
        float speedPerUnit = distance / timeDiff;

        Debug.Log(String.Format("Desired speed: {2}; Speed per unit: {0}; Allowable speed: {1}", speedPerUnit, minimalAllowableSpeed, this.desiredSpeed));
        return speedPerUnit < minimalAllowableSpeed;
    }

    private void FixedUpdate() {
        System.Diagnostics.Debug.Assert(this.enabled);

        // calculate a new rotation position
        Vector3 lookPos = this.currentTarget - this.transform.position;
        lookPos.y = 0;

        Quaternion rotation = Quaternion.LookRotation(lookPos);

        // apply an rotation that is smoothly interpreted to the target
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, Time.fixedDeltaTime * 20);

        // calculate speed and execute
        Vector3 speed = this.transform.TransformDirection(Vector3.forward) * this.desiredSpeed * Time.fixedDeltaTime;
        this.transform.position += speed;
    }

    /// <summary>
    ///     Returns a value indicating if we're reached our current path target
    /// </summary>
    /// <returns></returns>
    private bool CheckReachedTarget() {
        // check if we distance is in the required radius
        return GetDistanceWithoutY(this.transform.position, this.currentTarget) < this.WaypointAcceptRadius;
    }


    private static float GetDistanceWithoutY (Vector3 currentPosition, Vector3 targetPosition) {
        Vector2 currentPosition2 = new Vector2(currentPosition.x, currentPosition.z);
        Vector2 targetPosition2 = new Vector2(targetPosition.x, targetPosition.z);

        return Vector2.Distance(currentPosition2, targetPosition2);
    }
}