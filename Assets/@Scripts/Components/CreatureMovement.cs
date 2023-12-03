using UnityEngine;

public class CreatureMovement : MonoBehaviour
{
    private CreatureController _owner { get; set; }
    private Rigidbody2D _rigid;

    #region  Movement
    public Vector2 MovementInput { get; set; }
    private Vector2 oldMovementInput;

    private float _maxSpeed = 4, _acceleration = 50, _deacceleration = 100;
    [SerializeField]
    private float _currentSpeed = 0;
    #endregion

    private void Awake()
    {
        gameObject.GetComponent<AIController>().OnChangedMovementInput = (input) =>
        {
            MovementInput = input;
            if (Managers.Game.JoystickState == Define.EJoystickState.Dragging &&
                _owner.ObjectType == Define.EObjectType.Hero && MovementInput == Vector2.zero)
            {
                Debug.LogError("게더링추격");
            }
            
        };

        _rigid = GetComponent<Rigidbody2D>();
    }
    
    private void FixedUpdate()
    {
        Move();
        AnimateCharacter();
    }

    public void SetInfo(CreatureController creature)
    {
        _owner = creature;
        _maxSpeed = creature.Attribute.MoveSpeed.CurrentValue;
    }

    public void PerformAttack()
    {
    }

    private void Move()
    {
        if (MovementInput.magnitude > 0 && _currentSpeed >= 0)
        {
            oldMovementInput = MovementInput;
            _currentSpeed += _acceleration * _maxSpeed * Time.deltaTime;
        }
        else
        {
            _currentSpeed -= _deacceleration * _maxSpeed * Time.deltaTime;
        }
        _currentSpeed = Mathf.Clamp(_currentSpeed, 0, _maxSpeed);
        _rigid.velocity = oldMovementInput * _currentSpeed;
        

    }

    private void AnimateCharacter()
    {
        if (MovementInput != Vector2.zero)
        {
            _owner.CurrentSprite.flipX = !(MovementInput.x < 0);
            _owner.CreatureState = Define.ECreatureState.Moving;
        }
        else
        {
            if (_owner.InteractingTarget.IsValid())
            {
                Vector2 dirVec = _owner.InteractingTarget.CenterPosition - _owner.CenterPosition;
                _owner.CurrentSprite.flipX = !(dirVec.x < 0);
            }
        }

    }

}
