using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class CameraFollowMassBehaviour : MonoBehaviour {
    private Vector3 startCameraPosition;

    /// <summary>
    /// Specifies the objects with the tag to follow
    /// </summary>
    public string TagToFollow = null;

    /// <summary>
    /// Specifies the threshold used for following groups of objects. Objects that differ too much from the average of positions, will not be followed.
    /// </summary>
    public float FollowThreshold = 50;

    /// <summary>
    /// Zoom factor for the case when all the dogs are spread out
    /// </summary>
    public float ZoomFactor = 10;

    /// <summary>
    /// Specifies the camera to follow. By default is this the <see cref="Camera.mainCamera"/>, if this field is set to null.
    /// </summary>
    public Camera CameraToFollow = null;

	// Use this for initialization
	void Start () {
	    if (this.TagToFollow == null) {
	        throw new UnityException("No 'TagToFollow' parameter configured");
	    }

	    this.CameraToFollow = this.CameraToFollow ?? Camera.mainCamera;
	    this.startCameraPosition = this.CameraToFollow.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	    if (TagToFollow == null) {
	        // don't disable script, but won't do anything either
            return;
	    }

        // find our game objects first
	    GameObject[] objectsToFollow = GameObject.FindGameObjectsWithTag(this.TagToFollow);

        if (objectsToFollow == null || objectsToFollow.Length == 0) {
            return; // don't touch the camera then
        }

        // determine the average position
	    float xSum = 0, zSum = 0;
	    foreach (GameObject obj in objectsToFollow) {
	        Vector3 position = obj.transform.position;

            xSum += position.x;
            zSum += position.z;
	    }

        float xAvg = xSum / objectsToFollow.Length;
        float zAvg = zSum / objectsToFollow.Length;
        Vector2 avgPosition = new Vector2(xAvg, zAvg);

        // check for each object if their position differs too much from the average
        List<GameObject> objectsOutOfBounds = new List<GameObject>(objectsToFollow.Length);
	    foreach (GameObject obj in objectsToFollow) {
            Vector2 position = new Vector2(obj.transform.position.x, obj.transform.position.z);

	        float length = Mathf.Abs(Vector2.Distance(position, avgPosition));

            if (length > FollowThreshold) {
                // we don't subtract anything from the current average just yet: it will influence future comparisons to objects
                objectsOutOfBounds.Add(obj);
            }
	    }

        // determine the final average position
	    Vector3 cameraPosition;
        if (objectsToFollow.Length == objectsOutOfBounds.Count) {
            // since all objects are out of bounds, we may just as well take the average position
            cameraPosition = new Vector3(xAvg, this.startCameraPosition.y, zAvg);

            // determine the min and max positions in the view port
            // minY, minX, maxX, and maxY will be around -1 or 1
            bool first = true;
            float minX = 0, minY = 0, maxX = 0, maxY = 0;
            foreach (GameObject objectToFollow in objectsToFollow) {
                Vector3 viewPortPoint = this.CameraToFollow.WorldToViewportPoint(objectToFollow.transform.position);

                if (first) {
                    first = false;
                    maxX = minX = viewPortPoint.x;
                    maxY = minY = viewPortPoint.y;
                } else {
                    minX = Mathf.Min(minX, viewPortPoint.x);
                    minY = Mathf.Min(minY, viewPortPoint.y);
                    maxX = Mathf.Max(maxX, viewPortPoint.x);
                    maxY = Mathf.Max(maxY, viewPortPoint.y);
                }
            }

            // determine new position for camera
            float zoomOutFactor = (maxX - minX) * ZoomFactor;
            cameraPosition.y += zoomOutFactor;

            float moveZfactor = (maxY - minY) * ZoomFactor;
            cameraPosition.z += moveZfactor;

            // TODO: implement proper zooming so we can view all the objects except of the morst far object
        } else {
            // substract each of the 'out of bounds' objects from the average to determine the new sum of positions
            foreach (GameObject objectsOutOfBound in objectsOutOfBounds) {
                Vector3 position = objectsOutOfBound.transform.position;

                xSum -= position.x;
                zSum -= position.z;
            }

            // determine the average
            int count = objectsToFollow.Length - objectsOutOfBounds.Count;
            xAvg = xSum / count;
            zAvg = zSum / count;

            cameraPosition = new Vector3(xAvg, this.startCameraPosition.y, zAvg);
        }

	    this.CameraToFollow.transform.position = cameraPosition;
	}
}
