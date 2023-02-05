using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public class ImpluseCamera : MonoBehaviour
{
    public static ImpluseCamera ins;

    [SerializeField]
    private ImpluseData testImpluseData;

    private Vector3 basePosition;

    private Vector2 _delta;
    private Timer _intervalTimer = new Timer(0);
    private Timer _durationTimer = new Timer(0);
    private int _priority;

    void Awake()
    {
        ins = this;
        basePosition = transform.position;
    }

    void Update()
    {
        if (_durationTimer.UpdateEnd)
        {
            enabled = false;
            transform.position = basePosition;
            return;
        }

        if (!_intervalTimer.UpdateEnd)
            return;
        _intervalTimer.ContinuousReset();

        Vector3 position = basePosition;
        position.x += Random.Range(-_delta.x, _delta.x);
        position.y += Random.Range(-_delta.y, _delta.y);
        transform.position = position;
    }


    // public void GenerateImpluse(Vector2 delta, float interval, float duration)
    // {
    //     _delta = delta;
    //     _intervalTimer.TargetTime = interval;
    //     _durationTimer.TargetTime = duration;
    //     _intervalTimer.Reset();
    //     _durationTimer.Reset();
    //     enabled = true;
    // }

    public void GenerateImpluse(ImpluseData impluseData)
    {
        if (enabled)
        {
            if (impluseData.Priority < _priority)
            {
                Debug.Log(impluseData);
                return;
            }
        }

        _delta = impluseData.Delta;
        _intervalTimer.TargetTime = impluseData.Interval;
        _durationTimer.TargetTime = impluseData.Duration;
        _priority = impluseData.Priority;
        _intervalTimer.Reset();
        _durationTimer.Reset();
        enabled = true;
    }


#if UNITY_EDITOR
    void OnValidate()
    {
        if (testImpluseData != null)
        {
            GenerateImpluse(testImpluseData);
            testImpluseData = null;
        }
    }
#endif
}
