using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;

    private PlayerInput _input;

    private Vector2 _moveDirection;


    void Awake()
    {
        _input = GetComponent<PlayerInput>();

        _input.Scheme.Player.Move.performed += OnMovePerformed;
        _input.Scheme.Player.Move.canceled += OnCancelPerformed;
    }

    void FixedUpdate()
    {
        Vector2 position = transform.position;
        position += moveSpeed * Time.fixedDeltaTime * _moveDirection;
        transform.position = position;
    }


#region Input
    void OnMovePerformed(CallbackContext callbackContext) => _moveDirection = callbackContext.ReadValue<Vector2>();
    void OnCancelPerformed(CallbackContext callbackContext) => _moveDirection = callbackContext.ReadValue<Vector2>();
#endregion
}
