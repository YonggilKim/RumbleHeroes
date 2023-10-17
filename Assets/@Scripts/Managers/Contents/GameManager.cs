using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [FormerlySerializedAs("JoystickType")] public Define.EJoystickType eJoystickType = Define.EJoystickType.Flexible;
    
    #region Player
    public HeroController Hero { get; set; }
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
    #endregion
 
    #region Action
    
    public event Action<Vector2> OnMoveDirChanged;
    
    #endregion
    
    public void Init()
    {

    }

}
