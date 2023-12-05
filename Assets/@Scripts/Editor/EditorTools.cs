using System.Collections;
using System.Diagnostics;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class EditorTools
{
        [MenuItem("Tools/Upload To S3")]
        public static void UploadToS3()
        {
            string path = Application.dataPath;
            string batFilePath = Path.GetFullPath(Path.Combine(path, @"../copy_to_s3.bat"));
            
            string awsCliPath = @"C:\Program Files\Amazon\AWSCLI\bin\aws"; // AWS CLI 설치 경로에 맞게 수정
            string arguments = "s3 cp ServerData s3://rookiss-rumble-addressables/ --recursive";
    
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
    
            Debug.Log(process.StandardOutput.ReadToEnd());
    
            process.WaitForExit();
            process.Close();
        }
    
        [MenuItem("Clear/Clear Addressables")]
        public static void ClearAddressableData()
        {
            Caching.ClearCache();
        }
    
        [MenuItem("Tools/EditorPlay _F5")]
        public static void EditorPla2y()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(WaitForAddressablesBuildAndPlay());
        }
        
        private static IEnumerator WaitForAddressablesBuildAndPlay()
        {
            // AddressableAssetSettings.CleanPlayerContent();
            AddressablesPlayerBuildResult result;
            AddressableAssetSettings.BuildPlayerContent(out result);
    
            yield return new WaitForSeconds((float)result.Duration + 0.5f);
            // 빌드 완료 후 Play 모드 시작
            EditorApplication.isPlaying = true;
        }
}
