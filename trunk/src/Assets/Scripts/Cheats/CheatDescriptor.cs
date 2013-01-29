using System.Reflection;

/// <summary>
/// Class with the purpose of describing a cheat
/// </summary>
public sealed class CheatDescriptor {
    public readonly string CommmandName;
    public readonly string Description;
    public readonly MethodInfo Method;
    public readonly ParameterInfo[] Parameters;

    public CheatDescriptor(string commmandName, string description, MethodInfo method, ParameterInfo[] parameters) {
        this.CommmandName = commmandName;
        this.Description = description;
        this.Method = method;
        this.Parameters = parameters;
    }
}