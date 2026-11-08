using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class WrestlingProjectSetup
{
    static WrestlingProjectSetup()
    {
        EditorApplication.delayCall += EnsureMainScene;
    }

    private static void EnsureMainScene()
    {
        const string folder = "Assets/Scenes";
        const string scenePath = folder + "/Main.unity";
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        if (!File.Exists(scenePath))
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, scenePath);
        }

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(scenePath, true)
        };
        AssetDatabase.Refresh();
    }
}
