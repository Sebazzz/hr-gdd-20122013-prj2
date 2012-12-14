using UnityEngine;
using System.Collections;

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
	public float minWatchDistance = 4.0F;
	
	public float prevangle ;
	/// <summary>
	/// The follow speed.
	/// </summary>
	public int followSpeed =20;
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
				closestshepherd  = FindClosestDog ();	
				if(closestshepherd != null) {
				prevangle = transform.rotation.y;	
				}
			} 
		}
		if (gameObject.GetComponent<SheepIdleBehaviour> ().sheepState.Equals (SheepIdleBehaviour.SheepState.active)) {
			LookAtClosestDog (closestshepherd );	
		} 
	}

	GameObject FindClosestDog ()
	{
		//FIND SHPEHERD
		GameObject[] shepherd = GameObject.FindGameObjectsWithTag (Tags.Shepherd);

		if (shepherd.Length == 0) {
			return null;
		}
	     
		//INIT COMPARE
		closestshepherd = shepherd [0];
		Vector3 vecmem = shepherd [0].transform.position - transform.position;
		for (int i = 0; i < shepherd.Length; i ++) {
			Vector3 vec = shepherd [i].transform.position - transform.position;
			if (vec.sqrMagnitude.CompareTo (vecmem.sqrMagnitude) < 1) { 
				vecmem = vec;
				closestshepherd = shepherd [i];
			}
		}
		return closestshepherd;
	}
	
	/// <summary>
	/// Looks at closest dog.
	/// </summary>
	/// <param name='LookAt'>
	/// Look at : The transform to look at
	/// .
	/// </param>
	public void LookAtClosestDog (GameObject LookAt)
	{		
		if (Vector3.Distance(LookAt.transform.position,transform.position) < minWatchDistance ) {
			Vector3 lookPos = closestshepherd.transform.position - transform.position;
			lookPos.y = 0;
			Quaternion rotation = Quaternion.LookRotation (lookPos);
			transform.rotation = Quaternion.Slerp (transform.rotation, rotation, Time.fixedDeltaTime * followSpeed);		
		}
	}
	
	public void loosFocus(){
	
		transform.Rotate(Vector3.up, pre
			
			
		
	}
	

}
