using UnityEngine;
using System.Collections;


public class ObjectManager : ScriptableObject {
	private ArrayList selectedObjects = new ArrayList();
	
	public void addObject(GameObject gameObject){
		selectedObjects.Add(gameObject);
	}
	
	public void removeObject(GameObject gameObject){
		selectedObjects.Remove(gameObject);
	}
	
	public void action(Action action){
		foreach(GameObject o in selectedObjects){
			ActionBehaviour ap = (ActionBehaviour)o.GetComponent("ActionBehaviour");
			if(ap != null){
				action.setGameObject(o);
				ap.addAction(action);
			}
		}
	}
	
	
	
    public static ObjectManager _instance;
	public static ObjectManager getInstance(){
		if(_instance == null){
			_instance = new ObjectManager();	
		}
		return _instance;
	}
	
}
