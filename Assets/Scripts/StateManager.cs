using UnityEngine;
using System.Collections;

public class StateManager : MonoBehaviour {
	public static string STATE_IDLE;
	public static string STATE_FIGHTING;
	public static string STATE_MOVING;
	
	private string state;
	// Use this for initialization
	void Start () {
		this.state = StateManager.STATE_IDLE;
	}
	
	public void setState(string state){
		this.state = state;
	}
	
	public string getState(){
		return this.state;
	}
}
