using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class OnMouseOverFadeObjectBehaviour : MonoBehaviour {
    /// <summary>
    /// Gets or sets if all the child game objects should also be set translucent
    /// </summary>
    public bool RecursiveWalk = true;

    private bool processGameObjects = false;
    private bool currentlyMouseOver = false;
    private float alpha = 1f;

	public float alphaMin = 0.3f;
	public float alphaMax = 1f;
    public float FadeSpeed = 5f;

	/// <summary>
	/// Use this for initialization
	/// </summary>
	private void Start () {
	    
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () {
        if (!this.processGameObjects) {
            return;
        }

	    GameObject gameObjectToProcess = this.gameObject;

	    if (this.RecursiveWalk) {
	        //this.WalkAndProcessGameObject(gameObjectToProcess);
            //Debug.LogWarning("Recursive walk currently not implemented");
            this.processGameObjects = !this.ProcessGameObject(gameObjectToProcess);
        } else {
	        this.processGameObjects = !this.ProcessGameObject(gameObjectToProcess);
	    }
	}

    /// <summary>
    /// Recursively walk and process the graph of game objects.
    /// </summary>
    /// <remarks>
    /// We actually push everything on the stack because graphs of game objects may get very deep.
    /// </remarks>
    /// <param name="gameObjectToProcess"></param>
    private void WalkAndProcessGameObject (GameObject gameObjectToProcess) {
        Stack<GameObject> gameObjectsToProcess = new Stack<GameObject>();
        bool processingComplete = true;

        // seed the stack
        gameObjectsToProcess.Push(gameObjectToProcess);

        // walk each game object and process it
        while (gameObjectsToProcess.Count > 0) {
            // process the current object
            GameObject current = gameObjectsToProcess.Pop();
            processingComplete = processingComplete && this.ProcessGameObject(current);

            // add each one of the children to the list
            foreach (Transform childTransform in current.transform) {
                GameObject child = childTransform.gameObject;

                gameObjectsToProcess.Push(child);
            }
        }

        // turn off processing if we're good
        if (processingComplete) {
            this.processGameObjects = false;
        }
    }

    /// <summary>
    /// Processes the game objects alpha and returns a value indicating if the fade in / fade out has been completed
    /// </summary>
    /// <param name="gameObjectToProcess"></param>
    /// <returns></returns>
    private bool ProcessGameObject (GameObject gameObjectToProcess) {
        Collider currentCollider = gameObjectToProcess.GetComponent<Collider>();

        if (currentCollider == null) {
            return true;
        }

        Renderer currentRenderer = currentCollider.GetComponent<Renderer>();

        if (currentRenderer == null) {
            return true;
        }

        // determine if we're done
        // ... check what our actual target is
        bool targetIsUpper = Mathf.Abs(this.alpha - this.alphaMax) <= 0.1;
        bool targetIsLower = Mathf.Abs(this.alpha - this.alphaMin) <= 0.1;

        // ... check the difference to our target, example: if our target is high, check the difference between the current material color and the high target
        bool targetAlphaUpperReached = targetIsUpper && Mathf.Abs(currentRenderer.material.color.a - this.alphaMax) < 0.001;
        bool targetAlphaLowerReached = targetIsLower && Mathf.Abs(currentRenderer.material.color.a - this.alphaMin) < 0.001;
        if (targetAlphaUpperReached || targetAlphaLowerReached) {
            return true;
        }
		
		
		
        Color shadercolor = new Color(
                currentRenderer.material.color.r,
                currentRenderer.material.color.g,
                currentRenderer.material.color.b,
                Mathf.Lerp(currentRenderer.material.color.a, this.alpha, Time.deltaTime * FadeSpeed));
        currentRenderer.material.shader = Shader.Find("Transparent/Diffuse");
        currentRenderer.material.color = shadercolor;

        return false;
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
