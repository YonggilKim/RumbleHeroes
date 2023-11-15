using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;

// Output the build size or a failure depending on BuildPlayer.

public class BuildPlayer : MonoBehaviour
{
    [MenuItem("Tools/Build AOS")]
    public static void MyBuild_AOS()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[]
            { "Assets/@Scenes/TitleScene.unity", "Assets/@Scenes/LobbyScene.unity", "Assets/@Scenes/GameScene.unity" };
        buildPlayerOptions.locationPathName = $"./Builds/AOS_{PlayerSettings.bundleVersion}.apk";
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;

        PlayerSettings.Android.keystorePass = "rookiss";
        PlayerSettings.Android.keyaliasName = "rookiss";
        PlayerSettings.Android.keyaliasPass = "rookiss";

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }
}