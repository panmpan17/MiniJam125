using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public class PlayerAnimator : MonoBehaviour
{
    [SerializeField]
    private PlayerBehaviour behaviour;
    [SerializeField]
    private PlayerMovement movement;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Sprite upSprite;
    [SerializeField]
    private Sprite downSprite;
    [SerializeField]
    private Sprite sideSprite;

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
        movement.OnFacingChanged += OnFacingChanged;
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

    void OnFacingChanged(PlayerMovement.Facing facing)
    {
        switch (facing)
        {
            case PlayerMovement.Facing.Up:
                spriteRenderer.sprite = upSprite;
                transform.rotation = Quaternion.identity;
                break;
            case PlayerMovement.Facing.Down:
                spriteRenderer.sprite = downSprite;
                transform.rotation = Quaternion.identity;
                break;
            case PlayerMovement.Facing.Left:
                spriteRenderer.sprite = sideSprite;
                transform.localScale = Vector3.one;
                transform.rotation = Quaternion.identity;
                break;
            case PlayerMovement.Facing.Right:
                spriteRenderer.sprite = sideSprite;
                transform.localScale = new Vector3(-1, 1, 1);
                transform.rotation = Quaternion.identity;
                break;
        }
    }
}
