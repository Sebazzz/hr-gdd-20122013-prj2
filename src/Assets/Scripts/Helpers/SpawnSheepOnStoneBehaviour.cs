using UnityEngine;
using System.Collections;

public class SpawnSheepOnStoneBehaviour : MonoBehaviour {

	// The object that you want to spawn on the stone
	public GameObject ObjectToSpawn;

	/// <summary>
	/// Use this for initialization
	/// </summary>
	private void Start () {
		GameObject instance = (GameObject)Instantiate(
			ObjectToSpawn,
			new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z),
			transform.rotation);

	    instance.transform.parent = GroupObjects.SheepGroupObject.transform;

	    SheepIdleBehaviour idleBehaviour = instance.GetComponent<SheepIdleBehaviour>();
        if (idleBehaviour != null) {
            idleBehaviour.sheepState = SheepIdleBehaviour.SheepState.inactive;
        }
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	private void Update () {
	
	}
}
