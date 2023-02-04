using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private new Rigidbody2D rigidbody2D;

    [Header("Moving")]
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

    [Header("Dash")]
    [SerializeField]
    private Timer dashTimer;
    [SerializeField]
    private float dashForce;
    [SerializeField]
    private AnimationCurveReference dashForceCurve;
    [SerializeField]
    private Timer dashColddownTimer;
    private bool _dashing;
    private Vector2 _dashDirection;

    public event System.Action OnDashStartedEvent;
    public event System.Action OnDashEndedEvent;

    private PlayerInput _input;
    private Facing _facing = Facing.Up;

    private Vector2 _moveDirection;


    void Awake()
    {
        _input = GetComponent<PlayerInput>();
    }

    void OnEnable()
    {
        _input.Move.performed += OnMovePerformed;
        _input.Move.canceled += OnCancelPerformed;
        _input.Dash.started += OnDashStarted;
    }

    void OnDisable()
    {
        _input.Move.performed -= OnMovePerformed;
        _input.Move.canceled -= OnCancelPerformed;
        _input.Dash.started -= OnDashStarted;
    }


    void FixedUpdate()
    {
        if (_dashing)
        {
            HandleDashing();
            return;
        }

        if (dashColddownTimer.Running)
            HandleDashColddown();
        HandleMoving();
    }

    void HandleMoving()
    {
        Vector2 position = transform.position;
        position += moveSpeed * Time.fixedDeltaTime * _moveDirection;
        rigidbody2D.position = position;

        Facing newFacing = CalculateFacing(_moveDirection);

        if (_facing != newFacing)
        {
            SwitchToNewFacing(newFacing);

            _facing = newFacing;
        }
    }

    void HandleDashColddown()
    {
        if (dashColddownTimer.UpdateEnd)
            dashColddownTimer.Running = false;
    }

    void HandleDashing()
    {
        if (dashTimer.UpdateEnd)
        {
            _dashing = false;
            dashColddownTimer.Reset();
            OnDashEndedEvent?.Invoke();
            return;
        }

        Vector2 position = transform.position;
        float dashSpeed = dashForceCurve.Evaluate(dashTimer.Progress) * dashForce;
        position += dashSpeed * Time.fixedDeltaTime * _dashDirection;
        rigidbody2D.position = position;
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

    void OnDashStarted(CallbackContext callbackContext)
    {
        if (_dashing)
            return;
        if (dashColddownTimer.Running)
            return;

        if (_moveDirection.x == 0 && _moveDirection.y == 0)
        {
            switch (_facing)
            {
                case Facing.Left:
                    _dashDirection = Vector2.left;
                    break;
                case Facing.Right:
                    _dashDirection = Vector2.right;
                    break;
                case Facing.Up:
                    _dashDirection = Vector2.up;
                    break;
                case Facing.Down:
                    _dashDirection = Vector2.down;
                    break;
            }
        }
        else
            _dashDirection = _moveDirection.normalized;

        _dashing = true;
        dashTimer.Reset();
        OnDashStartedEvent?.Invoke();
    }
#endregion

    public enum Facing { Up, Down, Left, Right }
}
