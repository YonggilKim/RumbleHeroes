using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class CreatureController : InteractionObject
{
    protected Rigidbody2D _rigidBody { get; set; }
    #region Stat

    public Data.CreatureData CreatureData { get; private set; }

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
    public virtual SkillBook Skills { get; set; }
    
    #endregion
    
    public InteractionObject InteractingTarget { get; set; }
    private Define.ECreatureState _creatureState = Define.ECreatureState.Moving;
    public virtual Define.ECreatureState CreatureState
    {
        get => _creatureState;
        set
        {
            if (ObjectType == Define.EObjectType.Hero)
            {
                // Debug.Log($"Hero State : {value}");
            }

            _creatureState = value;
            UpdateAnimation();
        }
    }

    protected Coroutine MoveCoroutine;
    private void Awake()
    {
        Init();
    }

    protected override bool Init()
    {
        base.Init();

        Skills = gameObject.GetOrAddComponent<SkillBook>();
        _rigidBody = gameObject.GetComponent<Rigidbody2D>() ;

        CurrentSprite = GetComponent<SpriteRenderer>();
        
        if (CurrentSprite == null)
            CurrentSprite = Util.FindChild<SpriteRenderer>(gameObject);

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

        
        Init();
        InitSkill();
    }

    public virtual void InitCreatureStat(bool isFullHp = true)
    {
        MaxHp = CreatureData.MaxHp;
        Atk = CreatureData.Atk * CreatureData.AtkRate;
        Hp = MaxHp;
        MoveSpeed = CreatureData.MoveSpeed * CreatureData.MoveSpeedRate;
    }
    
    public virtual void InitSkill()
    {
        foreach (int skillId in CreatureData.SkillIdList)
        {
             Skills.AddSkill(skillId);
        }
    }
    
    protected virtual void UpdateAnimation()
    {
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;;
        switch (CreatureState)
        {
            case Define.ECreatureState.Idle:
                Anim.Play("Idle");
                Scanning();
                break;
            case Define.ECreatureState.Attack:
                _rigidBody.constraints = RigidbodyConstraints2D.None; 
                _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation; 
                break;
            case Define.ECreatureState.Moving:
                Anim.Play("Move");
                _rigidBody.constraints = RigidbodyConstraints2D.None; 
                _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation; 
                break;
            case Define.ECreatureState.Gathering:
                Anim.Play("Gathering");
                break;
            case Define.ECreatureState.OnDamaged:
                Anim.Play("OnDamaged");
                break;
            case Define.ECreatureState.Dead:
                Anim.Play("Die");
                _rigidBody.simulated = false;
                break;
            default:
                break;
        }
    }
    
    protected virtual void MoveAndAttack(InteractionObject target)
    {
        MoveCoroutine = null;
        MoveCoroutine = StartCoroutine(CoMove(target, false, () =>
        {
            // 이동이 완료되면 공격
            CreatureState = Define.ECreatureState.Attack;
            Skills.BaseAttackSkill.DoSkill();
        }));
    }
    
    protected IEnumerator CoMove(InteractionObject target,bool isGathering, Action callback = null)
    {
        yield return new WaitForFixedUpdate();
        CreatureState = Define.ECreatureState.Moving;
        float elapsed = 0;
        
        while (true)
        {
            elapsed += Time.deltaTime;
            if(target.IsValid() == false)
                break;
        
            if (ObjectType == Define.EObjectType.Hero)
            {
                if(target.ObjectType == Define.EObjectType.Hero)
                    Debug.Log("CoMove");
                
            }
            float dist = Vector3.Distance(CenterPosition, target.CenterPosition);
            float stopDist = isGathering ? 1f : CreatureData.AtkRange;
        
            if (dist <= stopDist)
                break;
            Vector2 position = _rigidBody.position;
            Vector2 dirVec = target.CenterPosition - CenterPosition;
            CurrentSprite.flipX = !(dirVec.x < 0);
            Vector2 nextVec = dirVec.normalized * (MoveSpeed * Time.fixedDeltaTime);
        
            _rigidBody.MovePosition(position + nextVec);
        
            yield return null;
        }
        
        InteractingTarget = target;
        if (ObjectType == Define.EObjectType.Hero)
        {
            Debug.Log("이동 완료");
        }

        yield return new WaitForSeconds(0.2f);

        
        callback?.Invoke();
    }

    protected void StopMoveCoroutine()
    {
        if (MoveCoroutine != null)
        {
            StopCoroutine(MoveCoroutine);
            MoveCoroutine = null; 
        }
    }
   
    protected virtual void Scanning() { }

    protected virtual void OnDead()
    {
    }

    public void OnAttackAnimationEvent()
    {
        if(InteractingTarget.IsValid())
            InteractingTarget.OnDamaged(this);
        else
        {
            CreatureState = Define.ECreatureState.Idle;
        }
    }

    public void OnAttackAnimationEndEvent()
    {
        if(InteractingTarget.IsValid() == false)
        {
            SetIdleStateAndScanning();
            return;
        }

        if (ObjectType == Define.EObjectType.Monster)
        {
            float dist = Vector3.Distance(CenterPosition, InteractingTarget.CenterPosition);
            float stopDistance = (ColliderRadius + InteractingTarget.ColliderRadius) + 0.1f;
            
            if (dist > stopDistance)
            {
                SetIdleStateAndScanning();
            }
        }
    }
    
    private void SetIdleStateAndScanning()
    {
        CreatureState = Define.ECreatureState.Idle;
        InteractingTarget = null;
    }
    
    public override void OnDamaged(InteractionObject Attacker)
    {
        base.OnDamaged(Attacker);
        
        if(ObjectType == Define.EObjectType.Hero)
            return;
        CreatureController creature = Attacker as CreatureController;
        if (creature)
        {
            Hp = Mathf.Clamp(Hp-creature.Atk, 0, MaxHp);
            if (Hp == 0)
            {
                CreatureState = Define.ECreatureState.Dead;
            }
        }

    }

    private void OnDrawGizmos()
    {
        string label = string.Format("({0})", CreatureState);
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.blue;
        style.fontSize = 15;
        Handles.Label(transform.position + Vector3.down * 0.3f, label, style);
        
        if (InteractingTarget)
        {
            Gizmos.color = Color.red; // 선의 색상 설정
            Gizmos.DrawLine(CenterPosition, InteractingTarget.CenterPosition); // 시작점에서 끝점까지 선 그리기
        }
    }

}
