using System;
using System.Collections;
using System.Globalization;
using UnityEngine;

// Cheats implementation: General Commands
public static partial class CheatImplementation {
    [CheatCommand("Help", CheatCategory.GeneralCommands)]
    public static void ShowCheatsHelpReference() {
        // show the dialog
        CheatReferenceDialog.ShowDialog("Cheats Reference",
                                         CheatService.GetAllCommandsByHumanReadableCategory(),
                                         CheatService.GetAllVariabeles(),
                                        "CheatDialogLabel");
    }

    [CheatCommand("RepeatCommand", CheatCategory.GeneralCommands)]
    public static void RepeatSpecifiedCommand(float timeDelay, int numberOfTimes, string command) {
        StartCoroutine(RepeatSpecifiedCommandExecutor(timeDelay, numberOfTimes, command));
    }

    private static IEnumerator RepeatSpecifiedCommandExecutor(float timeDelay, int numberOfTimes, string command) {
        while (numberOfTimes-- > 0) {
            CheatService.ExecuteRawCommand(command, false);

            yield return new WaitForSeconds(timeDelay);
        }

        yield break;
    }

    [CheatCommand("Enable", CheatCategory.GeneralCommands)]
    public static void EnableTheSpecifiedVariabele(string variabele) {
        SetAVariabeleToTheSpecifiedValue(variabele, true);
    }

    [CheatCommand("Disable", CheatCategory.GeneralCommands)]
    public static void DisableTheSpecifiedVariabele(string variabele) {
        SetAVariabeleToTheSpecifiedValue(variabele, false);
    }

    [CheatCommand("Set", CheatCategory.GeneralCommands)]
    public static void SetAVariabeleToTheSpecifiedValue(string variabele, object value) {
        // select member field
        CheatVariabeleDescriptor variabeleDescriptor = CheatService.GetCheatByVariabeleName(variabele);

        if (variabeleDescriptor == null) {
            CheatNotificationDialog.ShowDialog("Error",
                                           String.Format(
                                               "Variabele could not be set to '{1}': Variabele '{0}' does not exist.",
                                               variabele, value));
            return;
        }

        // convert value
        object parsedParameter;
        try {
            parsedParameter = Convert.ChangeType(value, variabeleDescriptor.FieldInfo.FieldType, CultureInfo.InvariantCulture);
        } catch (Exception) {
            CheatNotificationDialog.ShowDialog("Error", String.Format("Cheat could not be applied: Value '{0}' for variabele '{1}' could not be parsed as '{2}'", value, variabele, variabeleDescriptor.FieldInfo.FieldType.FullName));
            return;
        }

        variabeleDescriptor.FieldInfo.SetValue(null, parsedParameter);
    }
}