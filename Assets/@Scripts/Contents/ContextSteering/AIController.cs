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
    private const float ATTACK_DISTANCE = 0.5f;

    //Inputs sent from the Enemy AI to the Enemy controller
    public UnityEvent OnAttackPressed;
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

    private void Start()
    {
        //Detecting Player and Obstacles around
        InvokeRepeating("PerformDetection", 0, DETECTION_DELAY);
    }

    public void SetInfo(CreatureController owner)
    {
        _owner = owner;
        
        GameObject AIContainer = Managers.Resource.Instantiate("AIContainer", gameObject.transform);
        
        _aiData = Util.GetOrAddComponent<AIData>(gameObject);
        SetDetector<TargetDetector>(AIContainer);
        SetDetector<ObstacleDetector>(AIContainer);
        
        SetBehaviour<ObstacleAvoidanceBehaviour>(AIContainer);
        SetBehaviour<SeekBehaviour>(AIContainer);
      
        _movementDirectionSolver = Util.FindChild<ContextSolver>(gameObject, recursive: true);
    }

    private void PerformDetection()
    {
        foreach (Detector detector in _detectors)
        {
            detector.Detect(_aiData);
        }
    }

    private void Update()
    {
        if(_aiData == null) return;
        //Enemy AI movement based on Target availability
        if (_aiData.currentTarget != null)
        {
            //Looking at the Target
            if (_following == false)
            {
                _following = true;
                StartCoroutine(ChaseAndAttack());
            }
        }
        else if (_aiData.GetTargetsCount() > 0)
        {
            //Target acquisition logic
            _aiData.currentTarget = _aiData.targets[0];
        }
    }

    private IEnumerator ChaseAndAttack()
    {
        if (_aiData.currentTarget == null)
        {
            //Stopping Logic
            Debug.Log("@>> AICtrl : Stopping");
            MovementInput = Vector2.zero;
            _following = false;
            yield break;
        }
        else
        {
            float distance = Vector2.Distance(_aiData.currentTarget.CenterPosition, transform.position);

            if (distance < ATTACK_DISTANCE)
            {
                //Attack logic
                MovementInput = Vector2.zero;
                OnAttackPressed?.Invoke();
                yield return new WaitForSeconds(ATTACK_DELAY);
                StartCoroutine(ChaseAndAttack());
            }
            else
            {
                //Chase logic
                MovementInput = _movementDirectionSolver.GetDirectionToMove(_steeringBehaviours, _aiData);
                yield return new WaitForSeconds(AI_UPDATE_DELAY);
                StartCoroutine(ChaseAndAttack());
            }

        }

    }
    
    private void SetDetector<T>(GameObject parent) where T : Detector
    {
        T detector = Util.FindChild<T>(parent, recursive: true);
        detector.transform.position = _owner.CenterPosition;
        detector.Owner = _owner;
        _detectors.Add(detector);
    }

    private void SetBehaviour<T>(GameObject parent) where T : SteeringBehaviour
    {
        T behaviour = Util.FindChild<T>(parent, recursive: true);
        behaviour.Owner = _owner;
        behaviour.transform.position = _owner.CenterPosition;
        _steeringBehaviours.Add(behaviour);
    }

}