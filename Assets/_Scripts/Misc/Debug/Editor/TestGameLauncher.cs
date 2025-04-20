using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class TestGameLauncher
{
    private const string ScenePath = "Assets/Scenes/PlayerGame.unity"; // Adjust this path as needed
    private const string DebugObjectName = "========DEBUGGING/TEST STUFF ENABLE TO ADD 5 CPUs========";

    private static bool shouldEnableDebugObject = false;

    [MenuItem("Tools/Play Test Game %t")]
    public static void PlayPlayerGame()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning("Already in Play mode.");
            return;
        }

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(ScenePath);
            shouldEnableDebugObject = true;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.isPlaying = true;
        }
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode && shouldEnableDebugObject)
        {
            EnableInactiveDebugObject();
            shouldEnableDebugObject = false;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
    }

    private static void EnableInactiveDebugObject()
    {
        foreach (var transform in Resources.FindObjectsOfTypeAll<Transform>())
        {
            if (transform.name == DebugObjectName)
            {
                transform.gameObject.SetActive(true);
                Debug.Log($"[TestGameLauncher] Enabled debug GameObject: {DebugObjectName}");
                return;
            }
        }

        Debug.LogWarning($"[TestGameLauncher] Debug GameObject '{DebugObjectName}' not found.");
    }
}
