using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeekBehaviour : SteeringBehaviour
{
    [SerializeField]
    private float _targetRechedThreshold = 0.5f;

    [SerializeField]
    private bool _showGizmo = true;

    bool _reachedLastTarget = true;

    //gizmo parameters
    private Vector2 _targetPositionCached;
    private float[] _interestsTemp;

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData)
    {
        if (_reachedLastTarget)
        {
            if (aiData.targets == null || aiData.targets.Count <= 0)
            {
                //Stop Seeking
                aiData.currentTarget = null;
                return (danger, interest);
            }
            else
            {
                _reachedLastTarget = false;

                // aiData.currentTarget = aiData.targets
                //     .OrderBy(target => (target.CenterPosition - transform.position).sqrMagnitude)
                //     .FirstOrDefault();
                
                //가장 가까운 원소 를 현재타겟으로 설정
                float minDistance = float.MaxValue;
                foreach (var target in aiData.targets)
                {
                    float distance = (target.CenterPosition - transform.position).sqrMagnitude;
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        aiData.currentTarget = target;
                    }
                }
            }
        }

        if (aiData.currentTarget != null && aiData.targets != null && aiData.targets.Contains(aiData.currentTarget))
            _targetPositionCached = aiData.currentTarget.CenterPosition;

        // 목표 위치에 도착
        float distanceSquared = (_targetPositionCached - (Vector2)transform.position).sqrMagnitude;

        if (Vector2.Distance(_targetPositionCached, transform.position) < _targetRechedThreshold )
        {
            _reachedLastTarget = true;
            aiData.currentTarget = null;
            return (danger, interest);
        }

        //도착할때까지 Direction 계산
        Vector2 directionToTarget = (_targetPositionCached - (Vector2)transform.position);
        for (int i = 0; i < interest.Length; i++)
        {
            float result = Vector2.Dot(directionToTarget.normalized, Directions.eightDirections[i]);

            // directionToTarget 방향과 eightDirection의 각도가 90도 미만인 경우에만 대입
            // if (result > Mathf.Cos(Mathf.Deg2Rad * 120)) // 120도 미만
            if (result > 0)
            {
                float valueToPutIn = result;
                if (valueToPutIn > interest[i])
                {
                    interest[i] = valueToPutIn;
                }
            }
        }
        _interestsTemp = interest;
        return (danger, interest);
    }

    private void OnDrawGizmos()
    {

        if (_showGizmo == false)
            return;
        Gizmos.DrawSphere(_targetPositionCached, 0.2f);

        if (Application.isPlaying && _interestsTemp != null)
        {
            if (_interestsTemp != null)
            {
                Gizmos.color = Color.green;
                for (int i = 0; i < _interestsTemp.Length; i++)
                {
                    Gizmos.DrawRay(transform.position, Directions.eightDirections[i] * _interestsTemp[i]*2);
                }
                if (_reachedLastTarget == false)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(_targetPositionCached, 0.1f);
                }
            }
        }
    }
}