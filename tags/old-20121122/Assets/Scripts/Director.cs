using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents the master mind of the enemies and makes sure enemies are controlled towards the friendlies
/// </summary>
public class Director : ScriptableObject {
    private List<GameObject> spawnPoints;
    private List<GameObject> enemies;
    private List<GameObject> friendlies;
    
    public Director() {
        spawnPoints = new List<GameObject>();
        enemies = new List<GameObject>();
        friendlies = new List<GameObject>();
    }
	
    void Start () {
        // discover all our existing enemies and friendlies
        RegisterInitializedByTag(Tags.Enemy, this.enemies);
        RegisterInitializedByTag(Tags.Friendly, this.friendlies);
        RegisterInitializedByTag(Tags.Spawnpoint, this.spawnPoints);
        
        // TODO: we might want to do this in the state manager of the object itself, if applicable
    }
    
    void Update() {
        // TODO: we might want to do something here like activating enemies at spawn points but the algorithm has not been specified yet
    }
    
    private void RegisterInitializedByTag(string tag, List<GameObject> lst) {
        lst.AddRange(GameObject.FindGameObjectsWithTag(tag));
    }
    
    /// <summary>
    /// Register the specified object for usage by the director.
    /// </summary>
    /// <param name='obj'>
    /// Object.
    /// </param>
    /// <exception cref='UnityException'>
    /// Is thrown when an game object with unsupported tag is passed
    /// </exception>
    public void Register(GameObject obj) {
        List<GameObject> lst = null;
        
        if (obj.tag == Tags.Enemy) {
            lst = this.enemies;
        } else if (obj.tag == Tags.Friendly) {
            lst = this.friendlies;
        } else if (obj.tag == Tags.Spawnpoint) {
            lst = this.spawnPoints;
        } else {
            throw new UnityException("GameObject with unknown tag passed: " + obj.tag);
        }
        
        lst.Add(obj);
    }
    
    /// <summary>
    /// Unregister the specified object from the director after death.
    /// </summary>
    /// <param name='obj'>
    /// Object.
    /// </param>
    /// <exception cref='UnityException'>
    /// Is thrown when an game object with unsupported tag is passed
    /// </exception>
    public void Unregister(GameObject obj) {
        List<GameObject> lst = null;
        
        if (obj.tag == Tags.Enemy) {
            lst = this.enemies;
        } else if (obj.tag == Tags.Friendly) {
            lst = this.friendlies;
        } else {
            throw new UnityException("GameObject with unknown tag passed: " + obj.tag);
        }
        
        lst.Remove(obj);
    }
    
	
	private static Director _instance;
	public static Director Instance {
		get {
			if(_instance == null){
				_instance = ScriptableObject.CreateInstance<Director>();	
			}
			return _instance;
		}
	}
}
