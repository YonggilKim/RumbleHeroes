using UnityEngine;

public class LobbyScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        ESceneType = Define.EScene.LobbyScene;

        //TitleUI
        //Managers.UI.ShowSceneUI<UI_LobbyScene>();
        Screen.sleepTimeout = SleepTimeout.SystemSetting;

        //Managers.Sound.Play(Define.Sound.Bgm, "Bgm_Lobby");
    }

    public override void Clear()
    {

    }

}
