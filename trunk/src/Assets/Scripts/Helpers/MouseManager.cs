using System;
using UnityEngine;


/// <summary>
///     Helper class for acquiring a 'lock' on the mouse. Make sure to set the correct script execution order in the Script Execution Manager!
/// </summary>
public static class MouseManager {
    /// <summary>
    /// Defines mouse buttons
    /// </summary>
    public enum MouseButton {
        Left = 0,
        Right = 1
    }

    /// <summary>
    ///     Gets if the mouse is locked currently
    /// </summary>
    public static bool IsMouseLocked { get; private set; }

    /// <summary>
    ///     Gets the current lock owner of the mouse.
    /// </summary>
    /// <remarks>
    ///     This property is primarily for debugging purposes.
    /// </remarks>
    public static MonoBehaviour CurrentLockOwner { get; private set; }

    /// <summary>
    ///     Tries to get the lock on the mouse, returning a value indicating success.
    /// </summary>
    /// <param name="requestee">The current instance calling this script. Pass 'this' to this parameter.</param>
    /// <returns></returns>
    public static bool TryAcquireLock (MonoBehaviour requestee) {
        if (requestee == null) {
            throw new ArgumentNullException("requestee");
        }
        if (CurrentLockOwner == requestee) {
            return true;
        }

        if (IsMouseLocked) {
            return false;
        }

        IsMouseLocked = true;
        CurrentLockOwner = requestee;

        return true;
    }

    /// <summary>
    ///     Releases the lock on the mouse.
    /// </summary>
    /// <param name="requestee">Used for checking if the correct script releases the lock. Pass 'this' to this parameter.</param>
    public static void ReleaseLock (MonoBehaviour requestee) {
        if (requestee == null) {
            throw new ArgumentNullException("requestee");
        }

        if (!(requestee is LevelBehaviour) && !ReferenceEquals(CurrentLockOwner, requestee)) {
            throw new InvalidOperationException("Wrong owner requesting lock release");
        }

        IsMouseLocked = false;
        CurrentLockOwner = null;
    }

    /// <summary>
    ///     Tries to get the lock on the mouse, throwing an exception on failure. An failing version of
    ///     <see
    ///         cref="TryAcquireLock" />
    ///     .
    /// </summary>
    /// <param name="requestee">The current instance calling this script. Pass 'this' to this parameter.</param>
    public static void AcquireLock (MonoBehaviour requestee) {
        if (CurrentLockOwner == requestee) {
            return;
        }
        if (IsMouseLocked) {
            throw new InvalidOperationException("Lock is already acquired by " + CurrentLockOwner.GetType());
        }

        IsMouseLocked = true;
        CurrentLockOwner = requestee;
    }
}