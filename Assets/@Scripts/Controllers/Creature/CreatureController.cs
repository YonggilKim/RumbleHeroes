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

    public InteractionObject InteractingTarget { get; set; }
    private Define.ECreatureState _creatureState = Define.ECreatureState.Moving;

    public virtual Define.ECreatureState CreatureState
    {
        get => _creatureState;
        set
        {
            if (ObjectType == Define.EObjectType.Hero)
            {
                Debug.Log($"Hero State : {value}");
            }
            _creatureState = value;
            UpdateAnimation();
        }
    }

    //
    protected Coroutine MoveCoroutine;
    protected Coroutine ScanningCoroutine;
  
    private Vector3 _gatheringPoint;
    public Vector2 GatheringPoint
    {
        get => _gatheringPoint;
        set => _gatheringPoint = value;
    }

    protected AIController _aiController;

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
            Agent agent = Util.GetOrAddComponent<Agent>(gameObject);
            agent.SetInfo(this);
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
                Anim.Play("Gathering");
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

    protected IEnumerator CoScanning()
    {
        yield return new WaitForEndOfFrame();

        while (CreatureState == Define.ECreatureState.Idle)
        {
            InteractionObject target;
            if (ObjectType == Define.EObjectType.Hero)
            {
                InteractingTarget = Managers.Object.GetInteracctionTarget(this, Vector3.zero);
            }
            else
            {
                InteractingTarget = Managers.Object.GetInteracctionTarget(this, CenterPosition);
            }

            if (InteractingTarget.IsValid())
            {
                MoveAndAttack(InteractingTarget);
                StopScanningCoroutine();
                yield break;
            }
            else
            {
                if (ObjectType == Define.EObjectType.Hero)
                {
                    if (Vector3.Distance(CenterPosition, Managers.Game.Leader.GatheringPoint) > 3)
                    {
                        CreatureState = Define.ECreatureState.Gathering;
                        StopScanningCoroutine();
                        yield break;
                    }
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    protected virtual void MoveAndAttack(InteractionObject target)
    {
        GhostMode(true);
        MoveCoroutine = null;
        MoveCoroutine = StartCoroutine(CoMove(false, () =>
        {
            GhostMode(false);
            // 이동이 완료되면 공격
            if (InteractingTarget.IsValid())
            {
                CurrentCollider.radius = ColliderRadius;
                CreatureState = Define.ECreatureState.Attack;
                Skills.BaseAttackSkill.DoSkill();
            }
            else
            {
                //진행중 타겟이 없어지면 Idle로 리셋
                CreatureState = Define.ECreatureState.Idle;
            }

            StopMoveCoroutine();
        }));
    }

    protected virtual void Gathering()
    {
        GhostMode(true);
        if (MoveCoroutine == null)
            MoveCoroutine = StartCoroutine(CoMove(true, () =>
            {
                GhostMode(false);
                // 다모이면
                CreatureState = Define.ECreatureState.Idle;
                StopMoveCoroutine();
            }));
    }

    protected IEnumerator CoMove(bool isGathering, Action callback = null)
    {
        yield return new WaitForFixedUpdate();
        CreatureState = Define.ECreatureState.Moving;
        float elapsed = 0;

        Vector2 gatheringPos = Managers.Game.Leader.GatheringPoint;

        while (true)
        {
            Vector3 targetPos;

            if (isGathering)
            {
                targetPos = ObjectType == Define.EObjectType.Hero
                    ? gatheringPos
                    : InteractingTarget.CenterPosition;
            }
            else
            {
                targetPos = InteractingTarget.CenterPosition;
            }

            elapsed += Time.deltaTime;
            if (InteractingTarget.IsValid() == false && isGathering == false)
                break;

            float dist = Vector3.Distance(CenterPosition, targetPos);
            float stopDist = isGathering ? 2.5f : CreatureData.AtkRange;


            if (InteractingTarget && InteractingTarget.ObjectType == Define.EObjectType.GatheringResources)
                stopDist = 1.5f;

            if (dist <= stopDist)
                break;
            Vector2 position = _rigidBody.position;
            Vector2 dirVec = targetPos - CenterPosition;
            CurrentSprite.flipX = !(dirVec.x < 0);
            Vector2 nextVec = dirVec.normalized * (Attribute.MoveSpeed.CurrentValue * Time.fixedDeltaTime);

            _rigidBody.MovePosition(position + nextVec);

            yield return null;
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

    protected virtual void Scanning(){ }

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

    //Animation에서 받는 이벤트 
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

    private void SetIdleStateAndScanning()
    {
        CreatureState = Define.ECreatureState.Idle;
        InteractingTarget = null;
    }

    protected void GhostMode(bool isOn)
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

    protected void StopScanningCoroutine()
    {
        if (ScanningCoroutine != null)
        {
            StopCoroutine(ScanningCoroutine);
            ScanningCoroutine = null;
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