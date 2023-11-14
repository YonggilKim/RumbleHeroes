using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedArcAttack : StandardAttack
{

    public override void OnAnimationEvent(int param)
    {
        Define.EAnimationState animState = (Define.EAnimationState)param;
        switch (animState)
        {
            case Define.EAnimationState.Attack:
                if (Owner.InteractingTarget.IsValid())
                {
                    if (Owner.InteractingTarget.ObjectType != Define.EObjectType.GatheringResources)
                    {
                        // 상대방에게 발사
                        var proj = Managers.Object.Spawn<RangeArcProjectile>(Owner.CenterPosition, prefabName: SkillData.PrefabLabel);
                        proj.SetInfo(Owner, this);
                    }
                    else
                    {
                        Owner.OnAttackAnimationEvent();
                    }
                }
                else
                {
                    Owner.CreatureState = Define.ECreatureState.Idle;
                }



                break;
            case Define.EAnimationState.End:
                Owner.OnAttackAnimationEndEvent();
                break;
            default:
                break;
        }
    }
}
