using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public void LoadScene(Define.EScene type, Transform parents = null)
    {
        Managers.Clear();
        SceneManager.LoadScene(GetSceneName(type));

        //switch (CurrentScene.SceneType)
        //{
        //    case Define.Scene.TitleScene:
        //        Managers.Clear();
        //        SceneManager.LoadScene(GetSceneName(type));
        //        break;
        //    case Define.Scene.GameScene:
        //        Managers.Resource.Destroy(Managers.UI.SceneUI.gameObject);
        //        Managers.Clear();
        //        SceneManager.LoadScene(GetSceneName(type));
        //        break;
        //    case Define.Scene.LobbyScene:
        //        Managers.Resource.Destroy(Managers.UI.SceneUI.gameObject);
        //        Managers.Clear();
        //        SceneManager.LoadScene(GetSceneName(type));
        //        break;
        //}

    }

    private string GetSceneName(Define.EScene type)
    {
        string name = System.Enum.GetName(typeof(Define.EScene), type);
        return name;
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }
}
