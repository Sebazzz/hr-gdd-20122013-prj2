using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This script applies drag according to the texture that is underneath.
/// UpdateInterval slows down this proces so it doenst put a large dent in performance.
/// </summary>

public class ApplyPhysicsBehaviour : MonoBehaviour {
    public float updateInterval = 0.5f; // Interval in seconds

    // table of textures and their respective PhysicsMaterials
    private readonly Dictionary<int, PhysicsProperties> textures = new Dictionary<int, PhysicsProperties>();

	void Start () {
	    // Set textures
        textures.Add(0, new PhysicsProperties("GrassPhysics"));
        textures.Add(1, new PhysicsProperties("IcePhysics", 0));
        textures.Add(2, new PhysicsProperties("SnowPhysics", 0.5f));
	}
	
	void FixedUpdate () {
        
        if (Time.time % updateInterval == 0) {
            var surfaceIndex = TerrainSurface.GetMainTexture(transform.position);
            if(textures.ContainsKey(surfaceIndex)){
                textures[surfaceIndex].apply(this.gameObject);
            }
        }
	}
}

public class PhysicsProperties {
    private PhysicMaterial material;
    private float drag;

    public PhysicsProperties(string MaterialPath) : this(MaterialPath, 1) {}
    public PhysicsProperties(string MaterialPath, float drag) {
        this.drag = drag;
        this.material = (PhysicMaterial)Resources.Load(MaterialPath);

        if (CheatsController.TerrainBounce) {
            this.material.bounciness = 1;
        }
    }

    public void apply(GameObject gameObject){
        gameObject.collider.material = this.material;
        gameObject.rigidbody.drag = drag;
    }

}