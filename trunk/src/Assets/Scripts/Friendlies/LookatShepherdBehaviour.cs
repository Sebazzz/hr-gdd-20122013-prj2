using UnityEngine;
using System.Collections;

public class LookatShepherdBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	if(gameObject.GetComponent<SheepIdleBehaviour>().sheepState.Equals(SheepIdleBehaviour.SheepState.active)){
		LookAtClosestDog();	
		}
			
	
	}

	void LookAtClosestDog(){
		///FIND SHPEHERD
	GameObject[] shepherd = GameObject.FindGameObjectsWithTag(Tags.Shepherd);
	     
		///INIT COMPARE
		Vector3 vecmem = shepherd[0].transform.position -transform.position;
		for( int i = 0 ; i < shepherd.Length ; i ++ )
		{
			Vector3 vec =  shepherd[i].transform.position - transform.position;
			if(vec.magnitude.CompareTo(vecmem.magnitude) < 1  ){ vecmem = vec;}
		}
		
		///LOOKAT POS
		transform.LookAt(vecmem);
		
		
	}

}
