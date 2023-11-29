using UnityEngine;

public class Agent : MonoBehaviour
{
    private CreatureController _creature { get; set; }
    private Rigidbody2D _rigid;

    #region  Movement
    public Vector2 MovementInput { get; set; }
    private Vector2 oldMovementInput;

    [SerializeField]
    private float _maxSpeed = 4, _acceleration = 50, _deacceleration = 100;
    [SerializeField]
    private float currentSpeed = 0;
    #endregion

    private void Awake()
    {
        gameObject.GetComponent<AIController>().OnChangedMovementInput = (input) =>
        {
            MovementInput = input;
        };

        _rigid = GetComponent<Rigidbody2D>();
    }
    
    private void FixedUpdate()
    {
        AnimateCharacter();
        Move();
    }

    public void SetInfo(CreatureController creature)
    {
        _creature = creature;
        _maxSpeed = creature.Attribute.MoveSpeed.CurrentValue * 1.5f;
    }

    public void PerformAttack()
    {
    }

    private void Move()
    {
        if (MovementInput.magnitude > 0 && currentSpeed >= 0)
        {
            oldMovementInput = MovementInput;
            currentSpeed += _acceleration * _maxSpeed * Time.deltaTime;
        }
        else
        {
            currentSpeed -= _deacceleration * _maxSpeed * Time.deltaTime;
        }
        currentSpeed = Mathf.Clamp(currentSpeed, 0, _maxSpeed);
        _rigid.velocity = oldMovementInput * currentSpeed;
    }

    private void AnimateCharacter()
    {
        if (MovementInput != Vector2.zero)
        {
            _creature.CurrentSprite.flipX = !(MovementInput.x < 0);
            _creature.CreatureState = Define.ECreatureState.Moving;
        }
        else
        {
            _creature.CreatureState = Define.ECreatureState.Idle;
        }

    }

}
