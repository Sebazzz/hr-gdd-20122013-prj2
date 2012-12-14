using UnityEngine;
using System.Collections;

/// <summary>
/// This script has the simple purpose to check for collisions with elements with the <see cref="JumpZoneConfiguration"/> script attached. 
/// These object
/// </summary>
/// <dependency cref="JumpZoneConfiguration" />
public class AutoJumpBehaviour : MonoBehaviour
{
	/// <summary>
	/// Defines how much the force applied as configured in <see cref="JumpZoneConfiguration.ForceUp"/> is multiplied for this object
	/// </summary>
	public float JumpUpModifier = 1f;
	
	/// <summary>
	/// The min angle range the RayCast should collide(hit) with this gameobject.
	/// </summary>
	public float angleRange = 45.0f;
	
	/// <summary>
	/// Defines how much the force applied as configured in <see cref="JumpZoneConfiguration.ForceForward"/> is multiplied for this object
	/// </summary>
	public float JumpForwardModifier = 1f;

	/// <summary>
	/// Defines how close an object must be for jumping to be triggered
	/// </summary>
	public float DetectRange = 2f;

	/// <summary>
	/// Defines the allowed rotation deviation ("afwijking") in degrees for the object to be able to jump
	/// </summary>
	public float AllowedRotationDeviation = 45;

	/// <summary>
	/// Defines if the jump is only triggered when the object is outside the jump zone and the conditions are met.
	/// For example, if this property is set to <c>false</c>, the character will jump once the conditions are met inside the jump zone.
	/// </summary>
	public bool JumpTriggerOnlyOutsideZone = true;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		this.DetectJump ();
	}

	void FixedUpdate ()
	{
        
	}

	private void DetectJump ()
	{
		// detect any obstacle, the first hit will be in the 'hit' var
		Ray jumpZoneDetectionRay = new Ray (this.transform.position, this.transform.TransformDirection (Vector3.forward));

		RaycastHit hit;
		if (!Physics.Raycast (jumpZoneDetectionRay, out hit, this.DetectRange)) {
			return;
		}
	

		// determine if we have information for jumping (i.e.: the other is a jumpzone)
		GameObject other = hit.collider.gameObject;
		JumpZoneConfiguration jumpZoneConfiguration = other.GetComponent<JumpZoneConfiguration> ();

		if (jumpZoneConfiguration == null) {
			return;
		}

		// check if we meet the conditions for a jump
		// ... check y position, if we're high enough we don't need to add extra force
		if ((this.transform.position.y - other.transform.position.y) > 0.5) {
			return;
		}

		// ... check for rotation
		// TODO: don't know how
		Vector2 ray2D = new Vector2 (jumpZoneDetectionRay.direction.x, jumpZoneDetectionRay.direction.z);
		Vector2 jumpzone2D = new Vector2 (jumpZoneConfiguration.transform.forward.x, jumpZoneConfiguration.transform.forward.z);
		
		
		float angle = Vector2.Angle (ray2D, jumpzone2D);
				
		///if( (angle < 90 - range || angle >  270.0F + range )||( angle > 90.0F + range  && angle < 270.0F - range ) ){
		
		if (angle < 90.0F - angleRange) {
			this.ExecuteJump (jumpZoneConfiguration);	
			Debug.Log (" back hit" + angle  +"<90-range");
		} else if ( angle > 180 -  angleRange ) {
			this.ExecuteJump (jumpZoneConfiguration);	
			Debug.Log (" front hit" + angle +"> 90-range");
		} else {
			Debug.Log (" no good hit" + angle );
			return;
		}
		 
			
		
		/// Vector3 v = this.rigidbody.GetPointVelocity(other.transform.position);
		///   Debug.Log(v);	
		///this.ExecuteJump(jumpZoneConfiguration);
	}

	private void ExecuteJump (JumpZoneConfiguration configuration)
	{
		float finalForceUp = configuration.ForceUp * this.JumpUpModifier;
		float finalForceForward = configuration.ForceForward * this.JumpForwardModifier;

		this.rigidbody.AddForce (Vector3.up * finalForceUp, ForceMode.VelocityChange);
		this.rigidbody.AddForce (Vector3.forward * finalForceForward, ForceMode.VelocityChange);
	}
}
