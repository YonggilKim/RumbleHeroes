using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using static Define;

public class TitleScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        ESceneType = Define.EScene.TitleScene;
        //TitleUI
        
        GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
        GraphicsSettings.transparencySortAxis = new Vector3(0.0f, 1.0f, 0.0f);
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

    private void StartLoaded()
    {
        Managers.Data.Init();
        Managers.UI.SceneUI = Managers.UI.ShowSceneUI<UI_TitleScene>();
    }

    public override void Clear()
    {

    }

}
