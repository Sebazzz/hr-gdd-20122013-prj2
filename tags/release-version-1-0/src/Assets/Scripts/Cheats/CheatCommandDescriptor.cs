using System.Reflection;

/// <summary>
/// Class with the purpose of describing a cheat and providing necessary reflection information
/// </summary>
public sealed class CheatCommandDescriptor : CheatDescriptor {
    public readonly MethodInfo Method;
    public readonly ParameterInfo[] Parameters;
    public readonly CheatCategory Category;

    public CheatCommandDescriptor(string name, string description, CheatCategory category, MethodInfo method, ParameterInfo[] parameters) : base(name, description) {
        this.Method = method;
        this.Category = category;
        this.Parameters = parameters;
    }
}