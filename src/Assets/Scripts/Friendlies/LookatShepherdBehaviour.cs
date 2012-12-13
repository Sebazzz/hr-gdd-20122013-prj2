using UnityEngine;
using System.Collections;

public class LookatShepherdBehaviour : MonoBehaviour {
    private float currentTime;

    /// <summary>
    /// Defines the time difference for looking at sheep
    /// </summary>
    public float TimeDifference = 0.5f;

	// Use this for initialization
	void Start () {
	    this.currentTime = TimeDifference;
	}
	
	// Update is called once per frame
	void Update () {
	    this.currentTime -= Time.deltaTime;
        if (this.currentTime > 0) {
            return;
        }

	    this.currentTime = this.TimeDifference;
	    if(gameObject.GetComponent<SheepIdleBehaviour>().sheepState.Equals(SheepIdleBehaviour.SheepState.active)){
		    LookAtClosestDog();	
		}
	}

	void LookAtClosestDog(){
		//FIND SHPEHERD
	GameObject[] shepherd = GameObject.FindGameObjectsWithTag(Tags.Shepherd);

        if (shepherd.Length == 0) {
            return;
        }
	     
		//INIT COMPARE
		GameObject closestshepherd = shepherd[0];
		Vector3 vecmem = shepherd[0].transform.position -transform.position;
		for( int i = 0 ; i < shepherd.Length ; i ++ )
		{
			Vector3 vec =  shepherd[i].transform.position - transform.position;
			if(vec.sqrMagnitude.CompareTo(vecmem.sqrMagnitude) < 1  ){ 
				vecmem = vec;
			closestshepherd = shepherd[i];
			}
		}
		
		//LOOKAT POS
		transform.LookAt(closestshepherd.transform);

		///Debug.Log(" LOOKING AT "  + closestshepherd.ToString);

		//print(" LOOKING AT "  + closestshepherd.transform.position);

		
		
	}

}
