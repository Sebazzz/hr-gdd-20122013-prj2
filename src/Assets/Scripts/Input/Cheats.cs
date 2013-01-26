using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Class that contains all cheats implementation and provides a basic cheat dialog implementation
/// </summary>
public static class Cheats {
    #region Cheats Implementation
    // ReSharper disable UnusedMember.Local
    /// <summary>
    /// Cheat implementation class
    /// </summary>
    private static class Impl {
        private static readonly List<CheatVar<bool>> CheatVars = new List<CheatVar<bool>>();

        static Impl() {
            CheatVars.Add(new CheatVar<bool>("supersheep", "Enlarge all sheeps in the next levels 4 times", v => CheatsController.EnableLargeSheep = v));
            CheatVars.Add(new CheatVar<bool>("gamecheats", "Show the in-game cheat button and menu", v => CheatsController.EnableInGameCheatsMenu = v));
        }

        [Cheat("Help")]
        public static void ShowCheatsHelpReference() {
            const string indent = "    ";

            List<string> cheatName = new List<string>();
            List<string> cheatDescription = new List<string>();

            // title for general cheats
            cheatName.Add("General commands:");
            cheatDescription.Add(null);

            // ... aggregate all general cheats and format them nicely
            MethodInfo[] cheatMembers = typeof(Impl).GetMethods(BindingFlags.Public | BindingFlags.Static);

            foreach (MethodInfo member in cheatMembers) {
                // get cheat attr
                object[] attr = member.GetCustomAttributes(typeof(CheatAttribute), false);

                if (attr.Length == 0) {
                    continue;
                }

                var cheatAttr = attr[0] as CheatAttribute;
                if (cheatAttr == null) {
                    continue;
                }

                // format
                string memberName = member.Name;

                string[] words = Regex.Split(memberName, "([A-Z][a-z]+)", RegexOptions.None);

                StringBuilder format = new StringBuilder();
                foreach (string word in words) {
                    if (format.Length == 0) {
                        format.Append(word);
                    } else {
                        format.Append(" ");
                        format.Append(word.ToLowerInvariant());
                    }
                }

                cheatName.Add(indent + cheatAttr.Name);
                cheatDescription.Add(format.ToString());
            }

            cheatName.Add(indent);
            cheatDescription.Add(indent);

            // title for enable/disable commands
            cheatName.Add("Variabeles to enable/disable:");
            cheatDescription.Add(null);

            // ... aggregate any enable/disable vars
            foreach (CheatVar<bool> cheatVar in CheatVars) {
                cheatName.Add(indent + cheatVar.Name);
                cheatDescription.Add(cheatVar.Description);
            }

            // show the dialog
            GameCheatReferenceDialog.ShowDialog("Cheats Reference", cheatName.ToArray(), cheatDescription.ToArray(), "MonospaceLabel");
        }

        [Cheat("PlayTheGround")]
        public static void LoadDeveloperSandboxLevel() {
            AsyncSceneLoader.Load(Scenes.Playground);
        }

        [Cheat("ClearSettings")]
        public static void ClearAllSettings() {
            PlayerPrefs.DeleteAll();

            AsyncSceneLoader.Load(Scenes.MainMenu);
        }

        [Cheat("BeTheBest")]
        public static void SetAllLevelsFullyPlayed() {
            Level currentLevel = Levels.GetFirstLevel();

            while (currentLevel != Level.None) {
                currentLevel.SetFinished();

                currentLevel = Levels.GetNextLevel(currentLevel);
            }

            AsyncSceneLoader.Load(Scenes.MainMenu);
        }

        [Cheat("enable/disable")]
        public static void EnableOrDisableVariabeles(bool enable, string name) {
            // select member of cheat controller
            foreach (var cheatVar in CheatVars) {
                if (String.Equals(cheatVar.Name, name, StringComparison.InvariantCultureIgnoreCase)) {
                    cheatVar.Setter.Invoke(enable);

                    Debug.Log(cheatVar.Name + ": " + enable);
                    break;
                }
            }
        }

        #region Nested type: CheatVar

        private struct CheatVar<T> {
            public readonly string Name;
            public readonly Action<T> Setter;
            public readonly string Description;

            public CheatVar(string name, Action<T> setter) {
                this.Name = name;
                this.Setter = setter;
                this.Description = "No description";
            }

            public CheatVar(string name, string description, Action<T> setter) {
                this.Name = name;
                this.Description = description;
                this.Setter = setter;
            }
        }

        #endregion
    }
    // ReSharper restore UnusedMember.Local
    #endregion

    #region Nested type: CheatAttribute

    private sealed class CheatAttribute : Attribute {
        private readonly string name;

        public CheatAttribute(string name) {
            this.name = name;
        }

        public string Name {
            get { return this.name; }
        }
    }

    #endregion



    #region Nested type: Dialog

    /// <summary>
    /// Helper class for showing a cheat dialog
    /// </summary>
    public static class Dialog {
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

            GameCheatReferenceDialog.HideDialog();
        }

        /// <summary>
        /// Disables showing of the dialog
        /// </summary>
        public static void HideDialog() {
            _ShowDialog = false;
            GameCheatReferenceDialog.HideDialog();
        }

        /// <summary>
        /// Draws the dialog. Call in <c>OnGUI</c>.
        /// </summary>
        /// <param name="skin"></param>
        public static void DrawDialog(GUISkin skin) {
            if (_ShowDialog) {
                _DialogRect = GUILayout.Window(0, _DialogRect, i => DrawInsideDialog(i, skin), "Cheats",
                                               skin.GetStyle("window"));
            }

            GameCheatReferenceDialog.DrawDialog(skin);
        }

        private static void DrawInsideDialog(int dialogId, GUISkin skin) {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Enter cheat: ", skin.GetStyle("label"));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _EnteredCheat = GUILayout.TextField(_EnteredCheat, 60, skin.GetStyle("textfield"));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.Width(50));
            if (GUILayout.Button("Apply", skin.GetStyle("button"))) {
                _ShowDialog = false;

                if (String.IsNullOrEmpty(_EnteredCheat)) {
                    _EnteredCheat = String.Empty;
                }
                else {
                    ApplyCheat(_EnteredCheat);
                }
            }
            GUILayout.EndHorizontal();
        }

        private static void ApplyCheat(string cheatText) {
            // select correct cheat member
            MethodInfo cheatMember = FindCheatMemberByCheatAttribute(cheatText);

            if (cheatMember == null) {
                // check for 'enable/disable' fallback
                cheatMember = FindCheatMemberByCheatAttribute("enable/disable");

                // split by space
                string[] split = cheatText.Split(new char[] {' '}, 2);
                if (split.Length != 2) {
                    return;
                }

                bool enable = String.Equals(split[0], "enable", StringComparison.InvariantCultureIgnoreCase);

                cheatMember.Invoke(null, new object[] {enable, split[1]});
                return;
            }

            // call
            cheatMember.Invoke(null, null);

            Debug.Log("Applied cheat: " + cheatMember.Name);
        }

        private static MethodInfo FindCheatMemberByCheatAttribute(string cheatText) {
            MethodInfo[] cheatMembers = typeof (Impl).GetMethods(BindingFlags.Public | BindingFlags.Static);

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

    #endregion

    private static class GameCheatReferenceDialog {
        private static Rect _DialogRect;
        private static bool _ShowDialog;

        private static string _DialogBodyTitle;
        private static string[] _DialogBodyTextColumn1;
        private static string[] _DialogBodyTextColumn2;
        private static string _DialogBodyTextStyleName;

        /// <summary>
        /// Enables showing of the dialog
        /// </summary>
        public static void ShowDialog(string title, string[] bodyColumn1, string[] bodyColumn2, string bodyStyleName) {
            _ShowDialog = true;

            const int width = 500;
            const int height = 400;
            _DialogRect = new Rect(Screen.width / 2 - (width / 2), Screen.height / 2 - (height / 2), width, height);

            _DialogBodyTitle = title;
            _DialogBodyTextColumn1 = bodyColumn1;
            _DialogBodyTextColumn2 = bodyColumn2;
            _DialogBodyTextStyleName = bodyStyleName;

            if (bodyColumn1.Length != bodyColumn2.Length) {
                throw new Exception("Both column text arrays must have same length!");
            }
        }

        /// <summary>
        /// Disables showing of the dialog
        /// </summary>
        public static void HideDialog() {
            _ShowDialog = false;
        }

        /// <summary>
        /// Draws the dialog. Call in <c>OnGUI</c>.
        /// </summary>
        /// <param name="skin"></param>
        public static void DrawDialog(GUISkin skin) {
            if (_ShowDialog) {
                _DialogRect = GUILayout.Window(0, _DialogRect, i => DrawInsideDialog(i, skin), _DialogBodyTitle,
                                               skin.GetStyle("window"));
            }
        }

        private static void DrawInsideDialog(int dialogId, GUISkin skin) {
            GUIStyle textStyle = skin.GetStyle(_DialogBodyTextStyleName);

            for (int i=0;i<_DialogBodyTextColumn1.Length&&i<_DialogBodyTextColumn2.Length;i++) {
                string col1Text = _DialogBodyTextColumn1[i];
                string col2Text = _DialogBodyTextColumn2[i];

                GUILayout.BeginHorizontal();

                bool createSecondColumn = col2Text != null;
                if (createSecondColumn) {
                    GUILayout.Label(col1Text, textStyle, GUILayout.Width(175));

                    GUILayout.BeginVertical();
                    GUILayout.Label(col2Text, textStyle);
                    GUILayout.EndVertical();
                } else {
                    GUILayout.Label(col1Text, textStyle);
                }

                GUILayout.EndHorizontal();
            }
            
            GUILayout.BeginHorizontal(GUILayout.Width(50));
            if (GUILayout.Button("Close", skin.GetStyle("button"))) {
                _ShowDialog = false;
            }
            GUILayout.EndHorizontal();
        }

    }
}