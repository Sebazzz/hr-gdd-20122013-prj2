public abstract class CheatDescriptor {
    public readonly string Name;
    public readonly string Description;

    protected CheatDescriptor(string name, string description) {
        this.Name = name;
        this.Description = description;
    }
}