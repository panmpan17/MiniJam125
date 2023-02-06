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
    private IntEventReference enegyEvent;
    [SerializeField]
    private EventReference deadEvent;
    [SerializeField]
    private EventReference winEvent;
    [SerializeField]
    private ImpluseData onDeadImpluse, onDamageImpluse;

    [SerializeField]
    private int maxHealthPoint;
    private int _healthPoint;

    [Header("Energy")]
    [SerializeField]
    private int maxEnegryPoint;
    private int _enegryPoint;
    public bool HasEnergy => _enegryPoint > 0;
    [SerializeField]
    private Timer enegryRecoverTimer;
    [SerializeField]
    private Timer enegryRecoverPauseTimer;

    [Header("Health Recover")]
    [SerializeField]
    private EffectReference healEffect;
    [SerializeField]
    private Timer perfectDashTimer;

    [Header("Attack")]
    [SerializeField]
    private AbstractWeapon weapon;

    [Header("Invincible")]
    [SerializeField]
    [Layer]
    private int playerLayer;
    [SerializeField]
    [Layer]
    private int enemyWeaponLayer;
    [SerializeField]
    private Timer invincibleWhenDashing;
    [SerializeField]
    private Timer invincibleAfterDamageTimer;
    private bool _invincible;

    [Header("Audio")]
    [SerializeField]
    private SFX dash;
    [SerializeField]
    private SFX energyEmpty, energyRefill, getHit, perfectDodge;

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

        _enegryPoint = maxEnegryPoint;
        enegyEvent.Invoke(_enegryPoint);
    }

    void OnEnable()
    {
        _input.WeaponAttack1.started += weapon.OnAttackStarted;
        _input.WeaponAttack1.performed += weapon.OnAttackPerformed;
        _input.WeaponAttack1.canceled += weapon.OnAttackCanceled;

        _movement.OnDashStartedEvent += OnDashStared;
        _movement.OnFacingChanged += weapon.OnPlayerFacingChanged;

        winEvent.RegisterEvent(OnWin);
    }

    void OnDisable()
    {
        _input.WeaponAttack1.started -= weapon.OnAttackStarted;
        _input.WeaponAttack1.performed -= weapon.OnAttackPerformed;
        _input.WeaponAttack1.canceled -= weapon.OnAttackCanceled;

        _movement.OnDashStartedEvent -= OnDashStared;
        _movement.OnFacingChanged -= weapon.OnPlayerFacingChanged;

        winEvent.UnregisterEvent(OnWin);
    }

    void Update()
    {
        if (perfectDashTimer.Running && perfectDashTimer.UpdateEnd)
            perfectDashTimer.Running = false;

        UpdateInvincibleTimer();
        UpdateEnergyRecover();
    }

    void UpdateInvincibleTimer()
    {
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

    void UpdateEnergyRecover()
    {
        if (enegryRecoverPauseTimer.Running)
        {
            if (enegryRecoverPauseTimer.UpdateEnd)
                enegryRecoverPauseTimer.Running = false;
            return;
        }

        if (!enegryRecoverTimer.Running)
            return;

        if (enegryRecoverTimer.UpdateEnd)
        {
            _enegryPoint++;
            enegyEvent.Invoke(_enegryPoint);
            energyRefill?.Play();

            if (_enegryPoint < maxEnegryPoint)
                enegryRecoverTimer.Reset();
            else
                enegryRecoverTimer.Running = false;
        }
    }


    public bool OnTakeDamage()
    {
        if (_invincible)
            return true;
        if (invincibleWhenDashing.Running)
            return false;
        if (invincibleAfterDamageTimer.Running)
            return false;

        _healthPoint -= 1;
        healthEvent.Invoke(_healthPoint);
        getHit?.Play();

        if (_healthPoint <= 0)
        {
            if (onDeadImpluse) ImpluseCamera.ins.GenerateImpluse(onDeadImpluse);
            gameObject.SetActive(false);
            deadEvent.Invoke();
            return true;
        }

        if (onDamageImpluse) ImpluseCamera.ins.GenerateImpluse(onDamageImpluse);
        OnHurt.Invoke();
        invincibleAfterDamageTimer.Reset();
        Physics2D.IgnoreLayerCollision(playerLayer, enemyWeaponLayer, true);
        perfectDashTimer.Reset();
        return true;
    }

    void HealthRecover()
    {
        if (_healthPoint >= maxHealthPoint)
            return;

        healthEvent.Invoke(++_healthPoint);
        healEffect.AddWaitingList(new EffectReference.EffectQueue
        {
            Parent = transform,
            Position = transform.position,
            Rotation = Quaternion.identity
        });

        perfectDodge?.Play();
    }

    void OnDashStared()
    {
        if (perfectDashTimer.Running)
        {
            perfectDashTimer.Running = false;
            HealthRecover();
        }

        invincibleWhenDashing.Reset();
        Physics2D.IgnoreLayerCollision(playerLayer, enemyWeaponLayer, true);

        dash?.Play();
    }

    public void UseEnergy()
    {
        _enegryPoint -= 1;
        enegyEvent.Invoke(_enegryPoint);

        enegryRecoverTimer.Reset();

        if (_enegryPoint <= 0)
        {
            enegryRecoverPauseTimer.Reset();
            energyEmpty?.Play();
        }
    }

    void OnWin()
    {
        _invincible = true;
    }
}
