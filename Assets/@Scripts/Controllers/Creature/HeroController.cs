using System;
using System.Collections;
using System.Collections.Generic;
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
        switch (CreatureState)
        {
            case Define.ECreatureState.Idle:
                Anim.Play("Idle");
                break;
            case Define.ECreatureState.Skill:
                Anim.Play("Skill");
                break;
            case Define.ECreatureState.Moving:
                Anim.Play("Move");
                break;
            case Define.ECreatureState.Gathering:
                Anim.Play("Gathering");
                break;
            case Define.ECreatureState.OnDamaged:
                Anim.Play("OnDamaged");
                break;
            case Define.ECreatureState.Dead:
                Anim.Play("Die");
                break;
            default:
                break;
        }
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
            //MOVE
            Indicator.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-dir.x, dir.y) * 180 / Mathf.PI);
            CreatureState = Define.ECreatureState.Moving;
            CellPos = Managers.Map.CurrentGrid.WorldToCell(transform.position);
            Debug.Log($"CellPos : {CellPos}, WorldPos : {transform.position}");
        }
        else
        {
            //IDLE
            _rigidBody.velocity = Vector2.zero;
            CreatureState = Define.ECreatureState.Idle;
        }
    }

    private bool isDraw = false;

    private void OnDrawGizmos()
    {
        if (Managers.Map.CurrentGrid == null)
            return;

        Gizmos.color = Color.green;

        Vector3 gridSize = Managers.Map.CurrentGrid.cellSize;

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