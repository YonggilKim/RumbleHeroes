using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CurveShotComponent : MonoBehaviour
{
    private Vector2 _targetPosition;
    private float _speed = 5;
    private float _heightArc = 3;
    private Vector3 _startPosition;
    public bool IsRotation = false;
    
    public void Shot(Vector2 startPosition, Vector3 targetPosition, float speed = 5, float heightArc = 3, Action EndCallback = null)
    {
        _startPosition = startPosition;
        _targetPosition = targetPosition;
        _speed = speed;
        _heightArc = heightArc;
        
        StartCoroutine(CoShoot(EndCallback));
        
    }
    IEnumerator CoShoot(Action EndCallback)
    {
        while (true)
        {
            float x0 = _startPosition.x;
            float x1 = _targetPosition.x;
            float distance = x1 - x0;

            float nextX = Mathf.MoveTowards(transform.position.x, x1, _speed * Time.deltaTime);
            float baseY = Mathf.Lerp(_startPosition.y, _targetPosition.y, (nextX - x0) / distance);
            float arc = _heightArc * (nextX - x0) * (nextX - x1) / (-0.25f * distance * distance);
            Vector3 nextPosition = new Vector3(nextX, baseY + arc, transform.position.z);

            if(IsRotation)
                transform.rotation = LookAt2D(nextPosition - transform.position);
            transform.position = nextPosition;

            if ((Vector2)nextPosition == _targetPosition)
            {
                EndCallback?.Invoke();
                break;
            }

            yield return new WaitForFixedUpdate();
        }
    }
    
    Quaternion LookAt2D(Vector2 forward)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
    }

}
