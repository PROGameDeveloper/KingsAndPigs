using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GatherInput : MonoBehaviour
{
    private Controls _controls;
    
    [SerializeField] private Vector2 value;
    public Vector2 Value => value;

    [SerializeField] private bool isJumping;
    public bool IsJumping { get => isJumping; set => isJumping = value; }

    private void Awake()
    {
        _controls = new Controls();
    }

    private void OnEnable()
    {
        _controls.Player.Move.performed += StartMove;
        _controls.Player.Move.canceled += StopMove;
        _controls.Player.Jump.performed += StartJump;
        _controls.Player.Jump.canceled += StopJump;
        _controls.Player.Enable();
    }

    private void StartMove(InputAction.CallbackContext context)
    {
        value = context.ReadValue<Vector2>().normalized;
    }

    private void StopMove(InputAction.CallbackContext context)
    {
        value = Vector2.zero;

    }

    private void StartJump (InputAction.CallbackContext context)
    {
        isJumping = true;
    }

    private void StopJump(InputAction.CallbackContext context)
    {
        isJumping = false;
    }

    private void OnDisable()
    {
        _controls.Player.Move.performed -= StartMove;
        _controls.Player.Move.canceled -= StopMove;
        _controls.Player.Jump.performed -= StartJump;
        _controls.Player.Jump.canceled -= StopJump;
        _controls.Player.Disable();
    }
}
