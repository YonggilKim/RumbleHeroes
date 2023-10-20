using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : BaseController
{
    [SerializeField]
    // ReSharper disable once InconsistentNaming
    protected SpriteRenderer CreatureSprite;
    protected string SpriteName;

    
    public Rigidbody2D _rigidBody { get; set; }
    protected Animator Anim { get; set; }

    #region Stat

    public Data.CreatureData CreatureData;
    public virtual int DataId { get; set; }
    public virtual float Hp { get; set; }
    public virtual float MaxHp { get; set; }
    public virtual float MaxHpBonusRate { get; set; } = 1;
    public virtual float HealBonusRate { get; set; } = 1;
    public virtual float HpRegen { get; set; }
    public virtual float Atk { get; set; }
    public virtual float AttackRate { get; set; } = 1;
    public virtual float Def { get; set; }
    public virtual float DefRate { get; set; }
    public virtual float CriRate { get; set; }
    public virtual float CriDamage { get; set; } = 1.5f;
    public virtual float DamageReduction { get; set; }
    public virtual float MoveSpeedRate { get; set; } = 1;
    public virtual float MoveSpeed { get; set; } = 4;
    // public virtual SkillBook Skills { get; set; }
    
    #endregion
    
    public Vector3 CenterPosition => _offset.bounds.center;
    public float ColliderRadius { get; set; }

    private Collider2D _offset;
    private Define.ECreatureState _creatureState = Define.ECreatureState.Moving;
    public virtual Define.ECreatureState CreatureState
    {
        get => _creatureState;
        set
        {
            _creatureState = value;
            UpdateAnimation();
        }
    }
    
    private void Awake()
    {
        Init();
    }

    protected override bool Init()
    {
        base.Init();

        _rigidBody = GetComponent<Rigidbody2D>();
        _offset = GetComponent<Collider2D>();
        ColliderRadius = GetComponent<CircleCollider2D>().radius;

        CreatureSprite = GetComponent<SpriteRenderer>();
        if (CreatureSprite == null)
            CreatureSprite = Util.FindChild<SpriteRenderer>(gameObject);

        Anim = GetComponent<Animator>();
        if (Anim == null)
            Anim = Util.FindChild<Animator>(gameObject);

        return true;
    }
    
    public void SetInfo(int creatureId)
    {
        DataId = creatureId;
        Dictionary<int, Data.CreatureData> dict = Managers.Data.CreatureDic;
        CreatureData = dict[creatureId];
        InitCreatureStat();
        var sprite = Managers.Resource.Load<Sprite>(CreatureData.SpriteName);
        CreatureSprite.sprite = Managers.Resource.Load<Sprite>(CreatureData.SpriteName);
        var ra = Managers.Resource.Load<RuntimeAnimatorController>(CreatureData.AnimatorName);
        Anim.runtimeAnimatorController = ra;
        Init();
    }

    public virtual void InitCreatureStat(bool isFullHp = true)
    {
        MaxHp = CreatureData.MaxHp;
        Atk = CreatureData.Atk * CreatureData.AtkRate;
        Hp = MaxHp;
        MoveSpeed = CreatureData.MoveSpeed * CreatureData.MoveSpeedRate;
    }
    
    protected virtual void UpdateAnimation() { }

}
