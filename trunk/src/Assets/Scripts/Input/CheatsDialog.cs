using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Class that contains all cheats implementation and provides a basic cheat dialog implementation
/// </summary>
public static class Cheats {
    #region Cheats Implementation
    // ReSharper disable UnusedMember.Local
    private static class Impl {
        [Cheat("PlayTheGround")]
        public static void LoadPlaygroundCheat() {
            AsyncSceneLoader.Load(Scenes.Playground);
        }

        [Cheat("ClearSettings")]
        public static void ClearSettings() {
            PlayerPrefs.DeleteAll();

            AsyncSceneLoader.Load(Scenes.MainMenu);
        }

        [Cheat("BeTheBest")]
        public static void SetAllLevelsPlayed() {
            Level currentLevel = Levels.GetFirstLevel();

            while (currentLevel != Level.None) {
                currentLevel.SetFinished();

                currentLevel = Levels.GetNextLevel(currentLevel);
            }

            AsyncSceneLoader.Load(Scenes.MainMenu);
        }

        [Cheat("enable/disable")]
        public static void EnableDisableVars(bool enable, string name) {
            // definition: cheat name and variabele
            var cheatVars = new List<CheatVar<bool>>();

            cheatVars.Add(new CheatVar<bool>("supersheep", v => CheatsController.EnableLargeSheep = v));

            // select member of cheat controller
            foreach (var cheatVar in cheatVars) {
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

            public CheatVar(string name, Action<T> setter) {
                this.Name = name;
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

            _DialogRect = new Rect(Screen.width/2 - 150, Screen.height/2 - 100, 300, 100);
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
                _DialogRect = GUILayout.Window(0, _DialogRect, i => DrawInsideDialog(i, skin), "Cheats",
                                               skin.GetStyle("window"));
            }
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
}