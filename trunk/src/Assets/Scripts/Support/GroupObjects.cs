using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Service class to get grouping objects: empty game objects to put other objects in as some kind of grouping. This makes run-time designer usage easier.
/// </summary>
static class GroupObjects
{
    /// <summary>
    /// Gets the object used for grouping sheep in
    /// </summary>
    public static GameObject SheepGroupObject {
        get {
            const string objectName = Globals.SheepRootGameObjectName;

            var existingObject = GetOrCreateGroupObject(objectName);

            return existingObject;
        }
    }

    /// <summary>
    /// Gets the object used for grouping enemies in
    /// </summary>
    public static GameObject EnemyGroupObject {
        get {
            const string objectName = Globals.EnemyRootGameObjectName;

            var existingObject = GetOrCreateGroupObject(objectName);

            return existingObject;
        }
    }

    private static GameObject GetOrCreateGroupObject (string objectName) {
        // find any root group object
        GameObject existingObject = GameObject.Find("/" + objectName);

        if (existingObject == null) {
            existingObject = new GameObject(objectName);
        }

        return existingObject;
    }
}