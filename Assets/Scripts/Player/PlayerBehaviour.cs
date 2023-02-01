using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    private int maxHealthPoint;
    private int _healthPoint;

    private PlayerInput _input;


    void Awake()
    {
        _input = GetComponent<PlayerInput>();
    }
}
