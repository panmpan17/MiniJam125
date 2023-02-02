using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class BossBehaviour : MonoBehaviour
{
    [SerializeField]
    private FanSpreadBullet fanSpreadBullet;

    private Timer timer = new Timer(6);

    void Start()
    {
        fanSpreadBullet.TriggerFire();
    }

    void Update()
    {
        if (timer.UpdateEnd)
        {
            timer.Reset();
            fanSpreadBullet.TriggerFire();
        }
    }
}
