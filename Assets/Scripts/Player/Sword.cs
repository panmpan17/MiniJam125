using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;
using MPack;


public abstract class AbstractWeapon : MonoBehaviour
{
    public abstract void OnAttackStarted(CallbackContext callbackContext);
    public abstract void OnAttackPerformed(CallbackContext callbackContext);
    public abstract void OnAttackCanceled(CallbackContext callbackContext);
}

public class Sword : AbstractWeapon
{
    [SerializeField]
    private Transform swordSprite;
    [SerializeField]
    private Transform swingEffect;
    [SerializeField]
    private TrailRenderer swingTrail;
    [SerializeField]
    private LayerMaskReference layerMaskReference;
    [SerializeField]
    private Collider2D attackCollider;
    private ContactFilter2D _contactFilter2D;
    private Collider2D[] _attackTargetColliders = new Collider2D[5];

    [SerializeField]
    private SwingPose restPose;

    [SerializeField]
    private SwingPose[] poses;
    private SwingPose _startPos;
    private SwingPose _endPos;

    [SerializeField]
    private SwingAnimation[] swingPoses;
    private int _swingAnimationIndex;
    private Timer timer;
    // [SerializeField]
    // private Timer allSwingEndRestTimer;
    [SerializeField]
    private float allSwingPoseEndRest;

    private SwordSwingState _swingState;


    void Awake()
    {
        swingEffect.gameObject.SetActive(false);
        swingTrail.emitting = false;

        swordSprite.localPosition = restPose.SwordLocalPosition;
        swordSprite.localRotation = Quaternion.Euler(restPose.SwordLocalRotation);

        _contactFilter2D = new ContactFilter2D {
            useTriggers = false,
            useLayerMask = true,
            layerMask = layerMaskReference.Value,
        };
    }

    void Update()
    {
        switch (_swingState)
        {
            case SwordSwingState.None:
                break;

            case SwordSwingState.Swing:
                swordSprite.localPosition = Vector3.Lerp(_startPos.SwordLocalPosition, _endPos.SwordLocalPosition, timer.Progress);
                swordSprite.localRotation = Quaternion.Euler(Vector3.Lerp(_startPos.SwordLocalRotation, _endPos.SwordLocalRotation, timer.Progress));

                if (!timer.UpdateEnd)
                    break;
                
                swingEffect.gameObject.SetActive(false);
                swingTrail.emitting = false;

                if (_swingAnimationIndex == swingPoses.Length - 1)
                {
                    _swingState = SwordSwingState.WaitColddown;
                    timer.TargetTime = allSwingPoseEndRest;
                    timer.Reset();
                    break;
                }

                _swingState = SwordSwingState.WaitNextSwing;
                timer.TargetTime = swingPoses[_swingAnimationIndex].WaitNextSwingDetect;
                timer.Reset();
                break;

            case SwordSwingState.WaitNextSwing:
            case SwordSwingState.WaitColddown:
                if (!timer.UpdateEnd)
                    break;

                _swingState = SwordSwingState.None;

                swordSprite.localPosition = restPose.SwordLocalPosition;
                swordSprite.localRotation = Quaternion.Euler(restPose.SwordLocalRotation);

                swingEffect.gameObject.SetActive(false);
                swingTrail.emitting = false;
                break;
        }
    }


    public override void OnAttackStarted(CallbackContext callbackContext)
    {
        SwingAnimation animation;
        switch (_swingState)
        {
            case SwordSwingState.None:
                _swingState = SwordSwingState.Swing;

                _swingAnimationIndex = 0;
                animation = swingPoses[_swingAnimationIndex];
                SetPose(animation);
                Attack(animation.DamageMultiplier);
                break;

            case SwordSwingState.WaitNextSwing:
                _swingState = SwordSwingState.Swing;

                _swingAnimationIndex += 1;
                animation = swingPoses[_swingAnimationIndex];
                SetPose(animation);
                Attack(animation.DamageMultiplier);
                break;
        }
    }

    private void Attack(float damageMultiplier)
    {
        Physics2D.OverlapCollider(attackCollider, _contactFilter2D, _attackTargetColliders);
        for (int i = 0; i < _attackTargetColliders.Length; i++)
        {
            if (_attackTargetColliders[i] == null)
                return;

            var body = _attackTargetColliders[i].GetComponent<BossBody>();
            if (body)
                body.OnDamage(damageMultiplier);
        }
    }

    void SetPose(SwingAnimation animation)
    {
        _startPos = poses[animation.StartPoseIndex];
        _endPos = poses[animation.EndPoseIndex];

        swordSprite.localPosition = _startPos.SwordLocalPosition;
        swordSprite.localRotation = Quaternion.Euler(_startPos.SwordLocalRotation);
        swingEffect.localScale = _startPos.SwingEffectLocalScale;

        swingEffect.gameObject.SetActive(true);
        swingTrail.emitting = true;

        timer.TargetTime = animation.SwingWait;
        timer.Reset();
    }

    public override void OnAttackPerformed(CallbackContext callbackContext)
    {
    }

    public override void OnAttackCanceled(CallbackContext callbackContext)
    {
    }

    public enum SwordSwingState
    {
        None, Swing, WaitNextSwing, WaitColddown
    }

    [System.Serializable]
    public struct SwingPose
    {
        public Vector3 SwordLocalPosition;
        public Vector3 SwordLocalRotation;

        public Vector3 SwingEffectLocalScale;
    }

    [System.Serializable]
    public struct SwingAnimation
    {
        public int StartPoseIndex;
        public int EndPoseIndex;

        public float SwingWait;
        public float WaitNextSwingDetect;
        [Range(0.5f, 1.5f)]
        public float DamageMultiplier;
    }
}
