using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEditor.Build.Reporting;
using Debug = UnityEngine.Debug;

// Output the build size or a failure depending on BuildPlayer.
public class JenkinsBuilder : MonoBehaviour
{
    //젠킨스
    [MenuItem("Tools/Build Android")]
    public static void BuildAndroid()
    {
        // 어드레서블 프로파일 변경
        EditorUtils.SetAddressableProfile(Define.EBuildType.Remote);
        
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[]
            { "Assets/@Scenes/TitleScene.unity", "Assets/@Scenes/LobbyScene.unity", "Assets/@Scenes/GameScene.unity" };
        buildPlayerOptions.locationPathName = $"./Builds/APK_{PlayerSettings.Android.bundleVersionCode}.apk";
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