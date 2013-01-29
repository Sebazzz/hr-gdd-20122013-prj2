using System;

/// <summary>
/// Attribute to decorate cheats with.
/// </summary>
public sealed class CheatAttribute : Attribute {
    private readonly string name;

    public CheatAttribute(string name) {
        this.name = name;
    }

    public string Name {
        get { return this.name; }
    }
}