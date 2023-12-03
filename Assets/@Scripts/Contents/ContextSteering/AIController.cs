using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class AIController : MonoBehaviour
{
    private readonly List<SteeringBehaviour> _steeringBehaviours = new List<SteeringBehaviour>();
    private readonly List<Detector> _detectors = new List<Detector>();

    public AIData AIData;
    [SerializeField] 
    private float DETECTION_DELAY = 0.15f;
    [SerializeField] 
    private float AI_UPDATE_DELAY = 0.06f;

    private const float ATTACK_DELAY = 1f;
    private float AttackRange = 0.5f;

    public Action OnAttack;
    public Action<Vector2> OnChangedMovementInput;

    private Vector2 _movementInput;

    public Vector2 MovementInput
    {
        get => _movementInput;
        set
        {
            _movementInput = value;
            OnChangedMovementInput?.Invoke(value);
        }
    }

    private ContextSolver _movementDirectionSolver;

    private bool _isFollowing = false;
    private CreatureController _owner;
    private bool _isAutoMode = false;
    private Coroutine detectionCoroutine;
    private Coroutine ChaseAndAttackCoroutine;

    public bool IsAutoMode
    {
        get => _isAutoMode;
        set
        {
            _isAutoMode = value;
            if (_isAutoMode)
            {
                if (detectionCoroutine == null)
                {
                    detectionCoroutine = StartCoroutine(CoPerformDetection());
                }
            }
            else
            {
                if (detectionCoroutine != null)
                {
                    StopCoroutine(detectionCoroutine);
                    detectionCoroutine = null;
                    
                    //aidata reset
                    AIData.targets = null;
                    AIData.obstacles = null;
                    AIData.currentTarget = null;
                }

                if (ChaseAndAttackCoroutine != null)
                {
                    StopCoroutine(ChaseAndAttackCoroutine);
                    _owner.CreatureState = Define.ECreatureState.Idle;
                    _isFollowing = false;
                    ChaseAndAttackCoroutine = null;
                    
                    //aidata reset
                    AIData.targets = null;
                    AIData.obstacles = null;
                    AIData.currentTarget = null;
                }

            }
        }
    }
    
    private void Start()
    {
        IsAutoMode = _owner.ObjectType != Define.EObjectType.Hero;
    }

    public void SetInfo(CreatureController owner)
    {
        _owner = owner;

        IsAutoMode = _owner.ObjectType != Define.EObjectType.Hero;
        AttackRange = _owner.CreatureData.AtkRange;

        GameObject AIContainer = Managers.Resource.Instantiate("AIContainer", gameObject.transform);

        AIData = Util.GetOrAddComponent<AIData>(gameObject);
        TargetDetector td = SetDetector<TargetDetector>(AIContainer);
        ObstacleDetector od = SetDetector<ObstacleDetector>(AIContainer);

        SetBehaviour<ObstacleAvoidanceBehaviour>(AIContainer);
        SetBehaviour<SeekBehaviour>(AIContainer);

        _movementDirectionSolver = Util.FindChild<ContextSolver>(gameObject, recursive: true);
        _movementDirectionSolver.transform.position = owner.CenterPosition;
    }

    private IEnumerator CoPerformDetection()
    {
        WaitForSeconds wait = new WaitForSeconds(DETECTION_DELAY);
        yield return wait;

        while (true)
        {
             foreach (Detector detector in _detectors)
            {
                detector.Detect(AIData);
            }

            yield return wait;
        }
    }

    private void Update()
    {
        if (AIData == null) return;
        if(_isAutoMode == false) return;
        if (IsValidTarget(AIData.currentTarget))
        {
            if (_isFollowing != false) return;
            if (ChaseAndAttackCoroutine == null)
            {
                _isFollowing = true;
                ChaseAndAttackCoroutine = StartCoroutine(CoChaseAndAttack());
            }
        }
        else if (AIData.GetTargetsCount() > 0)
        {
            AIData.currentTarget = AIData.targets[0];
            if (AIData.currentTarget == Managers.Object.GatherPoint && ChaseAndAttackCoroutine == null)
            {
                _isFollowing = true;
                ChaseAndAttackCoroutine = StartCoroutine(CoChaseAndAttack());
            }
        }
    }
    
    //add gathering
    private IEnumerator CoChaseAndAttack()
    {
        WaitForSeconds waitAttack = new WaitForSeconds(ATTACK_DELAY);
        WaitForSeconds waitADelay = new WaitForSeconds(AI_UPDATE_DELAY);

        while (true)
        {
            if (_owner.ObjectType == Define.EObjectType.Hero)
            {
                Debug.Log("");
            }

            if (AIData.currentTarget.IsValid() == false && AIData.currentTarget != Managers.Object.GatherPoint)
            {
                SetIdle();
                ChaseAndAttackCoroutine = null;
                yield break;
            }
            else if (AIData.currentTarget == Managers.Object.GatherPoint)
            {
                //Hero들이 다시 집합장소로 모임
                float distance = Vector2.Distance(AIData.currentTarget.CenterPosition, _owner.CenterPosition);

                if (distance < 2f)
                {
                    SetIdle();
                    ChaseAndAttackCoroutine = null;
                    yield break;
                }
                else
                {
                    // 추격 로직
                    _owner.CreatureState = Define.ECreatureState.Gathering;
                    MovementInput = _movementDirectionSolver.GetDirectionToMove(_steeringBehaviours, AIData);
                    yield return waitADelay;
                }
            }
            else
            {
                float distance = Vector2.Distance(AIData.currentTarget.CenterPosition, _owner.CenterPosition);

                if (distance < AttackRange)
                {
                    // 공격 로직
                    MovementInput = Vector2.zero;
                    _owner.CreatureState = Define.ECreatureState.Attack;
                    OnAttack?.Invoke();
                    yield return waitAttack;
                }
                else
                {
                    if (Managers.Game.JoystickState == Define.EJoystickState.Dragging &&
                        _owner.ObjectType == Define.EObjectType.Hero)
                    {
                        Debug.LogError("추격");
                    }

                    // 추격 로직
                    MovementInput = _movementDirectionSolver.GetDirectionToMove(_steeringBehaviours, AIData);
                    yield return waitADelay;
                }
            }

            yield return null;
        }
    }

    private void SetIdle()
    {
        MovementInput = Vector2.zero;
        _owner.CreatureState = Define.ECreatureState.Idle;
        _isFollowing = false;
    }

    private T SetDetector<T>(GameObject parent) where T : Detector
    {
        T detector = Util.FindChild<T>(parent, recursive: true);
        detector.transform.position = _owner.CenterPosition;
        detector.Owner = _owner;
        _detectors.Add(detector);
        return (T)detector;
    }

    private T SetBehaviour<T>(GameObject parent) where T : SteeringBehaviour
    {
        T behaviour = Util.FindChild<T>(parent, recursive: true);
        behaviour.Owner = _owner;
        behaviour.transform.position = _owner.CenterPosition;
        _steeringBehaviours.Add(behaviour);
        return (T)behaviour;
    }

    private bool IsValidTarget(InteractionObject interactionObject)
    {
        // if (interactionObject == Managers.Object.GatherPoint)
        //     return true;

        if (interactionObject == null || interactionObject.isActiveAndEnabled == false)
            return false;

        if (interactionObject.Attribute == null)
            return false;

        if (interactionObject.Attribute.Hp.CurrentValue == 0)
            return false;

        return interactionObject != null && interactionObject.isActiveAndEnabled;
    }
}