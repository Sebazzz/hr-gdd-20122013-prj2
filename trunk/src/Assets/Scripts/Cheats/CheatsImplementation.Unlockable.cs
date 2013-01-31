 // Cheats implementation: Functional
public static partial class CheatsImplementation {
    [Cheat("PlayTheGround")]
    public static void LoadDeveloperSandboxLevel() {
        AsyncSceneLoader.Load(Scenes.Playground);
    }
}