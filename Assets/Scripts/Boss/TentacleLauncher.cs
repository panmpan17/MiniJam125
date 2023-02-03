using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class TentacleLauncher : AbstractTriggerFire
{
    [SerializeField]
    private int launchCount;
    private int _launchCount;
    [SerializeField]
    private Timer launchIntervalTimer;
    [SerializeField]
    private bool launchRightAfterTrigger;

    // [Header("Rule")]
    // [SerializeField]
    // private bool horizontalVertcalMixed;
    private bool _lastIsHorizontal = false;
    // [SerializeField]
    // private bool leftRightMixed;
    private bool _lastIsLeft = false;
    // [SerializeField]
    // private bool upDownMixed;
    private bool _lastIsUp = false;

    [Header("Horizontal")]
    [SerializeField]
    private Tentacle[] horizontalTentacles;
    [SerializeField]
    private RangeReference leftToRightX;
    [SerializeField]
    private RangeReference rightToLeftX;
    [SerializeField]
    private RangeReference heightRange;

    [Header("Vertical")]
    [SerializeField]
    private Tentacle[] verticalTentacles;
    [SerializeField]
    private RangeReference upToDownY;
    [SerializeField]
    private RangeReference downToUpY;
    [SerializeField]
    private RangeReference widthRange;


    void Update()
    {
        if (!launchIntervalTimer.UpdateEnd)
            return;
        launchIntervalTimer.ContinuousReset();

        Launch();
    }


    void Launch()
    {
        if (_lastIsHorizontal)
            LaunchVertical();
        else
            LaunchHorizontal();
        _lastIsHorizontal = !_lastIsHorizontal;

        if (++_launchCount >= launchCount)
        {
            enabled = false;
        }
    }

    void LaunchHorizontal()
    {
        float startX;
        float endX;
        float scaleX;

        if (_lastIsLeft)
        {
            startX = rightToLeftX.Min;
            endX = rightToLeftX.Max;
            scaleX = -1;
        }
        else
        {
            startX = leftToRightX.Min;
            endX = leftToRightX.Max;
            scaleX = 1;
        }
        _lastIsLeft = !_lastIsLeft;

        for (int i = 0; i < horizontalTentacles.Length; i++)
        {
            if (horizontalTentacles[i].gameObject.activeSelf)
                continue;

            float y = heightRange.PickRandomNumber();
            horizontalTentacles[i].Place(
                new Vector3(startX, y), new Vector3(endX, y), new Vector3(scaleX, 1, 1));
            break;
        }
    }
    void LaunchVertical()
    {
        float startY;
        float endY;
        float scaleY;

        if (_lastIsUp)
        {
            startY = downToUpY.Min;
            endY = downToUpY.Max;
            scaleY = -1;
        }
        else
        {
            startY = upToDownY.Min;
            endY = upToDownY.Max;
            scaleY = 1;
        }
        _lastIsUp = !_lastIsUp;

        for (int i = 0; i < verticalTentacles.Length; i++)
        {
            if (verticalTentacles[i].gameObject.activeSelf)
                continue;

            float x = widthRange.PickRandomNumber();
            verticalTentacles[i].Place(
                new Vector3(x, startY), new Vector3(x, endY), new Vector3(1, scaleY, 1));
            break;
        }
    }


    public override void TriggerFire()
    {
        if (enabled)
            return;
        enabled = true;

        _launchCount = 0;
        if (launchRightAfterTrigger)
            Launch();

        launchIntervalTimer.Reset();
    }
}
