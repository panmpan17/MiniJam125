using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private new Rigidbody2D rigidbody2D;

    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private Quaternion upRotation;
    [SerializeField]
    private Quaternion downRotation;
    [SerializeField]
    private Quaternion leftRotation;
    [SerializeField]
    private Quaternion rightRotation;

    private PlayerInput _input;
    private Facing _facing = Facing.Up;

    private Vector2 _moveDirection;


    void Awake()
    {
        _input = GetComponent<PlayerInput>();
    }

    void OnEnable()
    {
        _input.Scheme.Player.Move.performed += OnMovePerformed;
        _input.Scheme.Player.Move.canceled += OnCancelPerformed;
    }

    void OnDisable()
    {
        _input.Scheme.Player.Move.performed -= OnMovePerformed;
        _input.Scheme.Player.Move.canceled -= OnCancelPerformed;
    }


    void FixedUpdate()
    {
        Vector2 position = transform.position;
        position += moveSpeed * Time.fixedDeltaTime * _moveDirection;
        // transform.position = position;
        rigidbody2D.position = position;

        Facing newFacing = CalculateFacing(_moveDirection);

        if (_facing != newFacing)
        {
            SwitchToNewFacing(newFacing);

            _facing = newFacing;
        }
    }

    Facing CalculateFacing(Vector2 directionVector)
    {
        Facing newFacing = _facing;
        float absX = Mathf.Abs(directionVector.x);
        float absY = Mathf.Abs(directionVector.y);
        if (absX > absY)
        {
            newFacing = directionVector.x > 0 ? Facing.Right : Facing.Left;
        }
        else if (absX < absY)
        {
            newFacing = directionVector.y > 0 ? Facing.Up : Facing.Down;
        }

        return newFacing;
    }

    void SwitchToNewFacing(Facing newFacing)
    {
        switch (newFacing)
        {
            case Facing.Up:
                transform.rotation = upRotation;
                break;
            case Facing.Down:
                transform.rotation = downRotation;
                break;
            case Facing.Left:
                transform.rotation = leftRotation;
                break;
            case Facing.Right:
                transform.rotation = rightRotation;
                break;
        }
    }


#region Input
    void OnMovePerformed(CallbackContext callbackContext) => _moveDirection = callbackContext.ReadValue<Vector2>();
    void OnCancelPerformed(CallbackContext callbackContext) => _moveDirection = callbackContext.ReadValue<Vector2>();
#endregion

    public enum Facing { Up, Down, Left, Right }
}
