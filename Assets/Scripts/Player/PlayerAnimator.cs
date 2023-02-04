using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public class PlayerAnimator : MonoBehaviour
{
    [SerializeField]
    private PlayerBehaviour behaviour;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [Header("Hurt")]
    [SerializeField]
    private Timer hurtTimer;
    [SerializeField]
    private Timer hurtIntervalTimer;
    [SerializeField]
    private Color hurColorA;
    [SerializeField]
    private Color hurColorB;

    void OnEnable()
    {
        behaviour.OnHurt += OnHurt;
    }

    void Update()
    {
        HandleHurt();
    }

    void HandleHurt()
    {
        if (!hurtTimer.Running)
            return;

        if (hurtTimer.UpdateEnd)
        {
            spriteRenderer.color = Color.white;
            hurtTimer.Running = false;
            return;
        }

        spriteRenderer.color = Color.Lerp(hurColorA, hurColorB, hurtIntervalTimer.Progress);
        if (hurtIntervalTimer.UpdateEnd)
        {
            hurtIntervalTimer.ReverseMode = !hurtIntervalTimer.ReverseMode;
        }
    }

    void OnHurt()
    {
        hurtTimer.Reset();
    }
}
