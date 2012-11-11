using UnityEngine;
using System.Collections;

public abstract class Action {
	public string type;
	public GameObject gameObject;
	public bool done = false;
	
	public void setGameObject(GameObject gameObject){
		this.gameObject = gameObject;
	}
	
	virtual public void Update () {	}
	
	public static Action getNewAction(Action action){
		Action a = null;
		
		if(action.type == ActionType.ACTION_WALK){
			ActionWalk b = (ActionWalk)action;
			a = new ActionWalk(b.target);
		}
	
		a.setGameObject(action.gameObject);

		return a;
	}
}


public class ActionType {
	public const string ACTION_WALK = "walk";
	public const string ACTION_BUILDING = "building";
}
