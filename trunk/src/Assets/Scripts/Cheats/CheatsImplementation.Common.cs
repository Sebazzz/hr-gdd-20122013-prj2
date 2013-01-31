﻿using System.Collections;
using UnityEngine;

/// <summary>
/// Cheat implementation class. These public static members are called via reflection only.
/// </summary>
/// <remarks>
/// <para>
/// The guide to writing cheat commands:
///   <para>
///     The cheats are just simple public static methods decorated with an <see cref="CheatAttribute"/>. The cheats
///     are called with the command name specified by the attribute <see cref="CheatAttribute.Name"/>. The cheats
///     can accept parameters but only in the form of simple types (<c>int, long, double, float, string</c>). Overloading
///     of the cheat methods and thus supplying default parameters is not supported.
///   </para>
///   <para>
///     Documentation of the cheats is generated by splitting the pascalcased words of the method name, lowering
///     them and the upper the first letter. "ForExampleThisIsAMethod" is turned in "For example this is a method".
///     The attribute <see cref="CheatAttribute.Name"/> and the names of the parameters are left as-is.
///   </para>
/// </para>
/// 
/// <para>
/// The guide to writing variabeles:
///   <para>
///     The actual variabeles are contained in the <see cref="CheatsController"/> class and take effect there, usually
///     after a level reload. The actual documentation and registration of the variabeles is done using the static contructor
///     of this class.
///   </para>
/// </para>
/// </remarks>
public static partial class CheatsImplementation {
    #region Support methods

    private static bool StartCoroutine(IEnumerator coroutineResult) {
        CheatsController controller = AssertGetCheatsController();

        if (controller == null) {
            return false;
        }

        controller.StartCoroutine(coroutineResult);
        return true;
    }

    private static CheatsController AssertGetCheatsController() {
        GameObject worldObject = GameObject.FindGameObjectWithTag(Tags.World);

        if (worldObject != null) {
            var ctrl = worldObject.GetComponent<CheatsController>();

            if (ctrl != null) {
                return ctrl;
            }
        }

        CheatNotificationDialog.ShowDialog("Error",
                                           "You're not currently in a level and the command cannot be executed therefore.");
        return null;
    }

    #endregion
}