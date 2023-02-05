using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MPack;


public class HUDDisplay : MonoBehaviour
{
    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private int healthBarMaxAmount;
    [SerializeField]
    private IntEventReference healthChangedEvent;

    [SerializeField]
    private Image energyBar;
    [SerializeField]
    private int energyBarMaxAmount;
    [SerializeField]
    private IntEventReference energyChangedEvent;

    void OnEnable()
    {
        healthChangedEvent.RegisterEvent(OnHealthChanged);
        energyChangedEvent.RegisterEvent(OnEnergyChanged);
    }
    void OnDisaable()
    {
        healthChangedEvent.UnregisterEvent(OnHealthChanged);
        energyChangedEvent.UnregisterEvent(OnEnergyChanged);
    }

    void OnHealthChanged(int healthAmount)
    {
        healthBar.fillAmount = (float)healthAmount / (float)healthBarMaxAmount;
    }

    void OnEnergyChanged(int evergyAmount)
    {
        energyBar.fillAmount = (float)evergyAmount / (float)energyBarMaxAmount;
    }
}
