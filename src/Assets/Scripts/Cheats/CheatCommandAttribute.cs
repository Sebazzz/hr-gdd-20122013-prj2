using System;

/// <summary>
/// Cheat commands are decorated with this attribute
/// </summary>
public sealed class CheatCommandAttribute : Attribute {
    private readonly string name;
    private readonly CheatCategory category;

    public CheatCommandAttribute(string name, CheatCategory category) {
        this.name = name;
        this.category = category;
    }

    public CheatCommandAttribute(string name)
        : this(name, CheatCategory.GeneralCommands) {
    }

    /// <summary>
    /// Specifies the name of the cheat command. The command is called with this string value.
    /// </summary>
    public string Name {
        get { return this.name; }
    }

    /// <summary>
    /// Specifies the category. Only used for documentation.
    /// </summary>
    public CheatCategory Category {
        get { return this.category; }
    }
}