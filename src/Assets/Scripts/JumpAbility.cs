using UnityEngine;
using System.Collections;

public class JumpAbility : MonoBehaviour
{

	void OnCollisionEnter (Collision collision)
	{
		 if(collision.gameObject.GetComponent<JumpedoverAbility>()!= null){
		this.rigidbody.AddForce(0,10,0);
		}
	}
}
