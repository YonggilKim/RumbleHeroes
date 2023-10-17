using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using static Define;

public class UI_TitleScene : UI_Scene
{
    #region Enum

    private enum GameObjects
    {
        StartButton
    }

    private enum Buttons
    {
    }


    private enum Texts
    {
    }
    #endregion


    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));

        GetObject((int)GameObjects.StartButton).BindEvent(() =>
        {
            Debug.Log("OnClick");
            Managers.Scene.LoadScene(EScene.GameScene);
        });

        //BindButton(typeof(Buttons));
        //BindText(typeof(Texts));
        
        return true;
    }

    private void Awake()
    {
        Init();
    }
    
}
