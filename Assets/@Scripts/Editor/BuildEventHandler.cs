using UnityEditor;

[InitializeOnLoad]
public class BuildEventHandler
{
    static BuildEventHandler()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            BeforePlay();
        }
    }

    /// <summary>
    /// 에디터에서 Play 버튼 누르면 먼저 실행되는 함수
    /// 어드레서블의 Profile의 Remote를 Default로 바꿔서 다운로드 없이 로컬에서 바로 실행 할 수 있도록 한다.
    /// </summary>
    private static void BeforePlay()
    {
        EditorUtils.SetAddressableProfile(Define.EBuildType.Editor_Local);
    }
}

public class AndroidBuildPreprocessor
{
    [InitializeOnLoadMethod]
    static void Initialize()
    {
        // 빌드 전에 실행될 함수 등록
        BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayerOptionsCallback);
    }

    static void BuildPlayerOptionsCallback(BuildPlayerOptions buildPlayerOptions)
    {
        EditorUtils.SetAddressableProfile(Define.EBuildType.Remote);

        // 나머지 빌드 옵션 처리
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
}

