using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using DG.Tweening;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Serialization;

public class DropItemController : BaseController
{
    private DropItemData _data { get; set; }
    private SpriteRenderer _currentSprite;
    public int Amount { get; set; }


    private CurveShotComponent _shotComp;
    protected override bool Init()
    {
        base.Init();
        _currentSprite = gameObject.GetOrAddComponent<SpriteRenderer>();
        _shotComp = gameObject.GetOrAddComponent<CurveShotComponent>();
        return true;
    }

    public void SetInfo(int DataId, Vector2 pos)
    {
        _data = Managers.Data.DropItemDic[DataId];
        ObjectType = (Define.EObjectType)_data.DataId;
        
        var sprite = Managers.Resource.Load<Sprite>(_data.SpriteName);
        _currentSprite.sprite =Managers.Resource.Load<Sprite>(_data.SpriteName);
        
        _shotComp.Shot(transform.position, pos, EndCallback: Arrived);
        
    }
   

    void Arrived()
    {
        _currentSprite.DOFade(0, 1f).OnComplete(() =>
        {
            Managers.Object.Despawn(this);
        });
    }


}
