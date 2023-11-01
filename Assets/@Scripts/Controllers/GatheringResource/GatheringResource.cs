using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GatheringResource : InteractionObject
{
    public int ResourceAmount;
    public float RegenTime;
    public string SpriteName;
    //GR Stage

    protected override bool Init()
    {
        base.Init();
      
        return true;
    }
    
    public void SetInfo(int creatureId)
    {
        DataId = creatureId;
        Dictionary<int, Data.GatheringResourceData> dict = Managers.Data.GatheringResourceDic;
        
        CurrentSprite.sprite = Managers.Resource.Load<Sprite>(dict[creatureId].SpriteName);
        MaxHp = dict[creatureId].MaxHp;
        ResourceAmount = dict[creatureId].ResourceAmount;
        RegenTime = dict[creatureId].RegenTime;
        SpriteName = dict[creatureId].SpriteName;

    }
    
}
