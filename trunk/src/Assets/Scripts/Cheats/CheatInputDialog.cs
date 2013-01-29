using System;
using System.Globalization;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Helper class for showing a cheat dialog
/// </summary>
public static class CheatInputDialog {
    private static bool _ShowDialog;
    private static Rect _DialogRect;
    private static string _EnteredCheat = String.Empty;

    /// <summary>
    /// Enables showing of the dialog
    /// </summary>
    public static void ShowDialog() {
        _EnteredCheat = String.Empty;
        _ShowDialog = true;

        const int width = 300;
        const int height = 100;
        _DialogRect = new Rect(Screen.width / 2 - (width / 2), Screen.height / 2 - (height / 2), width, height);

        CheatReferenceDialog.HideDialog();
        CheatNotificationDialog.HideDialog();
    }

    /// <summary>
    /// Disables showing of the dialog
    /// </summary>
    public static void HideDialog() {
        _ShowDialog = false;

        CheatReferenceDialog.HideDialog();
        CheatNotificationDialog.HideDialog();
    }

    /// <summary>
    /// Draws the dialog. Call in <c>OnGUI</c>.
    /// </summary>
    /// <param name="skin"></param>
    public static void DrawDialog(GUISkin skin) {
        if (_ShowDialog) {
            _DialogRect = GUILayout.Window(0, _DialogRect, i => DrawInsideDialog(i, skin), "Cheats1337",
                                           skin.GetStyle("window"));
        }

        CheatReferenceDialog.DrawDialog(skin);
        CheatNotificationDialog.DrawDialog(skin);
    }

    private static void DrawInsideDialog(int dialogId, GUISkin skin) {

        // label
        GUILayout.BeginHorizontal();
        GUILayout.Label("Enter cheat: ", skin.GetStyle("label"));
        GUILayout.EndHorizontal();

        // text field
        GUILayout.BeginHorizontal();
        GUI.SetNextControlName("CheatInputBox");
        _EnteredCheat = GUILayout.TextField(_EnteredCheat, 60, skin.GetStyle("textfield")).TrimStart('`');
        GUILayout.EndHorizontal();

        bool cheatEnterKeyPressed = _EnteredCheat.Length > 0 &&
                                    (_EnteredCheat[_EnteredCheat.Length - 1] == '\n' ||
                                     _EnteredCheat[_EnteredCheat.Length - 1] == '\r');

        // button
        GUILayout.BeginHorizontal(GUILayout.Width(50));
        if (GUILayout.Button("Apply", skin.GetStyle("button")) || cheatEnterKeyPressed) {
            _ShowDialog = false;

            if (String.IsNullOrEmpty(_EnteredCheat)) {
                _EnteredCheat = String.Empty;
            }
            else {
                _EnteredCheat = _EnteredCheat.TrimEnd('\n', ' ', '\r');
                ApplyCheat(_EnteredCheat);
            }
        }
        GUILayout.EndHorizontal();

        // set focus to cheat input box
        if (GUI.GetNameOfFocusedControl() == String.Empty) {
            GUI.FocusControl("CheatInputBox");
        }

    }

    private static void ApplyCheat(string cheatText) {
        string[] arguments = cheatText.Split(' ');

        // select correct cheat member
        MethodInfo cheatMember = FindCheatMemberByCheatAttribute(arguments[0]);

        // check if cheat is found
        if (cheatMember == null) {
            CheatNotificationDialog.ShowDialog("Error", "Cheat could not be applied: cheat not found. Type 'help' for cheat reference.", "MonospaceLabel");
            return;
        }

        // select parameters
        ParameterInfo[] memberParams = cheatMember.GetParameters();
            
        if (memberParams.Length != arguments.Length - 1) {
            CheatNotificationDialog.ShowDialog("Error", String.Format("Cheat could not be applied: Expected {0} parameters, but got {1} parameters", memberParams.Length, arguments.Length-1) , "MonospaceLabel");
            return;
        }

        object[] parsedParameters = new object[memberParams.Length];
        for (int i = 0; i < memberParams.Length && i < parsedParameters.Length; i++) {
            ParameterInfo currentParam = memberParams[i];
            string rawArgument = arguments[i + 1];

            try {
                parsedParameters[i] = Convert.ChangeType(rawArgument, currentParam.ParameterType, CultureInfo.InvariantCulture);
            } catch (Exception) {
                CheatNotificationDialog.ShowDialog("Error", String.Format("Cheat could not be applied: Value '{0}' for parameter '{1}' could not be parsed as '{2}'", rawArgument, currentParam.Name, currentParam.ParameterType.FullName), "MonospaceLabel");
                return;
            }
        }

        // call
        cheatMember.Invoke(null, parsedParameters);

        Debug.Log("Applied cheat: " + cheatMember.Name);
    }

    private static MethodInfo FindCheatMemberByCheatAttribute(string cheatText) {
        MethodInfo[] cheatMembers = typeof (CheatsImplementation).GetMethods(BindingFlags.Public | BindingFlags.Static);

        MethodInfo cheatMember = null;
        foreach (MethodInfo member in cheatMembers) {
            // get cheat attr
            object[] attr = member.GetCustomAttributes(typeof (CheatAttribute), false);

            if (attr.Length == 0) {
                continue;
            }

            var cheatAttr = attr[0] as CheatAttribute;
            if (cheatAttr == null) {
                continue;
            }

            // compare
            if (String.Equals(cheatAttr.Name, cheatText, StringComparison.InvariantCultureIgnoreCase)) {
                cheatMember = member;
                break;
            }
        }

        return cheatMember;
    }
}