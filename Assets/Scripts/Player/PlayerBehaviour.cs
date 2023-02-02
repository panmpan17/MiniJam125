using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    private int maxHealthPoint;
    private int _healthPoint;

    [Header("Attack")]
    [SerializeField]
    private AbstractWeapon weapon;

    private PlayerInput _input;


    void Awake()
    {
        _input = GetComponent<PlayerInput>();
    }

    void OnEnable()
    {
        _input.Scheme.Player.WeaponAttack1.started += weapon.OnAttackStarted;
        _input.Scheme.Player.WeaponAttack1.performed += weapon.OnAttackPerformed;
        _input.Scheme.Player.WeaponAttack1.canceled += weapon.OnAttackCanceled;
    }

    void OnDisable()
    {
        _input.Scheme.Player.WeaponAttack1.started -= weapon.OnAttackStarted;
        _input.Scheme.Player.WeaponAttack1.performed -= weapon.OnAttackPerformed;
        _input.Scheme.Player.WeaponAttack1.canceled -= weapon.OnAttackCanceled;
    }

    public void OnTakeDamage()
    {
        Debug.Log("Take damage");
    }
}
