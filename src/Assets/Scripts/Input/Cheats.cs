using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

/// <summary>
/// Class that contains all cheats implementation and provides a basic cheat dialog implementation
/// </summary>
public static class Cheats {
    #region Cheats Implementation
    // ReSharper disable UnusedMember.Local
    /// <summary>
    /// Cheat implementation class. These public static members are called via reflection only.
    /// </summary>
    private static class Impl {
        #region Functional (useful) cheats

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

                // format member name into words
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

                cheatDescription.Add(format.ToString());

                // command formatted with arguments
                format = new StringBuilder();
                format.Append(indent + cheatAttr.Name);

                foreach (ParameterInfo parameterInfo in  member.GetParameters()) {
                    format.AppendFormat(" <{0}>", parameterInfo.Name);
                }

                cheatName.Add(format.ToString());
            }

            cheatName.Add(indent);
            cheatDescription.Add(indent);

            // title for enable/disable commands
            cheatName.Add("Variabeles to enable/disable:");
            cheatDescription.Add(null);

            // ... aggregate any enable/disable vars
            foreach (CheatVar<bool> cheatVar in ToggleCheatVars) {
                cheatName.Add(indent + cheatVar.Name);
                cheatDescription.Add(cheatVar.Description);
            }

            cheatName.Add("Note: Some cheats may only be applied after a level reload.");
            cheatDescription.Add(null);

            // show the dialog
            GameCheatReferenceDialog.ShowDialog("Cheats Reference", cheatName.ToArray(), cheatDescription.ToArray(), "MonospaceLabel");
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

        #endregion

        #region Variabele modifications

        [Cheat("enable")]
        public static void EnableTheSpecifiedVariabele(string variabele) {
            SetBooleanVariabeleValue(variabele, true);
        }

        [Cheat("disable")]
        public static void DisableTheSpecifiedVariabele(string variabele) {
            SetBooleanVariabeleValue(variabele, true);
        }

        private static void SetBooleanVariabeleValue(string variabele, bool value) {
            // select member of cheat controller
            foreach (var cheatVar in ToggleCheatVars) {
                if (String.Equals(cheatVar.Name, variabele, StringComparison.InvariantCultureIgnoreCase)) {
                    cheatVar.Setter.Invoke(value);

                    Debug.Log(cheatVar.Name + ": " + value);
                    return;
                }
            }

            SimpleTextDialog.ShowDialog("Error", String.Format("Variabele could not be set to '{1}': Variabele '{0}' does not exist.", variabele, value), "MonospaceLabel");
        }

        private static readonly List<CheatVar<bool>> ToggleCheatVars = new List<CheatVar<bool>>();
        private static readonly List<CheatVar<float>> FloatCheatVars = new List<CheatVar<float>>();

        static Impl() {
            ToggleCheatVars.Add(new CheatVar<bool>("supersheep", "Enlarge all sheeps in the next levels 4 times", v => CheatsController.EnableLargeSheep = v));
            ToggleCheatVars.Add(new CheatVar<bool>("gamecheats", "Show the in-game cheat button and menu", v => CheatsController.EnableInGameCheatsMenu = v));
            ToggleCheatVars.Add(new CheatVar<bool>("terrainbounce", "Enable a bouncy terrain", v => CheatsController.TerrainBounce = v));
            ToggleCheatVars.Add(new CheatVar<bool>("sheeprotationlock", "Rotation lock of sheep. By default true.", v => CheatsController.EnableSheepRotationLock = v));
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

        #endregion

        #region Unlockables

        [Cheat("PlayTheGround")]
        public static void LoadDeveloperSandboxLevel() {
            AsyncSceneLoader.Load(Scenes.Playground);
        }

        #endregion

        #region Testing Helpers

        [Cheat("EndlessCameraMovement")]
        public static void DisableCameraMovementBounds() {
            ArrowMovementCameraBehaviour cameraScript = Camera.mainCamera.GetComponent<ArrowMovementCameraBehaviour>();

            if (cameraScript != null) {
                cameraScript.BoundingBoxBottomLeft = new Vector2(-1000, -1000);
                cameraScript.BoundingBoxTopRight = new Vector2(1000, 1000);
            }
        }

        #endregion

        #region Just for Fun
        
        [Cheat("SheepRain")]
        public static void StartsRainingSheepAllOverTheLevel(int amountPerSecond, int timeInSeconds) {
            // find a sheep to clone
            GameObject sheepToClone = GameObject.FindGameObjectWithTag(Tags.Sheep);

            StartCoroutine(SheepRainExecutor(amountPerSecond, timeInSeconds, sheepToClone));
        }

        private static IEnumerator SheepRainExecutor(int amountPerSecond, int timeInSeconds, GameObject sheepTemplate) {
            float timeLeft = timeInSeconds;

            // determine terrain bounds
            Vector3 lowerBounds = Terrain.activeTerrain.transform.position;
            Vector3 upperBounds = lowerBounds + Terrain.activeTerrain.terrainData.size;

            // split in two
            int amountPerSecondLower = Mathf.FloorToInt(amountPerSecond / 2f);
            int amountPerSecondUpper = Mathf.CeilToInt(amountPerSecond / 2f);

            while (timeLeft > 0) {
                // first half of second
                for (int n=0;n<amountPerSecondLower;n++) {
                    SpawnSheep(sheepTemplate, lowerBounds, upperBounds);
                }

                LevelBehaviour.Instance.RecountSheep();
                yield return new WaitForSeconds(0.5f);
                timeLeft -= 0.5f;

                // second half of second
                for (int n = 0; n < amountPerSecondUpper; n++) {
                    SpawnSheep(sheepTemplate, lowerBounds, upperBounds);
                }

                LevelBehaviour.Instance.RecountSheep();
                yield return new WaitForSeconds(0.5f);
                timeLeft -= 0.5f;
            }

            yield break;
        }

        private static void SpawnSheep(GameObject sheepTemplate, Vector3 lowerBounds, Vector3 upperBounds) {
            // determine position
            Vector3 spawnPosition = new Vector3(Random.Range(lowerBounds.x, upperBounds.x),
                                                Camera.mainCamera.transform.position.y + 50f,
                                                Random.Range(lowerBounds.z, upperBounds.z));

            // determine rotation
            Vector3 rotation = new Vector3();
            rotation.y = Random.Range(0, 360);

            if (!CheatsController.EnableSheepRotationLock) {
                rotation.x = Random.Range(0, 360);
                rotation.z = Random.Range(0, 360);
            }

            Object.Instantiate(sheepTemplate, spawnPosition, Quaternion.Euler(rotation));
        }

        [Cheat("SheepRocket")]
        public static void LaunchAllSheepUpIntoTheAirByTheSpecifiedForce(float force) {
            GameObject[] sheep = GameObject.FindGameObjectsWithTag(Tags.Sheep);

            foreach (GameObject gameObject in sheep) {
                Rigidbody rb = gameObject.rigidbody;

                float finalForce = force*rb.mass;

                rb.AddRelativeForce(Vector3.up * finalForce, ForceMode.Impulse);
            }
        }

        #endregion

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
                CheatsController ctrl = worldObject.GetComponent<CheatsController>();

                if (ctrl != null) {
                    return ctrl;
                }
            }

            SimpleTextDialog.ShowDialog("Error", "You're not currently in a level and the command cannot be executed therefore.", "MonospaceLabel");
            return null;
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
    public static class InputDialog {
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
            SimpleTextDialog.HideDialog();
        }

        /// <summary>
        /// Disables showing of the dialog
        /// </summary>
        public static void HideDialog() {
            _ShowDialog = false;

            GameCheatReferenceDialog.HideDialog();
            SimpleTextDialog.HideDialog();
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
            SimpleTextDialog.DrawDialog(skin);
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
            if (GUI.GetNameOfFocusedControl() == string.Empty) {
                GUI.FocusControl("CheatInputBox");
            }

        }

        private static void ApplyCheat(string cheatText) {
            string[] arguments = cheatText.Split(' ');

            // select correct cheat member
            MethodInfo cheatMember = FindCheatMemberByCheatAttribute(arguments[0]);

            // check if cheat is found
            if (cheatMember == null) {
                SimpleTextDialog.ShowDialog("Error", "Cheat could not be applied: cheat not found. Type 'help' for cheat reference.", "MonospaceLabel");
                return;
            }

            // select parameters
            ParameterInfo[] memberParams = cheatMember.GetParameters();
            
            if (memberParams.Length != arguments.Length - 1) {
                SimpleTextDialog.ShowDialog("Error", String.Format("Cheat could not be applied: Expected {0} parameters, but got {1} parameters", memberParams.Length, arguments.Length-1) , "MonospaceLabel");
                return;
            }

            object[] parsedParameters = new object[memberParams.Length];
            for (int i = 0; i < memberParams.Length && i < parsedParameters.Length; i++) {
                ParameterInfo currentParam = memberParams[i];
                string rawArgument = arguments[i + 1];

                try {
                     parsedParameters[i] = Convert.ChangeType(rawArgument, currentParam.ParameterType, CultureInfo.InvariantCulture);
                } catch (Exception) {
                    SimpleTextDialog.ShowDialog("Error", String.Format("Cheat could not be applied: Value '{0}' for parameter '{1}' could not be parsed as '{2}'", rawArgument, currentParam.Name, currentParam.ParameterType.FullName), "MonospaceLabel");
                    return;
                }
            }

            // call
            cheatMember.Invoke(null, parsedParameters);

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
        private static Vector2 _ScrollPosition;

        private static string _DialogBodyTitle;
        private static string[] _DialogBodyTextColumn1;
        private static string[] _DialogBodyTextColumn2;
        private static string _DialogBodyTextStyleName;

        /// <summary>
        /// Enables showing of the dialog
        /// </summary>
        public static void ShowDialog(string title, string[] bodyColumn1, string[] bodyColumn2, string bodyStyleName) {
            _ShowDialog = true;

            const int width = 600;
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

            _ScrollPosition = GUILayout.BeginScrollView(_ScrollPosition, false, true, skin.GetStyle("horizontalscrollbar"),
                                      skin.GetStyle("verticalscrollbar"), skin.GetStyle("scrollview"), GUILayout.ExpandWidth(true), GUILayout.Height(350));
            for (int i=0;i<_DialogBodyTextColumn1.Length&&i<_DialogBodyTextColumn2.Length;i++) {
                string col1Text = _DialogBodyTextColumn1[i];
                string col2Text = _DialogBodyTextColumn2[i];

                GUILayout.BeginHorizontal();

                bool createSecondColumn = col2Text != null;
                if (createSecondColumn) {
                    GUILayout.Label(col1Text, textStyle, GUILayout.Width(225));

                    GUILayout.BeginVertical();
                    GUILayout.Label(col2Text, textStyle);
                    GUILayout.EndVertical();
                } else {
                    GUILayout.Label(col1Text, textStyle);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
            
            GUILayout.BeginHorizontal(GUILayout.Width(50));
            GUI.SetNextControlName("CloseButton");
            if (GUILayout.Button("Close", skin.GetStyle("button"))) {
                _ShowDialog = false;
            }
            
            GUILayout.EndHorizontal();

            // set focus to close button
            if (GUI.GetNameOfFocusedControl() == string.Empty) {
                GUI.FocusControl("CloseButton");
            }
        }

    }

    private static class SimpleTextDialog {
        private static Rect _DialogRect;
        private static bool _ShowDialog;

        private static string _DialogBodyTitle;
        private static string _DialogBodyText;
        private static string _DialogBodyTextStyleName;

        /// <summary>
        /// Enables showing of the dialog
        /// </summary>
        public static void ShowDialog(string title, string text, string bodyStyleName) {
            _ShowDialog = true;

            const int width = 300;
            const int height = 100;
            _DialogRect = new Rect(Screen.width / 2 - (width / 2), Screen.height / 2 - (height / 2), width, height);

            _DialogBodyTitle = title;
            _DialogBodyText = text;
            _DialogBodyTextStyleName = bodyStyleName;
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

            GUILayout.BeginHorizontal();

            GUILayout.Label(_DialogBodyText, textStyle);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.Width(50));
            GUI.SetNextControlName("CloseButton");
            if (GUILayout.Button("Close", skin.GetStyle("button"))) {
                _ShowDialog = false;
            }
            GUILayout.EndHorizontal();

            // set focus to close button
            if (GUI.GetNameOfFocusedControl() == string.Empty) {
                GUI.FocusControl("CloseButton");
            }
        }

    }
}