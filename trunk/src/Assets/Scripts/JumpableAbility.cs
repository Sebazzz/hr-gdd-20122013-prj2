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
		if(other.tag == Tags.Shepherd){
			 Vector3 targ = other.gameObject.GetComponent<HerderLoopBehaviour>().target;
			 targ -= other.transform.position ;		
			 targ = targ.normalized;
			targ.y += transform.lossyScale.y * 3f ;
			 other.rigidbody.AddForce(targ * 300, ForceMode.Impulse);
	}
		else{
		}
	}
	

	
}
