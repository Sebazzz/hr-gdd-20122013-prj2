 // Cheats implementation: Functional
public static partial class CheatsImplementation {
    [Cheat("PlayTheGround", CheatCategory.Unlockables)]
    public static void LoadDeveloperSandboxLevel() {
        AsyncSceneLoader.Load(Scenes.Playground);
    }
}