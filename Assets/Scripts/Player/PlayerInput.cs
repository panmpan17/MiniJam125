using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInput : MonoBehaviour
{
    private InputScheme _scheme;
    public InputScheme Scheme {
        get {
            if (_scheme == null)
                _scheme = new InputScheme();
            return _scheme;
        }
    }

    public InputAction Move => Scheme.Player.Move;
    public InputAction WeaponAttack1 => Scheme.Player.WeaponAttack1;
    public InputAction Dash => Scheme.Player.Dash;


    void OnEnable()
    {
        _scheme.Enable();
    }

    void OnDisable()
    {
        _scheme.Disable();
    }
}
