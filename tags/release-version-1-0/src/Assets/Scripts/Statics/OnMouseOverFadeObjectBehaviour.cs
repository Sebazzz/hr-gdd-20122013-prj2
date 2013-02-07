using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class OnMouseOverFadeObjectBehaviour : MonoBehaviour {
    private bool processGameObjects = false;
    private bool currentlyMouseOver = false;
    private float alpha = 1f;

	public float alphaMin = 0.3f;
	public float alphaMax = 1f;
    public float FadeSpeed = 5f;

    private List<Material> materials;

	/// <summary>
	/// Use this for initialization
	/// </summary>
	private void Start () {
	    this.materials = GetMaterialsOfObject(this.gameObject);
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () {
        if (!this.processGameObjects) {
            return;
        }

	    int reachedCount = 0;
	    foreach (Material material in materials) {
            // determine if we're done
            // ... check what our actual target is
            bool targetIsUpper = Mathf.Abs(this.alpha - this.alphaMax) <= 0.1;
            bool targetIsLower = Mathf.Abs(this.alpha - this.alphaMin) <= 0.1;

            // ... check the difference to our target, example: if our target is high, check the difference between the current material color and the high target
            bool targetAlphaUpperReached = targetIsUpper && Mathf.Abs(material.color.a - this.alphaMax) < 0.001;
            bool targetAlphaLowerReached = targetIsLower && Mathf.Abs(material.color.a - this.alphaMin) < 0.001;
            if (targetAlphaUpperReached || targetAlphaLowerReached) {
                reachedCount++;
                continue;
            }

            Color shadercolor = new Color(
                    material.color.r,
                    material.color.g,
                    material.color.b,
                    Mathf.Lerp(material.color.a, this.alpha, Time.deltaTime * FadeSpeed));

            material.shader = Shader.Find("Transparent/Diffuse");
            material.color = shadercolor;
	    }

	    this.processGameObjects = reachedCount != this.materials.Count;
	}

    private static List<Material> GetMaterialsOfObject(GameObject targetGameObject) {
        List<Material> materials = new List<Material>();

        Stack<GameObject> gameObjects = new Stack<GameObject>();
        gameObjects.Push(targetGameObject);

        while (gameObjects.Count > 0) {
            GameObject current = gameObjects.Pop();

            // disable emitter
            MeshRenderer renderer = current.GetComponent<MeshRenderer>();
            if (renderer != null) {
                materials.AddRange(renderer.materials);
            }

            // search for additional
            foreach (Transform childTransform in current.transform) {
                GameObject child = childTransform.gameObject;

                gameObjects.Push(child);
            }
        }

        return materials;
    }

    /// <summary>
	/// On Mouse Over Collider of the game object
	/// </summary>
	void OnMouseOver() {
        if (this.currentlyMouseOver) {
            return;
        }

		this.alpha = this.alphaMin;
        this.processGameObjects = true;
        this.currentlyMouseOver = true;
    }

	void OnMouseExit() {
		this.alpha = this.alphaMax;
	    this.processGameObjects = true;
	    this.currentlyMouseOver = false;
	}
}
