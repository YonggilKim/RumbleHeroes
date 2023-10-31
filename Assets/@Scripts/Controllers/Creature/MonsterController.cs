using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MonsterController : CreatureController
{
    private bool isAggressive { get; set; } = false;

    protected override bool Init()
    {
        base.Init();
        ObjectType = Define.EObjectType.Monster;

        isAggressive = Util.HasAnimationClip(Anim, "Attack");
        
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
        if(isAggressive)
            StartCoroutine(CoScanning());
        else
        {
            //TODO AUTO MOVING
        }
    }

    protected override void OnDead()
    {
        base.OnDead();
        StartCoroutine(CoOndead());
    }

    IEnumerator CoOndead()
    {
        yield return new WaitForSeconds(2f);
        Managers.Object.Despawn(this);
    }

    IEnumerator CoScanning()
    {
        Collider2D[] hitColliders;
        List<HeroController> heros = new List<HeroController>();
        yield return new WaitForFixedUpdate();

        while (CreatureState == Define.ECreatureState.Idle)
        {
            hitColliders = Physics2D.OverlapCircleAll((Vector2)CenterPosition, 5);
            
            foreach (var collider in hitColliders)
            {
                HeroController monster = collider.GetComponent<HeroController>();
                if(monster)
                    heros.Add(monster);
            }

            if (heros.Count > 0)
                break;
            yield return new WaitForSeconds(0.5f);
        }

        heros = heros.OrderBy(target => (CenterPosition - target.CenterPosition).sqrMagnitude).ToList();
        HeroController target = heros[0];
        //Attack
        Attack(target);
    }

    protected override void Attack(BaseController target)
    {
        base.Attack(target);
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