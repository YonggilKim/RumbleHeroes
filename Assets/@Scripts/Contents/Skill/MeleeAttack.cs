
using System;
using System.Collections;
using UnityEngine;

public class MeleeAttack : StandardAttack
{
    private Coroutine _coroutine;
    
    private void Awake()
    {
        SkillType = Define.ESkillType.MeleeAttack;
    }

    public override void DoSkill(Action callback = null)
    {
        base.DoSkill();
    }

}
