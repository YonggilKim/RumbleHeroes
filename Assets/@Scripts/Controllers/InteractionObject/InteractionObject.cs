using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObject : BaseController
{
    [SerializeField]
    // ReSharper disable once InconsistentNaming
    protected SpriteRenderer CurrentSprite;
    protected string SpriteName;
    public Animator Anim { get; set; }

    public Vector3 CenterPosition => transform.position + Vector3.up * ColliderRadius;
    public float ColliderRadius { get; set; }
    protected DamageFlash DamageFlashComp;
    
    public virtual int DataId { get; set; }
    public virtual float Hp { get; set; }
    public virtual float MaxHp { get; set; }
    
    protected override bool Init()
    {
        base.Init();
        CurrentSprite = gameObject.GetOrAddComponent<SpriteRenderer>();
        ColliderRadius = gameObject.GetOrAddComponent<CircleCollider2D>().radius;
        DamageFlashComp = gameObject.GetOrAddComponent<DamageFlash>();
        Anim = GetComponent<Animator>();
        if (Anim == null)
            Anim = Util.FindChild<Animator>(gameObject);

        return true;
    }
    
    
    public virtual void OnDamaged(InteractionObject Attacker)
    {
        DamageFlashComp.CallDamageFlash();
    }

    public virtual void Despawn()
    {
        //TODO Play Animation
        Managers.Object.Despawn(this);
    }

}
