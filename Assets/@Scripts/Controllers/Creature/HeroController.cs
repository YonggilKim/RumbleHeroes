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
                // Managers.Map.GatheringPoint = CenterPosition;
                Managers.Object.GatherPoint.transform.position = CenterPosition;
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

    private void HandleOnMoveDirChanged(Vector2 dir)
    {
        // _moveDir = dir;
        CreatureMovementMovement.MovementInput = dir;
    }

    private void HandleOnJoystickStateChanged(Define.EJoystickState joystickState)
    {
        switch (joystickState)
        {
            case Define.EJoystickState.PointDown:
                CreatureState = Define.ECreatureState.Moving;
                _aiController.IsAutoMode = false;
                break;
            case Define.EJoystickState.Dragging:
             break;
            case Define.EJoystickState.PointUp:
                CreatureState = Define.ECreatureState.Idle;
                _aiController.IsAutoMode = true;
                if (IsLeader)
                {
                    Managers.Object.GatherPoint.transform.position = CenterPosition;
                }
                break;
            default:
                break;
        }
    }
}