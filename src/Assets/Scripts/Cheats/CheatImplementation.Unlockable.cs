 // Cheats implementation: Unlockables
public static partial class CheatImplementation {
    [CheatCommand("PlayTheGround", CheatCategory.Unlockables)]
    public static void LoadDeveloperSandboxLevel() {
        AsyncSceneLoader.Load(Scenes.Playground);
    }

    [CheatCommand("EnterCarnage", CheatCategory.Unlockables)]
    public static void EnterCarnageTestLevel() {
        AsyncSceneLoader.Load(Scenes.Carnage);
    }

	[CheatCommand("GameDemo", CheatCategory.Unlockables)]
    public static void EnterDemoLevel() {
        AsyncSceneLoader.Load(Scenes.DemoLevel);
    }

    [CheatCommand("EnterTheRace", CheatCategory.Unlockables)]
    public static void EnterParcoursCarnageDemoLevel() {
        AsyncSceneLoader.Load(Scenes.Parcour);
    }
}