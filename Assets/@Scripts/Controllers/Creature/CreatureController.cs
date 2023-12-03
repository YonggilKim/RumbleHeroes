using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CreatureController : InteractionObject
{
    protected Rigidbody2D _rigidBody { get; set; }

    #region Stat
    public Data.CreatureData CreatureData { get; private set; }
    public SkillBook Skills { get; set; }
    #endregion
    public InteractionObject InteractingTarget
    {
        get => _aiController.AIData.currentTarget;
    }
    private Define.ECreatureState _creatureState = Define.ECreatureState.Moving;
    public Define.ECreatureState CreatureState
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
    public CreatureMovement CreatureMovementMovement { get; set; }
    protected Coroutine MoveCoroutine;
    protected Coroutine ScanningCoroutine;
  
    public AIController _aiController;

    protected override bool Init()
    {
        base.Init();

        Skills = gameObject.GetOrAddComponent<SkillBook>();
        _rigidBody = gameObject.GetComponent<Rigidbody2D>();

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
        CurrentSprite.sprite = sprite;

        {
            // Add AI
            _aiController = Util.GetOrAddComponent<AIController>(gameObject);
            _aiController.SetInfo(this);
            //TODO 비활성화되면 OnAttackHandler 호출 X?
            _aiController.OnAttack += OnAttackHandler;
            CreatureMovementMovement = Util.GetOrAddComponent<CreatureMovement>(gameObject);
            CreatureMovementMovement.SetInfo(this);
        }

        SetSkill();
    }

    public virtual void InitCreatureStat(bool isFullHp = true)
    {
        float moveSpeed  = CreatureData.MoveSpeed * CreatureData.MoveSpeedRate;
        
        Attribute = gameObject.AddComponent<AttributeSet>();
        Attribute.MaxHp.BaseValue = CreatureData.MaxHp;
        Attribute.MaxHp.CurrentValue = CreatureData.MaxHp;
        Attribute.Hp.BaseValue = CreatureData.MaxHp;
        Attribute.Hp.CurrentValue = CreatureData.MaxHp;
        Attribute.Atk.BaseValue = CreatureData.Atk;
        Attribute.Atk.CurrentValue = CreatureData.Atk;
        Attribute.MoveSpeed.BaseValue = moveSpeed;
        Attribute.MoveSpeed.CurrentValue = moveSpeed;
    }

    public virtual void SetSkill()
    {
        foreach (int skillId in CreatureData.SkillIdList)
        {
            Skills.AddSkill(skillId);
        }
    }

    protected virtual void UpdateAnimation()
    {
        // _rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        ;
        switch (CreatureState)
        {
            case Define.ECreatureState.Idle:
                Anim.Play("Idle");
                // Scanning();
                break;
            case Define.ECreatureState.Attack:
                // _rigidBody.constraints = RigidbodyConstraints2D.None;
                // _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
                break;
            case Define.ECreatureState.Moving:
                Anim.Play("Move");
                // _rigidBody.constraints = RigidbodyConstraints2D.None;
                // _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
                break;
            case Define.ECreatureState.Gathering:
                Anim.Play("Move");
                // Gathering();
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

    protected virtual void OnDead()
    {
    }

    private void OnAttackHandler()
    {
        //Attack
        CreatureState = Define.ECreatureState.Attack;
        Skills.BaseAttackSkill.DoSkill();
    }
    
    public void OnAttackAnimationEvent()
    {
        if (InteractingTarget.IsValid())
            InteractingTarget.OnDamaged(this);
    }

    public void OnAttackAnimationEndEvent()
    {
        CreatureState = Define.ECreatureState.Idle;

        // if (InteractingTarget.IsValid() == false)
        // {
        //     SetIdleStateAndScanning();
        //     return;
        // }
        //
        // if (ObjectType == Define.EObjectType.Monster)
        // {
        //     float dist = Vector3.Distance(CenterPosition, InteractingTarget.CenterPosition);
        //     float stopDistance = (ColliderRadius + InteractingTarget.ColliderRadius) + 0.1f;
        //
        //     if (dist > stopDistance)
        //     {
        //         SetIdleStateAndScanning();
        //     }
        // }
    }

    public void GhostMode(bool isOn)
    {
        if (isOn)
        {
            CurrentCollider.radius = ColliderRadius * 0.1f;
        }
        else
        {
            CurrentCollider.radius = ColliderRadius;
        }
    }

    public override void OnDamaged(InteractionObject Attacker)
    {
        base.OnDamaged(Attacker);

        if (ObjectType == Define.EObjectType.Hero)
            return;
        CreatureController creature = Attacker as CreatureController;
        if (creature)
        {
            Attribute.Hp.CurrentValue = Mathf.Clamp(Attribute.Hp.CurrentValue - creature.Attribute.Atk.CurrentValue, 0, Attribute.MaxHp.CurrentValue);
            if (Attribute.Hp.CurrentValue == 0)
            {
                CreatureState = Define.ECreatureState.Dead;
            }
        }
    }

#if UnityEditor
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
#endif
}