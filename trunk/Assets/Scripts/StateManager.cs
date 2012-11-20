using UnityEngine;
using System.Collections;

public class StateManager : MonoBehaviour {
    
	public static string STATE_IDLE = "s_idle";
	public static string STATE_FIGHTING = "s_fighting";
	public static string STATE_MOVING = "s_moving";
	
	private string state;
    private bool alive = true;
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

    public bool getHealth(){
        return alive;
    }
}
