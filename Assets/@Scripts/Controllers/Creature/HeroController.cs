using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class HeroController : CreatureController
{
    [SerializeField] public GameObject Indicator;
    private Vector2 _moveDir = Vector2.zero;
    public Vector3Int CellPos { get; set; } = Vector3Int.zero;
    private HeroController _myHero;

    public HeroController MyLeader
    {
        get => _myHero;
        set
        {
            _myHero = value;
            if (_myHero)
            {
                Indicator.gameObject.SetActive(false);
                IsLeader = false;
            }
            else
            {
                Indicator.gameObject.SetActive(true);
                FindObjectOfType<CameraController>().Target = this;
                GatheringPoint = CenterPosition;
                IsLeader = true;
                Managers.Game.Leader = this;
            }
            //1. 화살표 제거 
        }
    } 
    public bool IsLeader = false;

    protected override bool Init()
    {
        base.Init();

        ObjectType = Define.EObjectType.Hero;
        CreatureState = Define.ECreatureState.Idle;
        
        //event
        Managers.Game.OnMoveDirChanged -= HandleOnMoveDirChanged;
        Managers.Game.OnJoystickTypeChanged -= HandleOnJoystickStateChanged;
        Managers.Game.OnMoveDirChanged += HandleOnMoveDirChanged;
        Managers.Game.OnJoystickTypeChanged += HandleOnJoystickStateChanged;

        //camera
        // FindObjectOfType<CameraController>().PlayerTransform = gameObject.transform;
        // transform.localScale = Vector3.one;

        Vector3 pos = Managers.Map.CurrentGrid.GetCellCenterWorld(CellPos);
        // Vector3 pos2 = Managers.Map.CurrentGrid.CellToWorld(CellPos);

        return true;
    }

    private void FixedUpdate()
    {
        // if(IsLeader == false) return;
        UpdatePlayerDirection();
        MovePlayer();
    }

    protected override void UpdateAnimation()
    {
        base.UpdateAnimation();
        switch (CreatureState)
        {
            case Define.ECreatureState.Idle:
                break;
            case Define.ECreatureState.Attack:
                break;
            case Define.ECreatureState.Moving:
                break;
            case Define.ECreatureState.Gathering:
                // MoveCrew();
                break;
            case Define.ECreatureState.OnDamaged:
                break;
            case Define.ECreatureState.Dead:
                break;
            default:
                break;
        }
    }

    protected override void Scanning()
    {
        base.Scanning();
        if (ScanningCoroutine == null)
        {
            ScanningCoroutine = StartCoroutine(CoScanning());
        }
    }


    private void UpdatePlayerDirection()
    {
        if (_moveDir.x < 0)
            CurrentSprite.flipX = false;
        else
            CurrentSprite.flipX = true;

        if (InteractingTarget)
        {
            Vector3 dirVec = InteractingTarget.CenterPosition - CenterPosition;
            CurrentSprite.flipX = !(dirVec.x < 0);
        }
      
    }

    private void MovePlayer()
    {
        // _rigidBody.velocity = Vector2.zero;
        Vector3 dir = _moveDir * (Attribute.MoveSpeed.CurrentValue * Time.deltaTime);
        // transform.position += dir;
        if (dir != Vector3.zero)
        {
            _rigidBody.MovePosition( transform.position + dir);
            //MOVE
            Indicator.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-dir.x, dir.y) * 180 / Mathf.PI);
            CellPos = Managers.Map.CurrentGrid.WorldToCell(transform.position);
            CurrentSprite.flipX = !(dir.x < 0);
             // Debug.Log($"@>>{gameObject.name} Move to ... Speed : {MoveSpeed}  , dir : {transform.position + dir}");

            // Debug.Log($"CellPos : {CellPos}, WorldPos : {transform.position}");
        }
        else
        {
            //IDLE
            _rigidBody.velocity = Vector2.zero;
        }
    }

    private void MoveCrew()
    {
        // if (MyLeader)
        // {
        //     if(MoveCoroutine == null)
        //         MoveCoroutine = StartCoroutine(CoMove(MyLeader,true));
        // }
    }

    private bool isDraw = false;

    private void HandleOnMoveDirChanged(Vector2 dir)
    {
        _moveDir = dir;
    }

    private void HandleOnJoystickStateChanged(Define.EJoystickState joystickState)
    {
        switch (joystickState)
        {
            case Define.EJoystickState.PointDown:
                InteractingTarget = null;
                StopMoveCoroutine();
                StopScanningCoroutine();
                CreatureState = Define.ECreatureState.Moving;
                // if (IsLeader)
                // {
                //     CreatureState = Define.ECreatureState.Moving;
                // }
                // else
                // {
                // }

                break;
            case Define.EJoystickState.Dragging:
             break;
            case Define.EJoystickState.PointUp:
                StopMoveCoroutine();
                StopScanningCoroutine();
                CreatureState = Define.ECreatureState.Idle;
                if (IsLeader)
                {
                    GatheringPoint = CenterPosition;
                }

                break;
            default:
                break;
        }
    }
}