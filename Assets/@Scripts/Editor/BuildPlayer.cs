using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEditor.Build.Reporting;

// Output the build size or a failure depending on BuildPlayer.

public class BuildPlayer : MonoBehaviour
{
    [MenuItem("Tools/Build AOS")]
    public static void MyBuild_AOS()
    {
        // 어드레서블 프로파일 변경
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        AddressableAssetProfileSettings profile = settings.profileSettings;
        string profileID2 = settings.profileSettings.GetProfileId("Remote");
        settings.activeProfileId = profileID2;
        
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[]
            { "Assets/@Scenes/TitleScene.unity", "Assets/@Scenes/LobbyScene.unity", "Assets/@Scenes/GameScene.unity" };
        buildPlayerOptions.locationPathName = $"./Builds/RumbleHeroes.apk";
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
    
    // 빌드 전에 호출되는 함수
    [MenuItem("Build/Preprocess Build")]
    static void PreprocessBuild()
    {
        // 이 곳에 빌드 전에 실행되길 원하는 코드를 작성
        Debug.Log("Preprocessing Build!");
        
        // 여기서 필요한 작업을 수행하세요.
        // 예: 설정 변경, 자동화된 작업 등
    }
}