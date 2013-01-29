using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Class with the single purpose of descrbing cheats and aggregrating them
/// </summary>
public static class CheatReference {
    private static readonly List<CheatDescriptor> _Cheats;

    static CheatReference() {
        _Cheats =  new List<CheatDescriptor>();
        
        MethodInfo[] cheatMembers = typeof(CheatsImplementation).GetMethods(BindingFlags.Public | BindingFlags.Static);

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

            string description = format.ToString();

            CheatDescriptor descriptor = new CheatDescriptor(
                cheatAttr.Name,
                description,
                member,
                member.GetParameters());

            _Cheats.Add(descriptor);
        }
    }

    /// <summary>
    /// Gets all cheats in the system
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<CheatDescriptor> GetAllCheats() {
        return _Cheats;
    }

    /// <summary>
    /// Gets the cheat by the specified command name. If not found, returns null.
    /// </summary>
    /// <param name="commandName"></param>
    /// <returns></returns>
    public static CheatDescriptor GetCheatByCommandName(string commandName) {
        foreach (CheatDescriptor cheat in _Cheats) {
            if (String.Equals(cheat.CommandName, commandName, StringComparison.InvariantCultureIgnoreCase)) {
                return cheat;
            }
        }

        return null;
    } 
}