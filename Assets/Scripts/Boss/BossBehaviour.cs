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
    private float stageChangedWaitTime;
    [SerializeField]
    private float stage2Health;
    [SerializeField]
    private float stage3Health;
    private int _stage = 0;
    [SerializeField]
    private IntEventReference stageEvent;

    private Timer timer = new Timer(6);

    void Awake()
    {
        behaviourTreeRunner.OnTriggerFire += OnTriggerFire;
        behaviourTreeRunner.OnOutsideFunctionCalled += OnFunctionCalled;

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

    void OnFunctionCalled(string functionName)
    {
        // Debug.Log(functionName);
        switch (functionName)
        {
            case "EyeDrop":
                eyeBall.Drop();
                break;

            case "EyeRaise":
                eyeBall.Raise();
                break;
        }
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

        if (_stage == 0)
        {
            if (_healthPoint <= stage2Health)
                ChangeStage(1);
        }
        else if (_stage == 1)
        {
            if (_healthPoint <= stage3Health)
                ChangeStage(2);
        }

        healthPointPercentageUpdateEvent.Invoke(_healthPoint / maxHealthPoint);
    }

    void ChangeStage(int stage)
    {
        _stage = stage;
        stageEvent.Invoke(_stage);
        StartCoroutine(PauseBehaviourRunner());
    }

    IEnumerator PauseBehaviourRunner()
    {
        behaviourTreeRunner.enabled = false;
        yield return new WaitForSeconds(stageChangedWaitTime);
        behaviourTreeRunner.enabled = true;
    }


    public enum BossAttackMode { Bullet, Bomb, Tentacle, Canon }
}
