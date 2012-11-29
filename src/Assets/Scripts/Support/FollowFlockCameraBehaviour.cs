using UnityEngine;
using System.Collections;

public class FollowFlockCameraBehaviour : MonoBehaviour
{

    private Vector3 cameraLookAtTarget = Vector3.zero;
    private Vector3 cameraLookAtTargetPrevious = Vector3.zero;
    private Vector3 cameraToTargetDistance = new Vector3(0f, 20f, -7.5f);

    public const string FollowTag = "CameraFollowed";

    public float FollowThreshold = 15f;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    private void Start(){
        transform.position = new Vector3(
                cameraLookAtTarget.x + cameraToTargetDistance.x,
                cameraLookAtTarget.y + cameraToTargetDistance.y,
                cameraLookAtTarget.z + cameraToTargetDistance.z);
        
        transform.LookAt(cameraLookAtTarget);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update(){
        cameraLookAtTarget = GetFlockMidpoint();

        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(
                cameraLookAtTarget.x + cameraToTargetDistance.x,
                cameraLookAtTarget.y + cameraToTargetDistance.y,
                cameraLookAtTarget.z + cameraToTargetDistance.z),
            0.1f);
    }

    /// <summary>
    /// Find and return the center vector of the flock
    /// </summary>
    private Vector3 GetFlockMidpoint(){
        Vector3 flockCenter = new Vector3();
        GameObject[] followObjects = GameObject.FindGameObjectsWithTag(FollowTag);
        int sheepCount = 0;

        foreach (GameObject sheep in followObjects) {
            flockCenter += sheep.transform.position;
            sheepCount++;
        }
        if (sheepCount > 0) flockCenter /= sheepCount;
        
        Vector3 controlFlockCenter = flockCenter;
        foreach (GameObject sheep in followObjects) {
            if (Vector3.Distance(sheep.transform.position, controlFlockCenter) > FollowThreshold) {
                flockCenter *= sheepCount;
                flockCenter -= sheep.transform.position;
                sheepCount--;
                if (sheepCount > 0) flockCenter /= sheepCount;
            }
        }

        return flockCenter;
    }
}
