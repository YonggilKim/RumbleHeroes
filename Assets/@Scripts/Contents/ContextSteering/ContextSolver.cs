using System.Collections.Generic;
using UnityEngine;

public class ContextSolver : MonoBehaviour
{
    [SerializeField]
    private bool showGizmos = true;

    //gizmo parameters
    private Vector2 _resultDirection = Vector2.zero;
    private const float _rayLength = 2;

    public Vector2 GetDirectionToMove(List<SteeringBehaviour> behaviours, AIData aiData)
    {
        float[] danger = new float[8];
        float[] interest = new float[8];

        foreach (SteeringBehaviour behaviour in behaviours)
        {
            (danger, interest) = behaviour.GetSteering(danger, interest, aiData);
        }
        
        for (int i = 0; i < 8; i++)
        {
            interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
        }

        //각 방향 벡터에 대응하는 interest 값이 1인 경우 해당 방향 벡터가 최종 방향에 기여
        Vector2 outputDirection = Vector2.zero;
        for (int i = 0; i < 8; i++)
        {
            outputDirection += Directions.eightDirections[i] * interest[i];
        }

        outputDirection.Normalize();

        _resultDirection = outputDirection;

        return _resultDirection;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying && showGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, _resultDirection * _rayLength);
        }
    }
}