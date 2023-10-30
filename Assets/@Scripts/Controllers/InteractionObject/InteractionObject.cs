using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObject : BaseController
{
    [SerializeField]
    // ReSharper disable once InconsistentNaming
    protected SpriteRenderer CurrentSprite;
    protected string SpriteName;
    protected Animator Anim { get; set; }
    public Material DefaultMat;
    public Material HitEffectMat;

    public Data.CreatureData CreatureData;
    public virtual int DataId { get; set; }
    public virtual float Hp { get; set; }
    public virtual float MaxHp { get; set; }
    
    protected override bool Init()
    {
        base.Init();
        CurrentSprite = gameObject.GetOrAddComponent<SpriteRenderer>();
        
        Hp = 3;
        MaxHp = 3;
        
        return true;
    }
    
    public override void OnDamaged(BaseController Attacker)
    {
        base.OnDamaged(Attacker);
        Hp = Mathf.Clamp(Hp-1, 0, MaxHp);
        if (Hp == 0)
        {
            Depleted();
        }
    }

    public void Depleted()
    {
        //TODO Play Animation
        Managers.Object.Despawn(this);
    }

}
