using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatheringResource : InteractionObject
{
    private Data.GatheringResourceData _data;
    //GR Stage

    protected override bool Init()
    {
        base.Init();
      
        return true;
    }
    
    public void SetInfo(int creatureId)
    {
        DataId = creatureId;
        _data = Managers.Data.GatheringResourceDic[creatureId];
        CurrentSprite.sprite = Managers.Resource.Load<Sprite>(_data.SpriteName);

        MaxHp = _data.MaxHp;
        Hp = _data.MaxHp;
    }
    public override void OnDamaged(InteractionObject Attacker)
    {
        base.OnDamaged(Attacker);
        
        Hp = Mathf.Clamp(Hp-1, 0, MaxHp);
        if (Hp == 0)
        {
            Despawn();
        }
    }

    public override void Despawn()
    {
        base.Despawn();
    }
}
