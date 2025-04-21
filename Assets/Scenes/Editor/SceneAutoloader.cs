using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Linq;

[InitializeOnLoad]
public class SceneAutoloader
{
    private const string SceneToLoadKey = "SceneToLoadOnStartup";
    private const string EnabledKey = "SceneLoadingEnabled";
    private const string editorScenes = "EditorScenes";


    static SceneAutoloader()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    [MenuItem("Assets/Set as Scene to Load on Startup", false, 100)]
    private static void SetSceneToLoadOnStartup(MenuCommand menuCommand)
    {
        string scenePath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (Selection.activeObject is not SceneAsset)
        {
            throw new System.Exception("Not a scene");
        }
        if (!string.IsNullOrEmpty(scenePath))
        {
            EditorPrefs.SetString(SceneToLoadKey, scenePath);
            Debug.Log("Scene set to load on startup: " + scenePath);
        }
    }

    [MenuItem("File/ToggleSceneAutoloading", false, 100)]
    private static void ToggleAutoloading(MenuCommand menuCommand)
    {
        EditorPrefs.SetBool(EnabledKey, !EditorPrefs.GetBool(EnabledKey));
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (!EditorPrefs.GetBool(EnabledKey)) return;
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            EditorPrefs.SetString(editorScenes, string.Join(';', EditorSceneManager.GetSceneManagerSetup().Where(scene => scene.isLoaded).OrderByDescending(scene => scene.isActive).Select(scene => scene.path)));
            EditorSceneManager.OpenScene(EditorPrefs.GetString(SceneToLoadKey));
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            bool hasActive = false;
            var originalScenes = EditorPrefs.GetString(editorScenes).Split(';').Select(path => new SceneSetup() { path = path, isLoaded = true, isActive = !hasActive | !(hasActive = true) }).ToArray();
            if (hasActive) EditorSceneManager.RestoreSceneManagerSetup(originalScenes);
        }
    }
}