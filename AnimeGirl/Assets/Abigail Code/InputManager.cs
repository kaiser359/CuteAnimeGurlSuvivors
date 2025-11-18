using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static Vector2 Movement;
    private InputAction _moveAction;
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        _moveAction = _playerInput.actions["Movement"];
    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();
    }
}
