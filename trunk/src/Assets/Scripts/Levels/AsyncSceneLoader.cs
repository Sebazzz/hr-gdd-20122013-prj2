using UnityEngine;

/// <summary>
/// Helper class for loading levels asynchronous
/// </summary>
internal static class AsyncSceneLoader {
    private static string CurrentLoadingScene;

    public static void Load(string sceneName) {
        CurrentLoadingScene = sceneName;

        Application.LoadLevel(Scenes.LoadingScreen);
    }

    internal static string GetTargetScene() {
        return CurrentLoadingScene;
    }

    internal static void ClearTargetScene() {
        CurrentLoadingScene = null;
    }
}