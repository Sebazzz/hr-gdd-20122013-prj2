using UnityEngine;
using System.Collections;

public class PhysicsTrog : MonoBehaviour
{



	/// <summary>
	/// The magnetic Strength is set to define how far the magnet is effective.
	/// </summary>
	public int magneticStrength = 10;
	
	
	/// <summary>
	/// Raises the trigger stay event.
	/// </summary>
	/// <param name='other'>
	/// Other.
	/// </param>
	void OnTriggerStay (Collider other)
	{
		if (other.gameObject.tag.Equals (Tags.Sheep)) {	
			Vector3 direction = transform.position - other.transform.position;
			if (direction.sqrMagnitude <= (magneticStrength * magneticStrength)) {
				other.rigidbody.AddForce (direction.normalized * magneticStrength);
			} 
			
		}
	}
	
}

