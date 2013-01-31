using System.Collections;
using UnityEngine;

/// <summary>
/// Cheat implementation class. These public static members are called via reflection only.
/// </summary>
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
                                           "You're not currently in a level and the command cannot be executed therefore.",
                                           "MonospaceLabel");
        return null;
    }

    #endregion
}