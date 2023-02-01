using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    void OnEnable()
    {
        _scheme.Enable();
    }

    void OnDisable()
    {
        _scheme.Disable();
    }
}
