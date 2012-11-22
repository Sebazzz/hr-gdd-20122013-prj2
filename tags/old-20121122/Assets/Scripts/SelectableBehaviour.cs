using UnityEngine;
using System.Collections;

public class SelectableBehaviour : MonoBehaviour {
	/* Selecting part */
	private ObjectManager objectManager;
	private bool selected;
	
	public ActionType MyAction;
	
	void Start () {
		selected = false;
		this.objectManager = ObjectManager.getInstance();
	}
	
	void OnMouseDown(){
		if(selected){
			deselect();
		}else{
			select();
		}
	}
	
	void deselect(){
		this.selected = false;
		objectManager.removeObject(this.gameObject);
	}
	
	void select(){
		this.selected = true;
		objectManager.addObject(this.gameObject);
	}
}
