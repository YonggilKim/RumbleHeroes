using UnityEngine;

public class PlayerController : CreatureController
{
    [SerializeField] public GameObject Indicator;
    private Vector2 _moveDir = Vector2.zero;

    public Vector2 MoveDir
    {
        get => _moveDir;
        set => _moveDir = value.normalized;
    }

    protected override bool Init()
    {
        base.Init();

        ObjectType = Define.EObjectType.Player;
        //
        // //event
        // Managers.Game.OnMoveDirChanged += HandleOnMoveDirChanged;
        //
        // //camera
        // FindObjectOfType<CameraController>()._playerTransform = gameObject.transform;
        // transform.localScale = Vector3.one;
        return true;
    }
}