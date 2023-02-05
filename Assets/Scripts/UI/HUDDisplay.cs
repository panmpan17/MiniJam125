using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MPack;


public class HUDDisplay : MonoBehaviour
{
    [Header("Health")]
    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private int healthBarMaxAmount;
    [SerializeField]
    private IntEventReference healthChangedEvent;

    [Header("Energy")]
    [SerializeField]
    private Image energyBar;
    [SerializeField]
    private int energyBarMaxAmount;
    [SerializeField]
    private IntEventReference energyChangedEvent;
    private Coroutine _healthCoroutine;

    [Header("Animation")]
    [SerializeField]
    private float increaseAnimTime;
    [SerializeField]
    private float decreaseAnimTime;
    private Coroutine _energyCoroutine;

    [SerializeField]
    private float gradeintIntervalTime;
    [SerializeField]
    private Gradient increaseGradient;
    [SerializeField]
    private Gradient decreaseGradient;

    void OnEnable()
    {
        healthChangedEvent.RegisterEvent(OnHealthChanged);
        energyChangedEvent.RegisterEvent(OnEnergyChanged);
    }
    void OnDisable()
    {
        healthChangedEvent.UnregisterEvent(OnHealthChanged);
        energyChangedEvent.UnregisterEvent(OnEnergyChanged);
    }

    void OnHealthChanged(int healthAmount)
    {
        float amount = (float)healthAmount / (float)healthBarMaxAmount;

        if (_healthCoroutine != null) StopCoroutine(_healthCoroutine);
        _healthCoroutine = StartCoroutine(FillAmountCoroutine(healthBar, amount));
    }

    void OnEnergyChanged(int evergyAmount)
    {
        float amount = (float)evergyAmount / (float)energyBarMaxAmount;

        if (_energyCoroutine != null) StopCoroutine(_energyCoroutine);
        _energyCoroutine = StartCoroutine(FillAmountCoroutine(energyBar, amount));
    }

    IEnumerator FillAmountCoroutine(Image fillImage, float toAmount)
    {
        float fromAmount = fillImage.fillAmount;

        bool increase = toAmount > fromAmount;
        float duration = increase ? increaseAnimTime : decreaseAnimTime;
        Gradient gradient = increase ? increaseGradient : decreaseGradient;

        float timer = 0;
        Timer intervalTimer = new Timer(gradeintIntervalTime);

        while (timer < duration)
        {
            timer += Time.deltaTime;

            fillImage.color = gradient.Evaluate(intervalTimer.Progress);
            if (intervalTimer.UpdateEnd)
                intervalTimer.ReverseMode = !intervalTimer.ReverseMode;

            fillImage.fillAmount = Mathf.Lerp(fromAmount, toAmount, timer / duration);


            yield return null;
        }

        fillImage.color = Color.white;
    }
}
