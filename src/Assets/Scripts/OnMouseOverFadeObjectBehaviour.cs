using UnityEngine;
using System.Collections;

public class OnMouseOverFadeObjectBehaviour : MonoBehaviour {

	private Renderer renderer;
	private float alpha = 1f;
	public float alphaMin = 0.3f;
	public float alphaMax = 1f;

	/// <summary>
	/// Use this for initialization
	/// </summary>
	private void Start () {
	
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () {

		renderer = gameObject.collider.renderer;

		if ((renderer.material.color.a >= alpha && alpha == alphaMax) ||
			(renderer.material.color.a <= alpha && alpha == alphaMin)) return;

		if (renderer) {
			Color shadercolor = new Color(
				renderer.material.color.r,
				renderer.material.color.g,
				renderer.material.color.b,
				Mathf.Lerp(renderer.material.color.a, alpha, Time.deltaTime*5f));
			renderer.material.shader = Shader.Find("Transparent/Diffuse");
			renderer.material.color = shadercolor;
		}
	}

	/// <summary>
	/// On Mouse Over Collider of the game object
	/// </summary>
	void OnMouseOver() {
		alpha = alphaMin;
	}

	void OnMouseExit() {
		alpha = alphaMax;
	}
}
