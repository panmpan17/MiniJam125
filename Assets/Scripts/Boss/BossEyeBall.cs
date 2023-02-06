using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class BossEyeBall : MonoBehaviour
{
    [Header("Drop & Raise")]
    [SerializeField]
    private BossBody body;
    [SerializeField]
    private Vector3 raisedPosition;
    [SerializeField]
    private float raisedDamageAmount;
    [SerializeField]
    private Vector3 droppedPosition;
    [SerializeField]
    private float droppedDamageAmount;
    private bool _isDropped = false;

    [SerializeField]
    private ImpluseData dropImpulse;
    [SerializeField]
    private ImpluseData raiseImpulse;

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
    private Timer patternStayTimer;
    private Coroutine _patternDisplayCoroutine;

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

    [Header("Eyelid")]
    [SerializeField]
    private SpriteRenderer eyelid;
    [SerializeField]
    private Sprite[] eyelidSprites;
    [SerializeField]
    private Timer eyelidSwitchTimer;
    private int eyelidIndex = 0;

    void OnEnable()
    {
        stageEvent.RegisterEvent(OnStageChanged);
    }
    void OnDisable()
    {
        stageEvent.UnregisterEvent(OnStageChanged);
    }

    void FixedUpdate()
    {
        float x = target.position.x;
        EyeTargetPosition newEyeTarget = EyeTargetPosition.Center;
        if (x <= transform.position.x + targetEdge.Min) newEyeTarget = EyeTargetPosition.Left;
        else if (x >= transform.position.x + targetEdge.Max) newEyeTarget = EyeTargetPosition.Right;

        if (newEyeTarget == _eyeTarget)
            return;
        
        if (_patternDisplayCoroutine == null)
            MatchEyeWhitePupilToTargetPosition(newEyeTarget);
        _eyeTarget = newEyeTarget;
    }

    private void MatchEyeWhitePupilToTargetPosition(EyeTargetPosition newEyeTarget)
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
    }

    void OnStageChanged(int stageIndex)
    {
        _eyeWhiteIndex = stageIndex;

        if (_eyeWhiteIndex == 1) blood1.SetActive(true);
        if (_eyeWhiteIndex == 2) blood2.SetActive(true);

        MatchEyeWhitePupilToTargetPosition(_eyeTarget);

        if (_isDropped)
        {
            Raise();
        }
    }

    public void ChangeToMode(BossBehaviour.BossAttackMode attackMode)
    {
        if (_patternDisplayCoroutine != null)
        {
            StopCoroutine(_patternDisplayCoroutine);
        }

        switch (attackMode)
        {
            case BossBehaviour.BossAttackMode.Bullet:
                _patternDisplayCoroutine = StartCoroutine(DisplayPattern(bulletPattern));
                break;

            case BossBehaviour.BossAttackMode.Bomb:
                _patternDisplayCoroutine = StartCoroutine(DisplayPattern(bombPattern));
                break;

            case BossBehaviour.BossAttackMode.Tentacle:
                _patternDisplayCoroutine = StartCoroutine(DisplayPattern(tentaclePattern));
                break;

            case BossBehaviour.BossAttackMode.Canon:
                _patternDisplayCoroutine = StartCoroutine(DisplayPattern(canonPattern));
                break;
        }
    }

    IEnumerator DisplayPattern(GameObject pattern)
    {
        WaitForSeconds wait = new WaitForSeconds(eyelidSwitchTimer.TargetTime);
        WaitForSeconds wait2 = new WaitForSeconds(patternStayTimer.TargetTime);

        eyelid.gameObject.SetActive(true);
        eyelid.sprite = eyelidSprites[0];
        yield return wait;
        eyelid.sprite = eyelidSprites[1];
        yield return wait;
        eyelid.sprite = eyelidSprites[2];

        eyeWhite.sprite = centerEyeWhiteSprites[_eyeWhiteIndex];

        leftPupil.SetActive(false);
        centerPupil.SetActive(false);
        rightPupil.SetActive(false);

        canonPattern.SetActive(false);
        tentaclePattern.SetActive(false);
        bombPattern.SetActive(false);
        bulletPattern.SetActive(false);
        pattern.SetActive(true);

        yield return wait;
        eyelid.sprite = eyelidSprites[1];
        yield return wait;
        eyelid.sprite = eyelidSprites[0];
        eyelid.gameObject.SetActive(false);

        yield return wait2;

        eyelid.gameObject.SetActive(true);
        eyelid.sprite = eyelidSprites[0];
        yield return wait;
        eyelid.sprite = eyelidSprites[1];
        yield return wait;
        eyelid.sprite = eyelidSprites[2];

        MatchEyeWhitePupilToTargetPosition(_eyeTarget);
        pattern.SetActive(false);

        yield return wait;
        eyelid.sprite = eyelidSprites[1];
        yield return wait;
        eyelid.sprite = eyelidSprites[0];
        eyelid.gameObject.SetActive(false);


        _patternDisplayCoroutine = null;
    }


    public void Drop()
    {
        transform.position = droppedPosition;
        _isDropped = true;
        body.DamageAmount = droppedDamageAmount;

        if (dropImpulse) ImpluseCamera.ins.GenerateImpluse(dropImpulse);
    }

    public void Raise()
    {
        transform.position = raisedPosition;
        _isDropped = false;
        body.DamageAmount = raisedDamageAmount;

        if (raiseImpulse) ImpluseCamera.ins.GenerateImpluse(raiseImpulse);
    }

    public void HidePupil()
    {
        leftPupil.SetActive(false);
        centerPupil.SetActive(false);
        rightPupil.SetActive(false);
        bulletPattern.SetActive(false);
        tentaclePattern.SetActive(false);
        canonPattern.SetActive(false);
        bombPattern.SetActive(false);
        eyelid.gameObject.SetActive(false);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position + new Vector3(targetEdge.Min, 0, 0), transform.position + new Vector3(targetEdge.Min, -3, 0));
        Gizmos.DrawLine(transform.position + new Vector3(targetEdge.Max, 0, 0), transform.position + new Vector3(targetEdge.Max, -3, 0));
    }

    private enum EyeTargetPosition { Left, Center, Right }
}
