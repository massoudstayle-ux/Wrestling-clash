using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class WrestlingAndroidBuilder
{
    public static void Build()
    {
        try
        {
            string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            string sceneRelativePath = "Assets/Scenes/Main.unity";
            string sceneFullPath = Path.Combine(projectRoot, sceneRelativePath);

            // Never manufacture an empty scene: that would produce a blank APK
            // and falsely suggest that the wrestling game had been built.
            if (!File.Exists(sceneFullPath))
                throw new FileNotFoundException(
                    "Gameplay scene is missing: " + sceneRelativePath +
                    ". Restore/import the complete Unity game source before building.");

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(sceneRelativePath, true)
            };

            PlayerSettings.productName = "Wrestling Clash";
            PlayerSettings.SetApplicationIdentifier(
                BuildTargetGroup.Android, "com.massoudstayle.wrestlingclash");
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(
                BuildTargetGroup.Android, BuildTarget.Android))
                throw new Exception("Unity could not switch the active build target to Android.");

            EditorUserBuildSettings.buildAppBundle = false;

            string workspace = Environment.GetEnvironmentVariable("GITHUB_WORKSPACE");
            if (string.IsNullOrEmpty(workspace))
                workspace = Path.GetFullPath(Path.Combine(projectRoot, ".."));

            string outputPath = Path.Combine(
                workspace, "build", "Android", "RingClash3D.apk");
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { sceneRelativePath },
                locationPathName = outputPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            });

            if (report.summary.result != BuildResult.Succeeded)
                throw new Exception("Android build failed: " + report.summary.result);

            if (!File.Exists(outputPath) || new FileInfo(outputPath).Length == 0)
                throw new Exception("Unity reported success but the APK is missing or empty.");

            Debug.Log("WRESTLING_ANDROID_APK=" + outputPath);
            Debug.Log("APK_SIZE_BYTES=" + new FileInfo(outputPath).Length);
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }
}
