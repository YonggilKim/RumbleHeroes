using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEditor.Build.Reporting;
using Debug = UnityEngine.Debug;

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

    [MenuItem("Tools/Upload To S3")]
    public static void UploadToS3()
    {
        string path = Application.dataPath;
        string batFilePath = Path.GetFullPath(Path.Combine(path, @"../copy_to_s3.bat"));


        // ProcessStartInfo startInfo = new ProcessStartInfo
        // {
        //     FileName = batFilePath,
        //     UseShellExecute = true,
        //     Verb = "runas", // "runas"는 관리자 권한으로 실행하도록 하는 옵션
        // };

        // Process.Start(startInfo);
        
        string awsCliPath = @"C:\Program Files\Amazon\AWSCLI\bin\aws"; // AWS CLI 설치 경로에 맞게 수정
        string arguments = "s3 cp ServerData s3://rookiss-rumble-addressables/Android/ --recursive";

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = awsCliPath,
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        Process process = new Process
        {
            StartInfo = startInfo
        };

        process.Start();

        // 명령어 실행 결과를 출력합니다.
        UnityEngine.Debug.Log(process.StandardOutput.ReadToEnd());

        process.WaitForExit();
        process.Close();
    }


}