using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : Detector
{
    private float _targetDetectionRange = 10;
    private LayerMask _obstaclesLayerMask, _playerLayerMask;
    private bool _showGizmos = false;
    private List<CreatureController> _targets;

    public override void Detect(AIData aiData)
    {
        //Find out if player is near
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, _targetDetectionRange, _playerLayerMask);//target

        CreatureController player = null;
        if(playerCollider != null)
            player = playerCollider.gameObject.GetComponent<CreatureController>();
        
        if (player.IsValid())
        {
            //Check if you see the player
            var CenterPos = transform.position;
            Vector2 direction = (player.CenterPosition - CenterPos).normalized;
            RaycastHit2D hit = 
                Physics2D.Raycast(CenterPos, direction, _targetDetectionRange, _obstaclesLayerMask);

            #region  temp 장애물 때문에 시야가 안보여서 그대로 멈추는 로직이 필요한 경우
            // if (hit.collider != null && (playerLayerMask & (1 << hit.collider.gameObject.layer)) != 0)
            // {
            //     Debug.DrawRay(CenterPos, direction * targetDetectionRange, Color.magenta);
            //     targets = new List<CreatureController>() { player };
            // }
            // else
            // {
            //     targets = null;
            // }
            #endregion

            //장애물이 있을때 돌아서 가게끔 하는 로직
            Debug.DrawRay(CenterPos, direction * _targetDetectionRange, Color.magenta);
            _targets = new List<CreatureController>() { player };
        }
        else
        {
            //Enemy doesn't see the player
            _targets = null;
        }
        aiData.targets = _targets;
    }

    private void OnDrawGizmos()
    {
        if (_showGizmos == false)
            return;

        Gizmos.DrawWireSphere(transform.position, _targetDetectionRange);

        if (_targets == null)
            return;
        Gizmos.color = Color.magenta;
        foreach (var creature in _targets)
        {
            Gizmos.DrawSphere(creature.CenterPosition, 0.3f);
        }
    }
}