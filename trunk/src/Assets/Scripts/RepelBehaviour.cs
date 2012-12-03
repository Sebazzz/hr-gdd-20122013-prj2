using UnityEngine;
using System.Collections;

public class RepelBehaviour : MonoBehaviour {

 	public float radius = 10.0F;
    public float power = 50.0F;
    
	
	void Update() {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider hit in colliders) {
            if (hit.tag == "sheep")
                hit.rigidbody.AddExplosionForce(power, explosionPos, radius, 0);
            
        }
    }
}