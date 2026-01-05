using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public float _moveSpeed = 5f;

    private Vector2 _movement;
    public Animator _animator;
    private Rigidbody2D _rb;
    private Vector2 input;
    public Vector2 lastMoveDirection;
    public Transform Aim;
    bool isWalking = false;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _rb.linearVelocity = _movement * _moveSpeed;

        if(_rb.linearVelocity == Vector2.zero)
        {
            isWalking = false;
            //lastMoveDirection = InputManager.Movement;
            //Vector3 vector3 = Vector3.left * lastMoveDirection.x + Vector3.down * lastMoveDirection.y;
            
        }
        else
        {
            isWalking = true;
            Vector3 vector3 = Vector3.left * _movement.x + Vector3.down * _movement.y;
            Aim.rotation = Quaternion.LookRotation(Vector3.forward, vector3);
        }

        _animator.SetFloat("X", _rb.linearVelocityX);
        _animator.SetFloat("Y", _rb.linearVelocityY);


    }

    public void Move(InputAction.CallbackContext ctx)
    {
        _movement= ctx.ReadValue<Vector2>();

        if (ctx.ReadValue<Vector2>() != Vector2.zero)
        {
            lastMoveDirection = ctx.ReadValue<Vector2>();
        }
    }
}
