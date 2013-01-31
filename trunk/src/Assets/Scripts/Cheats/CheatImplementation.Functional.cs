﻿using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

// Cheats implementation: Functional
public static partial class CheatImplementation {
    [CheatCommand("Help", CheatCategory.GeneralCommands)]
    public static void ShowCheatsHelpReference() {
        const string indent = "    ";

        var cheatName = new List<string>();
        var cheatDescription = new List<string>();

        // title for general cheats
        cheatName.Add("General commands:");
        cheatDescription.Add(null);

        // ... aggregate all general cheats and format them nicely
        IEnumerable<CheatCommandDescriptor> cheatDescriptors = CheatService.GetAllCommands();

        foreach (CheatCommandDescriptor member in cheatDescriptors) {
            cheatDescription.Add(member.Description);

            // command formatted with arguments
            var format = new StringBuilder();
            format.Append(member.Name);

            foreach (ParameterInfo parameterInfo in member.Parameters) {
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
        foreach (CheatVariabeleDescriptor cheatVar in CheatService.GetAllVariabeles()) {
            cheatName.Add(cheatVar.Name);
            cheatDescription.Add(cheatVar.Description);
        }

        cheatName.Add(indent);
        cheatDescription.Add(indent);
        cheatName.Add("Note: Some cheats may only be applied after a level reload.");
        cheatDescription.Add(null);

        // show the dialog
        CheatReferenceDialog.ShowDialog("Cheats Reference", cheatName.ToArray(), cheatDescription.ToArray(),
                                        "CheatDialogLabel");
    }

    [CheatCommand("ClearSettings", CheatCategory.TestingHelpers)]
    public static void ClearAllSettings() {
        PlayerPrefs.DeleteAll();

        AsyncSceneLoader.Load(Scenes.MainMenu);
    }

    [CheatCommand("BeTheBest", CheatCategory.TestingHelpers)]
    public static void SetAllLevelsFullyPlayed() {
        Level currentLevel = Levels.GetFirstLevel();

        while (currentLevel != Level.None) {
            currentLevel.SetFinished();

            currentLevel = Levels.GetNextLevel(currentLevel);
        }

        AsyncSceneLoader.Load(Scenes.MainMenu);
    }

    [CheatCommand("Reload", CheatCategory.TestingHelpers)]
    public static void ReloadTheCurrentLevel() {
        AsyncSceneLoader.Load(Application.loadedLevelName);
    }
}