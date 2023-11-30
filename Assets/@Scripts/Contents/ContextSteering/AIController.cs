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

    private AIData _aiData;
    
    private const float DETECTION_DELAY = 0.05f;
    private const float AI_UPDATE_DELAY = 0.06f;
    
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

    private bool _following = false;
    private CreatureController _owner;
    private bool _isAutoMode = false;
    private Coroutine detectionCoroutine;

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
                    _aiData.currentTarget = null;
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
        
        _aiData = Util.GetOrAddComponent<AIData>(gameObject);
        TargetDetector td = SetDetector<TargetDetector>(AIContainer);
        ObstacleDetector od = SetDetector<ObstacleDetector>(AIContainer);
        SetBehaviour<ObstacleAvoidanceBehaviour>(AIContainer);
        SetBehaviour<SeekBehaviour>(AIContainer);
      
        _movementDirectionSolver = Util.FindChild<ContextSolver>(gameObject, recursive: true);
        _movementDirectionSolver.transform.position = owner.CenterPosition;
    }

    private IEnumerator CoPerformDetection()
    {
        WaitForSeconds wait =  new WaitForSeconds(DETECTION_DELAY); 
        yield return wait;

        while (true)
        {
            // if (_owner.ObjectType == Define.EObjectType.Hero)
            // {
            //     Debug.Log("");
            // }
           
            foreach (Detector detector in _detectors)
            {
                detector.Detect(_aiData);
            }
            yield return wait;
        }
    }

    private void Update()
    {
        if(_aiData == null) return;
        if (_aiData.currentTarget.IsValid())
        {
            if (_following == false)
            {
                _following = true;
                StartCoroutine(CoChaseAndAttack());
            }
        }
        else if (_aiData.GetTargetsCount() > 0)
        {
            _aiData.currentTarget = _aiData.targets[0];
        }
    }

    private IEnumerator CoChaseAndAttack()
    {
        WaitForSeconds waitAttack = new WaitForSeconds(ATTACK_DELAY);
        WaitForSeconds waitADelay = new WaitForSeconds(AI_UPDATE_DELAY);
        
        while (true)
        {
            if (_aiData.currentTarget.IsValid() == false)
            {
                MovementInput = Vector2.zero;
                _owner.InteractingTarget = null;
                _following = false;            
                yield break;
                // _owner.CreatureState = Define.ECreatureState.Idle;
            }
            else
            {
                float distance = Vector2.Distance(_aiData.currentTarget.CenterPosition, _owner.CenterPosition);

                if (distance < AttackRange)
                {
                    // 공격 로직
                    MovementInput = Vector2.zero;
                    _owner.InteractingTarget = _aiData.currentTarget;
                    OnAttack?.Invoke();
                    yield return waitAttack;
                }
                else
                {
                    // 추격 로직
                    MovementInput = _movementDirectionSolver.GetDirectionToMove(_steeringBehaviours, _aiData);
                    yield return waitADelay;
                }
            }
            yield return null;
        }
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

}