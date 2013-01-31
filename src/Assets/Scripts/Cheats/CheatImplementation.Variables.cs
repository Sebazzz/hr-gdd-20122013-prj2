using System;
using System.Globalization;
using System.Reflection;

// Cheats implementation: Functional
public static partial class CheatImplementation {
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