using System.Collections;
using UnityEngine;

/// <summary>
/// Asynchronously loads the scene specified
/// </summary>
public sealed class SceneLoaderController : MonoBehaviour {
    private bool isLoading;

    /// <summary>
    /// Defines standard minimum loading time
    /// </summary>
    public float StandardWaitTime = 1f;

    void Update() {
        if (!isLoading) {
            isLoading = true;

            StartCoroutine(this.LoadScene());
        }
    }

    IEnumerator LoadScene() {
        string scene = AsyncSceneLoader.GetTargetScene();

        // standard wait time
        yield return new WaitForSeconds(this.StandardWaitTime);

        AsyncSceneLoader.ClearTargetScene();

        // load
        AsyncOperation loadOperation = Application.LoadLevelAsync(scene);
        yield return loadOperation;

        yield break;
    }
}