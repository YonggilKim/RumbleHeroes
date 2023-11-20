using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildEventHandler : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        // 빌드 전에 호출되는 코드
        // 여기에서 필요한 작업을 수행할 수 있습니다.
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        // 빌드 후에 호출되는 코드
        // 여기에서 필요한 작업을 수행할 수 있습니다.
        if (report.summary.result == BuildResult.Succeeded)
        {
            // 빌드가 성공적으로 완료되었을 때의 동작
            Debug.Log("빌드가 성공적으로 완료되었습니다.");

            // 여기에 원하는 이벤트 처리 코드를 추가할 수 있습니다.
        }
        else
        {
            // 빌드가 실패했을 때의 동작
            Debug.LogError("빌드가 실패했습니다.");
        }
    }
}