using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : CreatureController
{
    [SerializeField] 
    public GameObject Indicator;
    private Vector2 _moveDir = Vector2.zero;

    public bool IsLeader = false;
    public Vector2 MoveDir
    {
        get => _moveDir;
        set => _moveDir = value.normalized;
    }

    protected override bool Init()
    {
        base.Init();

        EObjectType = Define.EObjectType.Player;

        IsLeader = true;
        
        //event
        Managers.Game.OnMoveDirChanged += HandleOnMoveDirChanged;
        
        //camera
        FindObjectOfType<CameraController>().PlayerTransform = gameObject.transform;
        transform.localScale = Vector3.one;
        return true;
    }
    
    private void Update()
    {
        UpdatePlayerDirection();
        MovePlayer();
    }

    private void UpdatePlayerDirection()
    {
        if (_moveDir.x < 0)
            CreatureSprite.flipX = false;
        else
            CreatureSprite.flipX = true;
    }

    private void MovePlayer()
    {
        _rigidBody.velocity = Vector2.zero;

        Vector3 dir = _moveDir * (MoveSpeed * Time.deltaTime);
        transform.position += dir;

        if (dir != Vector3.zero)
        {
            Indicator.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-dir.x, dir.y) * 180 / Mathf.PI);
        }
        else
            _rigidBody.velocity = Vector2.zero;
    }

    private void HandleOnMoveDirChanged(Vector2 dir)
    {
        _moveDir = dir;
    }
}
