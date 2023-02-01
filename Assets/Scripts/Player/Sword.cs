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
    private Collider2D attackCollider;

    [SerializeField]
    private SwingPose restPose;

    [SerializeField]
    private SwingPose[] swingPoses;
    private int _swingPoseIndex;
    private Timer timer;
    // [SerializeField]
    // private Timer allSwingEndRestTimer;
    [SerializeField]
    private float allSwingPoseEndRest;

    private SwordSwingState _swingState;


    void Awake()
    {
        swingEffect.gameObject.SetActive(false);

        swordSprite.localPosition = restPose.SwordLocalPosition;
        swordSprite.localRotation = Quaternion.Euler(restPose.SwordLocalRotation);
    }

    void Update()
    {
        switch (_swingState)
        {
            case SwordSwingState.None:
                break;

            case SwordSwingState.Swing:
                if (!timer.UpdateEnd)
                    break;
                
                swingEffect.gameObject.SetActive(false);

                if (_swingPoseIndex == swingPoses.Length - 1)
                {
                    _swingState = SwordSwingState.WaitColddown;
                    timer.TargetTime = allSwingPoseEndRest;
                    timer.Reset();
                    break;
                }

                _swingState = SwordSwingState.WaitNextSwing;
                timer.TargetTime = swingPoses[_swingPoseIndex].WaitNextSwingDetect;
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
                break;
        }
    }


    public override void OnAttackStarted(CallbackContext callbackContext)
    {
        switch (_swingState)
        {
            case SwordSwingState.None:
                _swingState = SwordSwingState.Swing;

                _swingPoseIndex = 0;
                SetPose(swingPoses[_swingPoseIndex]);
                break;

            case SwordSwingState.WaitNextSwing:
                _swingState = SwordSwingState.Swing;

                _swingPoseIndex += 1;
                SetPose(swingPoses[_swingPoseIndex]);
                break;
        }
    }

    void SetPose(SwingPose pose)
    {
        swordSprite.localPosition = pose.SwordLocalPosition;
        swordSprite.localRotation = Quaternion.Euler(pose.SwordLocalRotation);

        swingEffect.localScale = pose.SwingEffectLocalScale;
        swingEffect.gameObject.SetActive(true);

        timer.TargetTime = pose.SwingWait;
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
        // public Vector3 SwordLocalRotation;

        public float SwingWait;
        public float WaitNextSwingDetect;
    }
}
