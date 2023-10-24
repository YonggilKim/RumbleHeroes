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

    public bool IsLeader = false;

    public Vector2 MoveDir
    {
        get => _moveDir;
        set => _moveDir = value.normalized;
    }

    protected override bool Init()
    {
        base.Init();

        ObjectType = Define.EObjectType.Player;

        IsLeader = true;

        //event
        Managers.Game.OnMoveDirChanged += HandleOnMoveDirChanged;

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

    protected override void AutoWorking()
    {
        base.AutoWorking();

        //1-1 몬스터가 있으면 몬스터로 돌진
        //1-2 가장가까운 자원으로 간다
        //2 아무것도 없으면 가만히 있는다.
    }

    private void UpdatePlayerDirection()
    {
        if (_moveDir.x < 0)
            CurrentSprite.flipX = false;
        else
            CurrentSprite.flipX = true;

        if (InteractTarget)
        {
            Vector3 dirVec = InteractTarget.CenterPosition - CenterPosition;
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
            CreatureState = Define.ECreatureState.Moving;
            CellPos = Managers.Map.CurrentGrid.WorldToCell(transform.position);
            InteractTarget = null;
            // Debug.Log($"CellPos : {CellPos}, WorldPos : {transform.position}");
        }
        else
        {
            //IDLE
            _rigidBody.velocity = Vector2.zero;
        }
    }

    private bool isDraw = false;

    private void OnDrawGizmos()
    {
        if (Managers.Map.CurrentGrid == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(CenterPosition, 7f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(CenterPosition, 0.1f);
        for (int i = Managers.Map.MinX; i < Managers.Map.MaxX; i++)
        {
            for (int j = Managers.Map.MinY; j < Managers.Map.MaxY; j++)
            {
                Vector3Int cellPosition = new Vector3Int(i, j, 0);
                Vector3 worldPosition = Managers.Map.CurrentGrid.CellToWorld(cellPosition) ;
                Gizmos.color = (i == 0 && j == 0) ? Color.blue : Color.green;
                Gizmos.DrawWireSphere(worldPosition + Vector3.up * 1f, 0.1f);

                string label = string.Format("({0}, {1})", i, j);
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.white;
                style.fontSize = 12;
                Handles.Label(worldPosition + Vector3.up * 0.8f, label, style);
                //label = string.Format("World: ({0:F1}, {1:F1}, {2:F1})", worldPosition.x, worldPosition.y, worldPosition.z);
                //Handles.Label(worldPosition + new Vector3(0, -0.2f, 0), label, style);
            }
        }

        isDraw = true;
    }

    private void HandleOnMoveDirChanged(Vector2 dir)
    {
        _moveDir = dir;
    }
    
}