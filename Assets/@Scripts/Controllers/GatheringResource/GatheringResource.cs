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
        ObjectType = Define.EObjectType.GatheringResources;

        Attribute = gameObject.AddComponent<AttributeSet>();
        Attribute.MaxHp.BaseValue = _data.MaxHp;
        Attribute.MaxHp.CurrentValue = _data.MaxHp;
        Attribute.Hp.BaseValue = _data.MaxHp;
        Attribute.Hp.CurrentValue = _data.MaxHp;

    }
    public override void OnDamaged(InteractionObject Attacker)
    {
        base.OnDamaged(Attacker);
        
        Attribute.Hp.CurrentValue = Mathf.Clamp(Attribute.Hp.CurrentValue-1, 0, Attribute.MaxHp.CurrentValue);
        if (Attribute.Hp.CurrentValue == 0)
        {
            var dropItem = Managers.Object.Spawn<DropItemController>(transform.position, _data.DropItemId);
            Vector2 ran = new Vector2(transform.position.x + Random.Range( -10, -15) * 0.1f, transform.position.y);
            Vector2 ran2 = new Vector2(transform.position.x + Random.Range( 10, 15) * 0.1f, transform.position.y);
            Vector2 dropPos = Random.value < 0.5 ? ran : ran2;
            // Vector2 DropPos = new Vector2(1f, transform.position.y);
            dropItem.SetInfo(_data.DropItemId, dropPos);
            
            Despawn();
        }
    }

    public override void Despawn()
    {
        base.Despawn();
    }
}
