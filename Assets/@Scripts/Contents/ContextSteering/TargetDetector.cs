using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : Detector
{
    [SerializeField]
    private LayerMask _obstaclesLayerMask, _targetLayerMask;
    [SerializeField]
    private bool _showGizmos = true;
    private float _targetDetectionRange = 10;
    private List<InteractionObject> _targets;

    private void Start()
    {
        if (Owner.ObjectType == Define.EObjectType.Hero)
        {
            _targetLayerMask =  LayerMask.GetMask("Monster");
            _obstaclesLayerMask = LayerMask.GetMask("Hero");
            _targetDetectionRange = Define.SCAN_RANGE_HERO;
        }
        else
        {
            _targetLayerMask =  LayerMask.GetMask("Hero");
            _obstaclesLayerMask = LayerMask.GetMask("Monster");

            _targetDetectionRange = Define.SCAN_RANGE_MONSTER;
        }
    }

    public override void Detect(AIData aiData)
    {

        InteractionObject target = Managers.Object.GetInteracctionTarget(Owner, Owner.CenterPosition);
        
        if (target.IsValid())
        {
            //Check if you see the player
            var CenterPos = transform.position;
            Vector2 direction = (target.CenterPosition - CenterPos).normalized;

            // #region  
            // RaycastHit2D hit = Physics2D.Raycast(CenterPos, direction, _targetDetectionRange, _obstaclesLayerMask);
            // if (hit.collider != null && (_obstaclesLayerMask & (1 << hit.collider.gameObject.layer)) != 0)
            // {
            //     Debug.DrawRay(CenterPos, direction * _targetDetectionRange, Color.magenta);
            //     InteractionObject io = hit.collider.GetComponent<InteractionObject>();
            //     _targets = new List<InteractionObject>() { io };
            // }
            // else
            // {
            //     _targets = null;
            // }
            // #endregion

            //장애물이 있을때 돌아서 가게끔 하는 로직
             Debug.DrawRay(CenterPos, direction * _targetDetectionRange, Color.magenta);
             _targets = new List<InteractionObject>() { target };
        }
        else
        {
            //타겟을 못찾았을 때
            if(Owner.ObjectType == Define.EObjectType.Hero)
                _targets = new List<InteractionObject>() { Managers.Object.GatherPoint};
            else
                _targets = null;
        }
        aiData.targets = _targets;
    }

    public void SetInfo(InteractionObject owner)
    {
        
    }

    private void OnDrawGizmos()
    {
        if (_showGizmos == false)
            return;

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, _targetDetectionRange);

        if (_targets == null)
            return;
        Gizmos.color = Color.magenta;
        foreach (var creature in _targets)
        {
            if(creature.IsValid())
                Gizmos.DrawSphere(creature.CenterPosition, 0.3f);
        }
    }
}