using UnityEngine;
using System.Collections;
using System;

public class LookatShepherdBehaviour : MonoBehaviour
{
	/// <summary>
	/// The last update.
	/// </summary>
	private float lastUpdate = 0.0F;
	/// <summary>
	/// The update delay.
	/// </summary>
	public  float UpdateDelay = 0.6F;
	/// <summary>
	/// The minimum watch distance.
	/// </summary>
	public float minWatchDistance = 20F;
	/// <summary>
	/// The previous position.
	/// </summary>
	public Quaternion prevPosition= new Quaternion();
	/// <summary>
	/// The follow speed.
	/// </summary>
	public int followSpeed = 20;
	/// <summary>
	/// The closestshepherd.
	/// </summary>
	public GameObject closestshepherd = null ;
	
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		if (Time.time - lastUpdate > UpdateDelay) {
			lastUpdate = Time.time;
			if (gameObject.GetComponent<SheepIdleBehaviour> ().sheepState.Equals (SheepIdleBehaviour.SheepState.active)) {
				FindClosestDog ();				
			} 
		}
		if (gameObject.GetComponent<SheepIdleBehaviour> ().sheepState.Equals (SheepIdleBehaviour.SheepState.active)) {
			LookAtClosestDog ();	
		} 
	}
	
	/// <summary>
	/// Finds the closest dog.
	/// </summary>
	void FindClosestDog ()
	{
		//FIND SHPEHERD
		GameObject[] shepherd = GameObject.FindGameObjectsWithTag (Tags.Shepherd);

		if (shepherd.Length == 0) {
			return ;
		}
	     
		//INITIATE COMPARE
		Vector3 vecmem = shepherd [0].transform.position - transform.position;
		prevPosition = transform.rotation;	
		for (int i = 0; i < shepherd.Length; i ++) {
			Vector3 vec = shepherd [i].transform.position - transform.position;
			if (vec.sqrMagnitude.CompareTo (vecmem.sqrMagnitude) < 1) { 
				vecmem = vec;
				closestshepherd = shepherd [i];
				}
		}
		
	}
	
	/// <summary>
	/// Looks at closest dog.
	/// </summary>
	 void LookAtClosestDog ()
	{	
		try{
		if (closestshepherd !=null) {
			print (gameObject.ToString());
			Vector3 distance = closestshepherd.transform.position - transform.position;
			print (distance.magnitude + " magnitude" + minWatchDistance + "min watchdist");
				
			if (distance.magnitude < minWatchDistance) {
				distance.y = 0;
				prevPosition= transform.rotation;		
				Quaternion rotation = Quaternion.LookRotation (distance);
				transform.rotation = Quaternion.Slerp (transform.rotation, rotation, Time.fixedDeltaTime * followSpeed);	
			 	
			}
			else{
			loosFocus();
			closestshepherd = null;	
			}
		}
		}
		catch(Exception e){
			print(e.Message);
			
		}
	}
	
	
	/// <summary>
	/// Looses the focus on the closest dogg.
	/// </summary>
	void loosFocus ()
	{
		transform.rotation = Quaternion.Slerp (transform.rotation, prevPosition, Time.fixedDeltaTime * followSpeed);	
	}
	

}
