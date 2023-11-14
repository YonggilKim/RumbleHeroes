using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeArcProjectile : BaseController
{
    private SpriteRenderer _pojectileSprite;
    // Owner?
    CreatureController _owner;
    public SkillBase Skill;
    private CurveShotComponent _comp;
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = Define.EObjectType.Projectile;
        _comp = gameObject.GetOrAddComponent<CurveShotComponent>();
        _pojectileSprite = GetComponent<SpriteRenderer>();
        return true;
    }

    public void SetInfo(CreatureController owner, SkillBase skill)
    {
        _owner = owner;
        Skill = skill;

        _pojectileSprite.sprite = Managers.Resource.Load<Sprite>($"{owner.CreatureData.PrefabLabel}_Bullet.sprite");

        // if (owner.InteractingTarget.IsValid())
        {
            _comp.Shot(transform.position, owner.InteractingTarget.CenterPosition, 5f, EndCallback: () =>
            {
                _owner.OnAttackAnimationEvent();
                Managers.Object.Despawn(this);
            });
        }
        // else
        // {
        //     Managers.Object.Despawn(this);
        // }

    }
   
}
