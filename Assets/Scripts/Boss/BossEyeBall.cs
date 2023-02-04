using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class BossEyeBall : MonoBehaviour
{
    [Header("Attack Hint")]
    [SerializeField]
    private GameObject canonPattern;
    [SerializeField]
    private GameObject tentaclePattern;
    [SerializeField]
    private GameObject bombPattern;
    [SerializeField]
    private GameObject bulletPattern;
    [SerializeField]
    private float hideTime;

    [Header("Blood")]
    [SerializeField]
    private GameObject blood1;
    [SerializeField]
    private GameObject blood2;

    [Header("Pupil")]
    [SerializeField]
    private Transform target;
    [SerializeField]
    private RangeReference targetEdge;
    [SerializeField]
    private GameObject leftPupil;
    [SerializeField]
    private GameObject centerPupil;
    [SerializeField]
    private GameObject rightPupil;
    private EyeTargetPosition _eyeTarget;

    [SerializeField]
    private IntEventReference stageEvent;

    [Header("Eye White")]
    [SerializeField]
    private SpriteRenderer eyeWhite;
    [SerializeField]
    private Sprite[] leftEyeWhiteSprites;
    [SerializeField]
    private Sprite[] centerEyeWhiteSprites;
    [SerializeField]
    private Sprite[] rightEyeWhiteSprites;
    private int _eyeWhiteIndex = 0;

    void OnEnable()
    {
        stageEvent.RegisterEvent(OnStageChanged);
    }
    void ONDisable()
    {
        stageEvent.UnregisterEvent(OnStageChanged);
    }

    void FixedUpdate()
    {
        float x = target.position.x;
        EyeTargetPosition newEyeTarget = EyeTargetPosition.Center;
        if (x <= transform.position.x + targetEdge.Min) newEyeTarget = EyeTargetPosition.Left;
        else if (x >= transform.position.x + targetEdge.Max) newEyeTarget = EyeTargetPosition.Right;

        if (newEyeTarget != _eyeTarget)
        {
            leftPupil.SetActive(false);
            centerPupil.SetActive(false);
            rightPupil.SetActive(false);

            switch (newEyeTarget)
            {
                case EyeTargetPosition.Left:
                    eyeWhite.sprite = leftEyeWhiteSprites[_eyeWhiteIndex];
                    leftPupil.SetActive(true);
                    break;
                case EyeTargetPosition.Center:
                    eyeWhite.sprite = centerEyeWhiteSprites[_eyeWhiteIndex];
                    centerPupil.SetActive(true);
                    break;
                case EyeTargetPosition.Right:
                    eyeWhite.sprite = rightEyeWhiteSprites[_eyeWhiteIndex];
                    rightPupil.SetActive(true);
                    break;
            }
            _eyeTarget = newEyeTarget;
        }
    }

    void OnStageChanged(int stageIndex)
    {
        _eyeWhiteIndex = stageIndex;

        if (_eyeWhiteIndex == 1) blood1.SetActive(true);
        if (_eyeWhiteIndex == 2) blood2.SetActive(true);

        switch (_eyeTarget)
        {
            case EyeTargetPosition.Left:
                eyeWhite.sprite = leftEyeWhiteSprites[_eyeWhiteIndex];
                break;
            case EyeTargetPosition.Center:
                eyeWhite.sprite = centerEyeWhiteSprites[_eyeWhiteIndex];
                break;
            case EyeTargetPosition.Right:
                eyeWhite.sprite = rightEyeWhiteSprites[_eyeWhiteIndex];
                break;
        }
    }

    public void ChangeToMode(BossBehaviour.BossAttackMode attackMode)
    {
        canonPattern.SetActive(false);
        tentaclePattern.SetActive(false);
        bombPattern.SetActive(false);
        bulletPattern.SetActive(false);

        switch (attackMode)
        {
            case BossBehaviour.BossAttackMode.Bullet:
                bulletPattern.SetActive(true);
                StartCoroutine(SetActive(bulletPattern, false, hideTime));
                break;

            case BossBehaviour.BossAttackMode.Bomb:
                bombPattern.SetActive(true);
                StartCoroutine(SetActive(bombPattern, false, hideTime));
                break;

            case BossBehaviour.BossAttackMode.Tentacle:
                tentaclePattern.SetActive(true);
                StartCoroutine(SetActive(tentaclePattern, false, hideTime));
                break;

            case BossBehaviour.BossAttackMode.Canon:
                canonPattern.SetActive(true);
                StartCoroutine(SetActive(canonPattern, false, hideTime));
                break;
        }
    }

    IEnumerator SetActive(GameObject gameObject, bool value, float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(value);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position + new Vector3(targetEdge.Min, 0, 0), transform.position + new Vector3(targetEdge.Min, -3, 0));
        Gizmos.DrawLine(transform.position + new Vector3(targetEdge.Max, 0, 0), transform.position + new Vector3(targetEdge.Max, -3, 0));
    }

    private enum EyeTargetPosition { Left, Center, Right }
}
