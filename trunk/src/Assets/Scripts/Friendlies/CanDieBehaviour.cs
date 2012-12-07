using UnityEngine;

/// <summary>
/// Base class for objects that can die.
/// </summary>
public abstract class CanDieBehaviour : MonoBehaviour {
    /// <summary>
    /// Causes the object to which this script is attached to die. Custom logic is implemented in derived classes.
    /// </summary>
    /// <param name="causeOfDeath"></param>
    public abstract void Die(GameObject causeOfDeath);
}
