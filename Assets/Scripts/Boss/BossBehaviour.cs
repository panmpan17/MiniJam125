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
    private BossEyeBall eyeBall;
    [SerializeField]
    private AbstractTriggerFire[] triggerFires;
    [SerializeField]
    private BossAttackMode[] attackModes;

    [Header("Health")]
    [SerializeField]
    private float maxHealthPoint;
    private float _healthPoint;
    [SerializeField]
    private FloatEventReference onDamageEvent;
    [SerializeField]
    private FloatEventReference healthPointPercentageUpdateEvent;

    [Header("Stage")]
    [SerializeField]
    private float stage2Health;
    [SerializeField]
    private float stage3Health;
    private int stage = 0;
    [SerializeField]
    private IntEventReference stageEvent;

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
        stageEvent.RegisterEvent(behaviourTreeRunner.SetStage);
    }
    void OnDisable()
    {
        onDamageEvent.UnregisterEvent(OnDamage);
        stageEvent.UnregisterEvent(behaviourTreeRunner.SetStage);
    }

    void OnTriggerFire(int index)
    {
        triggerFires[index].TriggerFire();
        eyeBall.ChangeToMode((BossAttackMode) index);
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

        if (stage == 0)
        {
            if (_healthPoint <= stage2Health)
            {
                stage = 1;
                stageEvent.Invoke(stage);
            }
        }
        else if (stage == 1)
        {
            if (_healthPoint <= stage3Health)
            {
                stage = 2;
                stageEvent.Invoke(stage);
            }
        }

        healthPointPercentageUpdateEvent.Invoke(_healthPoint / maxHealthPoint);
    }

    public enum BossAttackMode { Bullet, Bomb, Tentacle, Canon }
}
