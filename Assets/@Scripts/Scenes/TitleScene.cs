using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static Define;

public class TitleScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.TitleScene;
        //TitleUI
    }

    private void Start()
    {
        Managers.Resource.LoadAllAsync<Object>("Preload", (key, count, totalCount) =>
        {
            Debug.Log($"{key} {count}/{totalCount}");

            if (count == totalCount)
            {
                StartLoaded();
            }
        });
    }

    void StartLoaded()
    {
        Managers.Data.Init();

        Managers.UI.ShowSceneUI<UI_TitleScene>();
    }

    public override void Clear()
    {

    }

}
