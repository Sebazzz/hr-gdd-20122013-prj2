using UnityEngine;
using System.Collections;

public class ActionBuilding : Action {
	GameObject target;
	
	public ActionBuilding(GameObject target){
		this.type = ActionType.Building;
		this.target = target;
	}
	
	override public void Update () {
		this.gameObject.renderer.enabled = false;
	}
}
