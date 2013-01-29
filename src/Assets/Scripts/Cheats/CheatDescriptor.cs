using System.Reflection;

/// <summary>
/// Class with the purpose of describing a cheat and providing necessary reflection information
/// </summary>
public sealed class CheatDescriptor {
    public readonly string CommandName;
    public readonly string Description;
    public readonly MethodInfo Method;
    public readonly ParameterInfo[] Parameters;

    public CheatDescriptor(string commandName, string description, MethodInfo method, ParameterInfo[] parameters) {
        this.CommandName = commandName;
        this.Description = description;
        this.Method = method;
        this.Parameters = parameters;
    }
}