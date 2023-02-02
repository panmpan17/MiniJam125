using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using XnodeBehaviourTree;


[RequireComponent(typeof(BehaviourTreeRunner))]
public class BossBehaviour : MonoBehaviour
{
    [SerializeField]
    private BehaviourTreeRunner behaviourTreeRunner;
    [SerializeField]
    private AbstractTriggerFire[] triggerFires;

    private Timer timer = new Timer(6);

    void Awake()
    {
        behaviourTreeRunner.OnTriggerFire += OnTriggerFire;
    }

    void OnTriggerFire(int index)
    {
        triggerFires[index].TriggerFire();
    }
}
