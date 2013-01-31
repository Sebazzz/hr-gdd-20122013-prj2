using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Class with the single purpose of descrbing cheats and aggregrating them
/// </summary>
public static class CheatService {
    private static readonly List<CheatCommandDescriptor> CheatCommands;
    private static readonly List<CheatVariabeleDescriptor> CheatVariables; 

    static CheatService() {
        CheatCommands =  new List<CheatCommandDescriptor>();
        CheatVariables = new List<CheatVariabeleDescriptor>();

        FindCheatCommands();
        FindCheatVariables();
    }

    private static void FindCheatVariables() {
        FieldInfo[] cheatVariables = typeof (CheatVariables).GetFields(BindingFlags.Public | BindingFlags.Static);

        foreach (FieldInfo member in cheatVariables) {
            // get cheat attr
            object[] attr = member.GetCustomAttributes(typeof(CheatVariabeleAttribute), false);

            if (attr.Length == 0) {
                continue;
            }

            var cheatAttr = attr[0] as CheatVariabeleAttribute;
            if (cheatAttr == null) {
                continue;
            }

            // add variabele
            CheatVariabeleDescriptor descriptor = new CheatVariabeleDescriptor(cheatAttr.Name, cheatAttr.Description, member);

            CheatVariables.Add(descriptor);
        }

        // order list
        CheatCommands.Sort((a,b) => System.String.Compare(a.Name, b.Name, System.StringComparison.InvariantCultureIgnoreCase));
    }

    private static void FindCheatCommands() {
        MethodInfo[] cheatMembers = typeof (CheatImplementation).GetMethods(BindingFlags.Public | BindingFlags.Static);

        foreach (MethodInfo member in cheatMembers) {
            // get cheat attr
            object[] attr = member.GetCustomAttributes(typeof (CheatCommandAttribute), false);

            if (attr.Length == 0) {
                continue;
            }

            var cheatAttr = attr[0] as CheatCommandAttribute;
            if (cheatAttr == null) {
                continue;
            }

            // format member name into words
            string memberName = member.Name;
            string description = ConvertPascalCasedToSentence(memberName);

            CheatCommandDescriptor descriptor = new CheatCommandDescriptor(
                cheatAttr.Name,
                description,
                cheatAttr.Category,
                member,
                member.GetParameters());

            CheatCommands.Add(descriptor);
        }

        // order list
        CheatVariables.Sort((a, b) => System.String.Compare(a.Name, b.Name, System.StringComparison.InvariantCultureIgnoreCase));
    }

    private static string ConvertPascalCasedToSentence(string memberName) {
        string[] words = Regex.Split(memberName, "([A-Z][a-z]+)", RegexOptions.None);

        StringBuilder format = new StringBuilder();
        foreach (string word in words) {
            if (format.Length == 0) {
                format.Append(word);
            }
            else {
                format.Append(" " + word.ToLowerInvariant());
            }
        }

        string description = format.ToString();
        return description;
    }

    /// <summary>
    /// Gets all cheat commands in the system
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<CheatCommandDescriptor> GetAllCommands() {
        return CheatCommands;
    }

    /// <summary>
    /// Gets all cheat commands in the system
    /// </summary>
    /// <returns></returns>
    public static Dictionary<string, List<CheatCommandDescriptor>> GetAllCommandsByHumanReadableCategory() {
        Dictionary<string, List<CheatCommandDescriptor>> result = new Dictionary<string, List<CheatCommandDescriptor>>();

        Array categories = Enum.GetValues(typeof (CheatCategory));

        foreach (CheatCategory category in categories) {
            string categoryAsString = ConvertPascalCasedToSentence(category.ToString());
            List<CheatCommandDescriptor> list = new List<CheatCommandDescriptor>();

            // list per category
            foreach (CheatCommandDescriptor command in CheatCommands) {
                if (command.Category == category) {
                    list.Add(command);
                }
            }

            result.Add(categoryAsString, list);
        }

        return result;
    }

    /// <summary>
    /// Gets all cheat variables in the system
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<CheatVariabeleDescriptor> GetAllVariabeles() {
        return CheatVariables;
    }

    /// <summary>
    /// Gets the cheat by the specified command name. If not found, returns null.
    /// </summary>
    /// <param name="commandName"></param>
    /// <returns></returns>
    public static CheatCommandDescriptor GetCheatByCommandName(string commandName) {
        foreach (CheatCommandDescriptor cheat in CheatCommands) {
            if (String.Equals(cheat.Name, commandName, StringComparison.InvariantCultureIgnoreCase)) {
                return cheat;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets the cheat variabele by the specified command name. If not found, returns null.
    /// </summary>
    /// <param name="variabeleName"></param>
    /// <returns></returns>
    public static CheatVariabeleDescriptor GetCheatByVariabeleName(string variabeleName) {
        foreach (CheatVariabeleDescriptor cheat in CheatVariables) {
            if (String.Equals(cheat.Name, variabeleName, StringComparison.InvariantCultureIgnoreCase)) {
                return cheat;
            }
        }

        return null;
    }

    /// <summary>
    /// Executes the specified cheat
    /// </summary>
    /// <param name="cheat"></param>
    /// <param name="parameters"></param>
    public static void ExecuteCheat(CheatCommandDescriptor cheat, params object[] parameters) {
        // select parameters
        ParameterInfo[] memberParams = cheat.Parameters;

        if (memberParams.Length != parameters.Length) {
            CheatNotificationDialog.ShowDialog("Error", String.Format("Cheat could not be applied: Expected {0} parameters, but got {1} parameters", memberParams.Length, parameters.Length - 1));
            return;
        }

        object[] parsedParameters = new object[memberParams.Length];
        for (int i = 0; i < memberParams.Length && i < parsedParameters.Length; i++) {
            ParameterInfo currentParam = memberParams[i];
            object rawArgument = parameters[i];

            try {
                parsedParameters[i] = Convert.ChangeType(rawArgument, currentParam.ParameterType, CultureInfo.InvariantCulture);
            } catch (Exception) {
                CheatNotificationDialog.ShowDialog("Error", String.Format("Cheat could not be applied: Value '{0}' for parameter '{1}' could not be parsed as '{2}'", rawArgument, currentParam.Name, currentParam.ParameterType.FullName));
                return;
            }
        }

        // call
        cheat.Method.Invoke(null, parsedParameters);

        Debug.Log("Applied cheat: " + cheat.Name);
    }
}