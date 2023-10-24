using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class CreatureController : BaseController
{
    public Rigidbody2D _rigidBody { get; set; }

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
    protected SpriteRenderer CurrentSprite;
    protected string SpriteName;
    protected Animator Anim { get; set; }
    
    public BaseController InteractTarget { get; set; }
    private Define.ECreatureState _creatureState = Define.ECreatureState.Moving;
    public virtual Define.ECreatureState CreatureState
    {
        get => _creatureState;
        set
        {
            _creatureState = value;
            UpdateAnimation();
            Debug.Log($"{value}");
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

        CurrentSprite = GetComponent<SpriteRenderer>();
        
        if (CurrentSprite == null)
            CurrentSprite = Util.FindChild<SpriteRenderer>(gameObject);

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
        CurrentSprite.sprite = Managers.Resource.Load<Sprite>(CreatureData.SpriteName);
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

    protected IEnumerator CoAttack(BaseController target)
    {
        yield return null;
        // 1. 목적지 까지 이동
        yield return StartCoroutine(CoMove(target, () =>
        {   
            Debug.Log("이동 완료");
            // 이동이 완료되면 공격
            InteractTarget = target;
            CreatureState = Define.ECreatureState.Attack;
        }));
    }
    
    protected IEnumerator CoMove(BaseController target, Action callback = null)
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        CreatureState = Define.ECreatureState.Moving;
        float elapsed = 0;

        while (true)
        {
            elapsed += Time.deltaTime;
            if(target.IsValid() == false)
                break;
            if (elapsed > 3.0f)
                break;

            // Vector3 dir = (CenterPosition - target.CenterPosition).normalized;
            // Vector2 targetPosition = CenterPosition + dir * UnityEngine.Random.Range(SkillData.MinCoverage, SkillData.MaxCoverage);
            float stopDistance = (ColliderRadius + target.ColliderRadius) * 1.2f; 
            if (Vector3.Distance(_rigidBody.position, target.CenterPosition) <= stopDistance)
                continue;

            Vector2 position = _rigidBody.position;
            Vector2 dirVec = (Vector2)target.CenterPosition - position;
            CurrentSprite.flipX = !(dirVec.x < 0);
            
            Vector2 nextVec = dirVec.normalized * (MoveSpeed * Time.fixedDeltaTime);
            _rigidBody.MovePosition(position + nextVec);

            yield return null;
        }
        callback?.Invoke();
    }

    protected virtual void UpdateAnimation()
    {
        switch (CreatureState)
        {
            case Define.ECreatureState.Idle:
                Anim.Play("Idle");
                AutoWorking();
                break;
            case Define.ECreatureState.Attack:
                Anim.Play("Attack");
                break;
            case Define.ECreatureState.Moving:
                Anim.Play("Move");
                break;
            case Define.ECreatureState.Gathering:
                Anim.Play("Gathering");
                break;
            case Define.ECreatureState.OnDamaged:
                Anim.Play("OnDamaged");
                break;
            case Define.ECreatureState.Dead:
                Anim.Play("Die");
                break;
            default:
                break;
        }
    }

    protected virtual void AutoWorking()
    {
        //1. 주변을 검색한다.
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll((Vector2)CenterPosition, 5);

        List<MonsterController> monsters = new List<MonsterController>();
        List<InteractionObject> objects = new List<InteractionObject>();
        
        foreach (var collider in hitColliders)
        {
            MonsterController monster = collider.GetComponent<MonsterController>();
            if(monster)
                monsters.Add(monster);
            
            InteractionObject interactionObject = collider.GetComponent<InteractionObject>();
            if(interactionObject)
                objects.Add(interactionObject);
        }

        if (monsters.Count > 0)
        {
            monsters = monsters.OrderBy(target => (CenterPosition - target.CenterPosition).sqrMagnitude).ToList();
            MonsterController target = monsters[0];
            //Attack
            Attack(target);
        }
        else if(objects.Count > 0)
        {
            objects = objects.OrderBy(target => (CenterPosition - target.CenterPosition).sqrMagnitude).ToList();
            InteractionObject target = objects[0];
            Attack(target);

        }
    }

    protected virtual void Attack(BaseController target)
    {
        StartCoroutine(CoAttack(target));
    }

    public void OnAttackAnimationEvent()
    {
        Debug.Log("Attack");
        if(InteractTarget)
            InteractTarget.OnDamaged(this);
        else
        {
            CreatureState = Define.ECreatureState.Idle;
            AutoWorking();
        }
    }

    public void OnAttackAnimationEndEvent()
    {
        if(InteractTarget.IsValid() == false)
        {
            CreatureState = Define.ECreatureState.Idle;
            AutoWorking();
        }
    }

    public override void OnDamaged(BaseController Attacker)
    {
    }

}
