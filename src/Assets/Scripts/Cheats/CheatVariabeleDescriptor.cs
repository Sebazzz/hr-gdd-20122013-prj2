using System.Reflection;

public sealed class CheatVariabeleDescriptor : CheatDescriptor {
    public readonly FieldInfo FieldInfo;

    public CheatVariabeleDescriptor(string name, string description, FieldInfo fieldInfo) : base(name, description) {
        this.FieldInfo = fieldInfo;
    }
}