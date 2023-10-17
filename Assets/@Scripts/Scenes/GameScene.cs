using Data;
using DG.Tweening.Plugins.Core.PathCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Define;

public class GameScene : BaseScene
{
    private void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        Debug.Log("@>> GameScene Init()");
        base.Init();
        ESceneType = Define.EScene.GameScene;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Managers.UI.ShowSceneUI<UI_Joystick>();

        Managers.Object.Spawn<HeroController>(Vector3.zero, 201000);
        LoadStage();
    }
    
    public void LoadStage()
    {
        Managers.Object.LoadMap("BaseMap");
    }

    public override void Clear()
    {

    }

}
