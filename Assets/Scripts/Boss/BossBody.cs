using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class BossBody : MonoBehaviour
{
    [SerializeField]
    private float damangeAmount;
    public float DamageAmount { get => damangeAmount; set { damangeAmount = value; } }
    [SerializeField]
    private FloatEventReference bossDamageEvent;

    public void OnDamage(float multiplier=1f)
    {
        bossDamageEvent.Invoke(damangeAmount * multiplier);
    }
}
