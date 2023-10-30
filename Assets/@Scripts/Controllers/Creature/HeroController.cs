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

    private Coroutine ScanningCoroutine;
    public bool IsLeader = false;

    public Vector2 MoveDir
    {
        get => _moveDir;
        set => _moveDir = value.normalized;
    }

    protected override bool Init()
    {
        base.Init();

        ObjectType = Define.EObjectType.Hero;

        IsLeader = true;

        //event
        Managers.Game.OnMoveDirChanged += HandleOnMoveDirChanged;
        Managers.Game.OnJoystickTypeChanged += HandleOnJoystickStateChanged;

        //camera
        FindObjectOfType<CameraController>().PlayerTransform = gameObject.transform;
        transform.localScale = Vector3.one;

        Vector3 pos = Managers.Map.CurrentGrid.GetCellCenterWorld(CellPos);
        // Vector3 pos2 = Managers.Map.CurrentGrid.CellToWorld(CellPos);
        transform.position = pos;

        return true;
    }

    private void FixedUpdate()
    {
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
        if(ScanningCoroutine == null)
            ScanningCoroutine = StartCoroutine(CoScanning());
    }

    IEnumerator CoScanning()
    {
        while (true)
        {
            //1. 주변을 검색한다.
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll((Vector2)CenterPosition, 3);

            List<MonsterController> monsters = new List<MonsterController>();
            List<InteractionObject> objects = new List<InteractionObject>();
        
            foreach (var collider in hitColliders)
            {
                MonsterController monster = collider.GetComponent<MonsterController>();
                if(monster && monster.Hp > 0)
                    monsters.Add(monster);
            
                InteractionObject interactionObject = collider.GetComponent<InteractionObject>();
                if(interactionObject &&interactionObject.Hp > 0)
                    objects.Add(interactionObject);
            }

            if (monsters.Count > 0)
            {
                monsters = monsters.OrderBy(target => (CenterPosition - target.CenterPosition).sqrMagnitude).ToList();
                MonsterController target = monsters[0];
                //Attack
                Attack(target);
                break;
            }
            else if(objects.Count > 0)
            {
                objects = objects.OrderBy(target => (CenterPosition - target.CenterPosition).sqrMagnitude).ToList();
                InteractionObject target = objects[0];
                Attack(target);
                break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    protected void StopScanningCoroutine()
    {
        if (ScanningCoroutine != null)
        {
            StopCoroutine(ScanningCoroutine);
            ScanningCoroutine = null; 
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
        _rigidBody.velocity = Vector2.zero;

        Vector3 dir = _moveDir * (MoveSpeed * Time.deltaTime);
        transform.position += dir;

        if (dir != Vector3.zero)
        {
            //MOVE
            Indicator.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-dir.x, dir.y) * 180 / Mathf.PI);
            CellPos = Managers.Map.CurrentGrid.WorldToCell(transform.position);
            CurrentSprite.flipX = !(dir.x < 0);

            // Debug.Log($"CellPos : {CellPos}, WorldPos : {transform.position}");
        }
        else
        {
            //IDLE
            _rigidBody.velocity = Vector2.zero;
        }
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
                break;
            case Define.EJoystickState.Dragging:
                CreatureState = Define.ECreatureState.Moving;
                break;
            case Define.EJoystickState.PointUp:
                CreatureState = Define.ECreatureState.Idle;
                break;
            default:
                break;
        }
    }
}