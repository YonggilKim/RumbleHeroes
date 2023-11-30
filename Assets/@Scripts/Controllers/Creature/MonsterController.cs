using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterController : CreatureController
{
    private bool _hasAttackAnimClip { get; set; } = false;

    protected override bool Init()
    {
        base.Init();
        ObjectType = Define.EObjectType.Monster;

        _hasAttackAnimClip = Util.HasAnimationClip(Anim, "Attack");
        
        return true;
    }

    public void OnEnable()
    {
        CreatureState = Define.ECreatureState.Idle;
    }
    
    protected override void UpdateAnimation()
    {
        base.UpdateAnimation();
        switch (CreatureState)
        {
            case Define.ECreatureState.Idle:
                break;
            case Define.ECreatureState.Attack:
                break;
            case Define.ECreatureState.Moving:
                break;
            case Define.ECreatureState.Gathering:
                break;
            case Define.ECreatureState.OnDamaged:
                break;
            case Define.ECreatureState.Dead:
                OnDead();
                break;
            default:
                break;
        }
    }
    
    protected override void Scanning()
    {
        base.Scanning();
        if(_hasAttackAnimClip)
            StartCoroutine(CoScanning());
        else
        {
            //TODO AUTO MOVING
        }
    }

    protected override void OnDead()
    {
        base.OnDead();
        var dropItem = Managers.Object.Spawn<DropItemController>(transform.position, CreatureData.DropItemId);
        Vector2 ran = new Vector2(transform.position.x + Random.Range( -10, -15) * 0.1f, transform.position.y);
        Vector2 ran2 = new Vector2(transform.position.x + Random.Range( 10, 15) * 0.1f, transform.position.y);
        Vector2 dropPos = Random.value < 0.5 ? ran : ran2;
        // Vector2 DropPos = new Vector2(1f, transform.position.y);
        dropItem.SetInfo(CreatureData.DropItemId, dropPos);
        StartCoroutine(CoOndead());
    }

    IEnumerator CoOndead()
    {
        yield return new WaitForSeconds(0.4f);
        Managers.Object.Despawn(this);
    }

    protected override void MoveAndAttack(InteractionObject target)
    {
        base.MoveAndAttack(target);
    }
    
    // private void OnDrawGizmos()
    // {
    //     string label = string.Format("({0})", CreatureState);
    //     GUIStyle style = new GUIStyle();
    //     style.normal.textColor = Color.blue;
    //     style.fontSize = 15;
    //     Handles.Label(transform.position + Vector3.down * 0.3f, label, style);
    // }
}
