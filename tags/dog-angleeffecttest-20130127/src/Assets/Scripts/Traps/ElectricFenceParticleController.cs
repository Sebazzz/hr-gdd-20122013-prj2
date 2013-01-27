using UnityEngine;

/// <summary>
/// Helper class for setting a random start delay for the electric fence
/// </summary>
public sealed class ElectricFenceParticleController : MonoBehaviour {

    public float MinStartDelay = 1f;

    public float MaxStartDelay = 8f;
    
    void Start() {
        // get our particle system
        ParticleSystem particleController = this.GetComponent<ParticleSystem>();

        if (particleController == null) {
            particleController = this.GetComponentInChildren<ParticleSystem>();
        }

        if (particleController == null) {
            throw new UnityException("This script should be attached to the particle system objects parent or object self");
        }

        // set a random 
        float delay = Random.Range(this.MinStartDelay, this.MaxStartDelay);
        particleController.startDelay = delay;
        //particleController.enableEmission
        particleController.Play();
    }

}