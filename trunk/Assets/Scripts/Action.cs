using UnityEngine;
using System.Collections;

public abstract class Action {
	public ActionType type;
	public GameObject gameObject;
	public bool done = false;
	
	public void setGameObject(GameObject gameObject){
		this.gameObject = gameObject;
	}
	
	virtual public void Update () {	}
	
	public static Action getNewAction(Action action){
		Action a = null;
		
		if(action.type == ActionType.Walk){
			ActionWalk b = (ActionWalk)action;
			a = new ActionWalk(b.target);
		}
	
		a.setGameObject(action.gameObject);

		return a;
	}
}


public enum ActionType {
	Walk,
	Building,
}
