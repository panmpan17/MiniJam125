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

    [Header("Health")]
    [SerializeField]
    private float maxHealthPoint;
    private float _healthPoint;
    [SerializeField]
    private FloatEventReference onDamageEvent;
    [SerializeField]
    private FloatEventReference healthPointPercentageUpdateEvent;

    private Timer timer = new Timer(6);

    void Awake()
    {
        behaviourTreeRunner.OnTriggerFire += OnTriggerFire;

        _healthPoint = maxHealthPoint;
        healthPointPercentageUpdateEvent.Invoke(1);
    }

    void OnEnable()
    {
        onDamageEvent.RegisterEvent(OnDamage);
    }
    void OnDisable()
    {
        onDamageEvent.UnregisterEvent(OnDamage);
    }

    void OnTriggerFire(int index)
    {
        triggerFires[index].TriggerFire();
    }

    void OnDamage(float amount)
    {
        _healthPoint -= amount;

        if (_healthPoint < 0)
        {
            gameObject.SetActive(false);
            healthPointPercentageUpdateEvent.Invoke(0);
            return;
        }

        healthPointPercentageUpdateEvent.Invoke(_healthPoint / maxHealthPoint);
    }
}
