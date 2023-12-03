using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager
{
    public Define.EJoystickType JoystickType = Define.EJoystickType.Flexible;
    
    #region Player
    public HeroController Leader { get; set; }
    private Vector2 _moveDir;
    public Vector2 MoveDir
    {
        get => _moveDir;
        set
        {
            _moveDir = value;
            OnMoveDirChanged?.Invoke(_moveDir);
        }
    }
    
    private Define.EJoystickState _joystickState;
    public Define.EJoystickState JoystickState
    {
        get => _joystickState;
        set
        {
            _joystickState = value;
            OnJoystickTypeChanged?.Invoke(_joystickState);
        }
    }

    #endregion



    #region Action
    
    public event Action<Vector2> OnMoveDirChanged;
    public event Action<Define.EJoystickState> OnJoystickTypeChanged;
    
    #endregion
    
    public void Init()
    {

    }


}
