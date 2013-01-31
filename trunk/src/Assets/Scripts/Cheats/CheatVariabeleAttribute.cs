using System;

/// <summary>
/// Cheat variabeles are decorated with this attribute
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public sealed class CheatVariabeleAttribute : Attribute {
    private readonly string description;
    private readonly string name;

    /// <summary>
    /// Specifies a short name to call the variabele with
    /// </summary>
    public string Name {
        get { return this.name; }
    }

    /// <summary>
    /// Specifies a description, only used for documentation
    /// </summary>
    public string Description {
        get { return this.description; }
    }

    public CheatVariabeleAttribute(string name, string description) {
        this.name = name;
        this.description = description;
    }
}