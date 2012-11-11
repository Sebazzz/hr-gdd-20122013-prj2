using UnityEngine;
using System.Collections;

public class ActionWalk : Action {
	public Vector3 target;
	
	public ActionWalk(Vector3 target){
		this.type = ActionType.ACTION_WALK;
		this.target = target;
	}
	
	override public void Update () {
		gameObject.transform.LookAt(target);
		gameObject.transform.Translate(0, 0, 5f * Time.deltaTime);
		
		float dist = Vector3.Distance(gameObject.transform.position, target);
		if(dist < 2){
			this.done = true;	
		}
	}
}
