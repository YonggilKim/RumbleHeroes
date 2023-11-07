using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEditor.Build.Content;
using UnityEngine;

public class DropItemController : BaseController
{
    private DropItemData _data { get; set; }
    private SpriteRenderer _currentSprite;

    private int _amount;

    protected virtual bool Init()
    {
        base.Init();

        _currentSprite = gameObject.GetOrAddComponent<SpriteRenderer>();
        return true;
    }

    public void SetInfo(int DataId, int Amount = 1)
    {
        _data = Managers.Data.DropItemDic[DataId];
        _amount = 1;
    }


}
