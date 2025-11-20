using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;

    private Vector2 _movement;

    private Rigidbody2D _rb;
    private Vector2 input;
    private Vector2 lastMoveDirection;
    public Transform Aim;
    bool isWalking = false;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _movement.Set(InputManager.Movement.x, InputManager.Movement.y);

        _rb.linearVelocity = _movement * _moveSpeed;

        if(_rb.linearVelocity == Vector2.zero)
        {
            isWalking = false;
            lastMoveDirection = InputManager.Movement;
            Vector3 vector3 = Vector3.left * lastMoveDirection.x + Vector3.down * lastMoveDirection.y;
            
        }
        else
        {
            isWalking = true;
            Vector3 vector3 = Vector3.left * InputManager.Movement.x + Vector3.down * InputManager.Movement.y;
            Aim.rotation = Quaternion.LookRotation(Vector3.forward, vector3);
        }
    }
}
