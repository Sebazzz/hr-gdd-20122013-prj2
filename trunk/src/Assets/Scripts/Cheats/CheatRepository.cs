using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Class with the single purpose of descrbing cheats and aggregrating them
/// </summary>
public static class CheatRepository {
    private static readonly List<CheatCommandDescriptor> CheatCommands;
    private static readonly List<CheatVariabeleDescriptor> CheatVariables; 

    static CheatRepository() {
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

            CheatCommandDescriptor descriptor = new CheatCommandDescriptor(
                cheatAttr.Name,
                description,
                member,
                member.GetParameters());

            CheatCommands.Add(descriptor);
        }
    }

    /// <summary>
    /// Gets all cheat commands in the system
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<CheatCommandDescriptor> GetAllCommands() {
        return CheatCommands;
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
}