using UnityEngine;
using System.Collections.Generic;

public class ActionBehaviour : MonoBehaviour {
	private Queue<Action> actions;
	private Action currentAction;

	void Start () {
		actions = new Queue<Action>();
	}
	
	void Update () {
		if(actions.Count > 0){
			if(currentAction == null){
				currentAction = actions.Dequeue();
			}
		}
		
		if(currentAction != null){
			currentAction.Update();	
			if(currentAction.done){
				currentAction = null;
			}
		}
	}
	
	public void addAction(Action action){
		actions.Enqueue(action);
	}
}
