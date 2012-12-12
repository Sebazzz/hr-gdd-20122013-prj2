using UnityEngine;
using System.Collections;

public class JumpableAbility : MonoBehaviour
{
	
	/// <summary>
	/// Raises the trigger enter event.
	/// </summary>
	/// <param name='other'>
	/// Other.
	/// </param>
	void OnTriggerEnter (Collider other)
	{
		
			 rigidbody.AddForce(Vector3.up * 2000, ForceMode.Impulse);
	}
	
	

	
}
