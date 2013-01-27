using System.Collections.Generic;
using UnityEngine;

public class SheepBarnTouchBehaviour : MonoBehaviour {

    /// <summary>
    /// Defines the distance to the endpoint for it to be considered reached
    /// </summary>
    public float EndPointDistance = 1f;
    
    /// <summary>
    /// Defines the speed the dog and sheep walk into the barn
    /// </summary>
    public float WalkSpeed = 1f;

    private GameObject endPointTarget;
    private List<GameObject> walkingObjects;

    private void Start() {
        this.walkingObjects = new List<GameObject>();
        this.endPointTarget =
            this.transform.parent.GetComponentInChildren<SheepBarnTargetWalkPositionVisualiser>().gameObject;

        if (this.endPointTarget == null) {
            throw new UnityException("Improper object configuration");
        }
    }

    private void Update() {
        if (this.walkingObjects.Count <= 0) {
            return;
        }

        var targetPos = new Vector2(this.endPointTarget.transform.position.x, this.endPointTarget.transform.position.z);

        // move each sheep
        var removeableObjects = new List<GameObject>();
        foreach (GameObject gObject in this.walkingObjects) {
            Vector2 currentPos = new Vector2(gObject.transform.position.x, gObject.transform.position.z);
            Vector2 newPosition = Vector2.Lerp(currentPos, targetPos, Time.deltaTime*WalkSpeed);

            gObject.transform.position = new Vector3(newPosition.x, gObject.transform.position.y, newPosition.y);

            // check if we're reached the position
            currentPos = new Vector2(gObject.transform.position.x, gObject.transform.position.z);

            if (Vector2.Distance(currentPos, targetPos) < this.EndPointDistance) {
                removeableObjects.Add(gObject);
            }
        }

        // remove obsolete sheep
        foreach (GameObject removeableObject in removeableObjects) {
            // remove object
            Destroy(removeableObject);

            this.walkingObjects.Remove(removeableObject);
        }
    }

    void OnCollisionEnter(Collision collisionInfo) {
        this.ProcessObjectIfRequired(collisionInfo.gameObject);
    }

    void OnTriggerEnter(Collider collidingObject) {
        this.ProcessObjectIfRequired(collidingObject.gameObject);
    }

    protected void ProcessObjectIfRequired(GameObject gameObjectToCheck) {
        // determine if we should remove the object from the scene
        string collidingObjectTag = gameObjectToCheck.tag;

        bool isSheep = collidingObjectTag == Tags.Sheep;

        bool shouldProcessDogs = LevelBehaviour.Instance.CanLevelBeCompleted;
        bool isDog = collidingObjectTag == Tags.Shepherd;

        if (isSheep || (shouldProcessDogs && isDog)) {
            this.OnObjectCollision(gameObjectToCheck);
        }

        if ((shouldProcessDogs && isDog)) {
            // disable dog specific scripts
            gameObjectToCheck.GetComponent<HerderLoopBehaviour>().CancelWalk();
            gameObjectToCheck.GetComponent<ControlHerderBehaviour>().enabled = false;
        }
    }

    protected void OnObjectCollision(GameObject collidingObject) {
        // execute sheep specific behaviour
        if (collidingObject.tag == Tags.Sheep) {
            LevelBehaviour.Instance.OnSheepCollected();

            var audioSource = this.GetComponent<BarnAudioController>();
            if (audioSource != null) {
                audioSource.SheepBarnEnterSound.Play();
            }
        }

        // don't destroy object: remove collider instead and let it move towards the endpoint
        var c = collidingObject.GetComponent<Collider>();
        if (c != null) {
            c.enabled = false;
        }

        var rb = collidingObject.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = true;
        }

        this.walkingObjects.Add(collidingObject);

        Vector3 target = this.endPointTarget.transform.position;
        target.y = collidingObject.transform.position.y;
        collidingObject.transform.LookAt(target);

        // call the level manager if it's a dog
        if (collidingObject.tag == Tags.Shepherd) {
            LevelBehaviour.Instance.OnDogBarnEntered();
        }
    }
}