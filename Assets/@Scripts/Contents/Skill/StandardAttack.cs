using System;
using UnityEngine;

public class StandardAttack : SkillBase
{
    public override void DoSkill(Action callback = null)
    {
        if (Owner.CreatureState != Define.ECreatureState.Attack)
            return;
        Owner.Anim.Play("Attack");
    }

    public override void OnAnimationEvent(int param)
    {
        Define.EAnimationState animState = (Define.EAnimationState)param;
        switch (animState)
        {
            case Define.EAnimationState.Attack:
                Owner.OnAttackAnimationEvent();
                break;
            case Define.EAnimationState.End:
                Owner.OnAttackAnimationEndEvent();
                break;
            default:
                break;
        }
    }
}