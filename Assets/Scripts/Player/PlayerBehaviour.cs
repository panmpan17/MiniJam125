using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;
using MPack;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    private IntEventReference healthEvent;

    [SerializeField]
    [Layer]
    private int playerLayer;
    [SerializeField]
    [Layer]
    private int enemyWeaponLayer;
    [SerializeField]
    private int maxHealthPoint;
    private int _healthPoint;

    [Header("Health Recover")]
    // [SerializeField]
    // private bool recoverWhenInPerfectDash;
    [SerializeField]
    private EffectReference healEffect;
    [SerializeField]
    private Timer perfectDashTimer;

    [Header("Attack")]
    [SerializeField]
    private AbstractWeapon weapon;

    [Header("Invincible After Damage")]
    [SerializeField]
    private Timer invincibleWhenDashing;
    [SerializeField]
    private Timer invincibleAfterDamageTimer;

    public event System.Action OnHurt;


    private PlayerInput _input;
    private PlayerMovement _movement;


    void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _movement = GetComponent<PlayerMovement>();

        invincibleWhenDashing.Running = false;

        _healthPoint = maxHealthPoint;
        healthEvent.Invoke(_healthPoint);
    }

    void OnEnable()
    {
        _input.WeaponAttack1.started += weapon.OnAttackStarted;
        _input.WeaponAttack1.performed += weapon.OnAttackPerformed;
        _input.WeaponAttack1.canceled += weapon.OnAttackCanceled;

        _movement.OnDashStartedEvent += OnDashStared;
    }

    void OnDisable()
    {
        _input.WeaponAttack1.started -= weapon.OnAttackStarted;
        _input.WeaponAttack1.performed -= weapon.OnAttackPerformed;
        _input.WeaponAttack1.canceled -= weapon.OnAttackCanceled;

        _movement.OnDashStartedEvent -= OnDashStared;
    }

    void Update()
    {
        if (perfectDashTimer.Running && perfectDashTimer.UpdateEnd)
        {
            perfectDashTimer.Running = false;
        }

        if (invincibleWhenDashing.Running && invincibleWhenDashing.UpdateEnd)
        {
            invincibleWhenDashing.Running = false;

            if (!invincibleAfterDamageTimer.Running)
                Physics2D.IgnoreLayerCollision(playerLayer, enemyWeaponLayer, false);
        }

        if (invincibleAfterDamageTimer.Running && invincibleAfterDamageTimer.UpdateEnd)
        {
            invincibleAfterDamageTimer.Running = false;
            if (!invincibleWhenDashing.Running)
                Physics2D.IgnoreLayerCollision(playerLayer, enemyWeaponLayer, false);
        }
    }

    public bool OnTakeDamage()
    {
        if (invincibleWhenDashing.Running)
            return false;
        if (invincibleAfterDamageTimer.Running)
            return false;

        _healthPoint -= 1;
        healthEvent.Invoke(_healthPoint);
        OnHurt.Invoke();
        invincibleAfterDamageTimer.Reset();
        Physics2D.IgnoreLayerCollision(playerLayer, enemyWeaponLayer, true);
        return true;
    }

    void HealthRecover()
    {
        if (_healthPoint < maxHealthPoint)
        {
            healthEvent.Invoke(++_healthPoint);
            healEffect.AddWaitingList(new EffectReference.EffectQueue {
                Parent = transform,
                Position = transform.position,
                Rotation = Quaternion.identity
            });
        }
    }

    void OnDashStared()
    {
        perfectDashTimer.Reset();
        invincibleWhenDashing.Reset();
        Physics2D.IgnoreLayerCollision(playerLayer, enemyWeaponLayer, true);
    }
}
