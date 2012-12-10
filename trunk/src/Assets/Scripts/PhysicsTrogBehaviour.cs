using UnityEngine;
using System.Collections;
/// <summary>
/// Attracts sheep to the trog
/// </summary>
public class PhysicsTrogBehaviour : MonoBehaviour
{

	/// <summary>
	/// The magnetic Strength is set to define how far the magnet is effective.
	/// </summary>
	public int MagneticStrength = 10;
	
	
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
			if (direction.sqrMagnitude <= (this.MagneticStrength * this.MagneticStrength)) {
				other.rigidbody.AddForce (direction.normalized * this.MagneticStrength);
                other.transform.LookAt(this.transform);

			}

			
		}
	}
	
}

