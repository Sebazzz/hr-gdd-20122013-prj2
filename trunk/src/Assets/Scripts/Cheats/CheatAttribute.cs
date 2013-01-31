using System;

/// <summary>
/// Attribute to decorate cheats with.
/// </summary>
public sealed class CheatAttribute : Attribute {
    private readonly string name;
    private readonly CheatCategory category;

    public CheatAttribute(string name, CheatCategory category) {
        this.name = name;
        this.category = category;
    }

    public CheatAttribute(string name)
        : this(name, CheatCategory.GeneralCommands) {
    }

    public string Name {
        get { return this.name; }
    }

    public CheatCategory Category {
        get { return this.category; }
    }
}