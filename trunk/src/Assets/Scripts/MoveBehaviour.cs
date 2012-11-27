using UnityEngine;
using System.Collections;

public class MoveBehaviour : MonoBehaviour {
    /// <summary>
    /// Target of the movement. Only the <see cref="Vector3.x"/> and <see cref="Vector3.z"/> components will be used.
    /// </summary>
    public Vector3 Target;

    public float Speed = 5;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    float distance = Mathf.Abs(Vector3.Distance(this.transform.position, Target));

        if (distance > 1) {
	        // move somewhat
	        float xdiff = CalculateSpeed(this.Target.x, this.transform.position.x, this.Speed) * Time.deltaTime;
	        float zdiff = CalculateSpeed(this.Target.z, this.transform.position.z, this.Speed) * Time.deltaTime;

	        this.transform.Translate(new Vector3(xdiff, 0, zdiff));
	    } else {
	        this.enabled = false;
	    }
	}

    /// <summary>
    /// Enables the script for movement towards the specified target. Only the <see cref="Vector3.x"/> and <see cref="Vector3.z"/> components will be used.
    /// </summary>
    /// <param name="target"></param>
    public void MoveTo (Vector3 target) {
        this.enabled = true;
        this.Target = target;
    }

    private float CalculateSpeed(float target, float current, float speed) {
        float calcSpeed = Mathf.Abs(target - current) < speed ? Mathf.Abs(target - current) : speed;

        if (target < current) {
            calcSpeed *= -1f;
        }

        return calcSpeed;
    }
}
