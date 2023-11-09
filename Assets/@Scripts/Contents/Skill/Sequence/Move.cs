using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : SequenceSkill
{
    Rigidbody2D _rb;
    Coroutine _coroutine;
    string AnimationName { get; } = "Moving";
    private float Speed { get; set; } = 2.0f;
    private CreatureController _owner;
    private InteractionObject _target;
    private Transform targetPos;
    
    private void Awake()
    {
        _owner = GetComponent<CreatureController>();
    }
    public override void DoSkill(Action callback = null)
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        Speed = _owner.MoveSpeed;
        _coroutine = StartCoroutine(CoMove(callback));
    }

    IEnumerator CoMove(Action callback = null)
    {
        _rb = GetComponent<Rigidbody2D>();
        // transform.GetChild(0).GetComponent<Animator>().Play(AnimagtionName);
        float elapsed = 0;

        while (true)
        {
            elapsed += Time.deltaTime;
            if (elapsed > 3.0f)
                break;

            Vector3 dir = (_target.CenterPosition - _owner.CenterPosition).normalized;
            Vector2 targetPosition = _target.CenterPosition + dir * UnityEngine.Random.Range(SkillData.MinCoverage, SkillData.MaxCoverage);

            if (Vector3.Distance(_rb.position, targetPosition) <= 0.1f)
                continue;

            Vector2 dirVec = targetPosition - _rb.position;
            Vector2 nextVec = dirVec.normalized * SkillData.ProjSpeed * Time.fixedDeltaTime;
            _rb.MovePosition(_rb.position + nextVec);

            yield return null;
        }
        callback?.Invoke();
    }

    public override void OnChangedSkillData()
    {
    }
}
