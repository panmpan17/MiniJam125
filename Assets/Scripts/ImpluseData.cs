using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName="Game/Impluse Data")]
public class ImpluseData : ScriptableObject
{
    public Vector2 Delta;
    public float Interval;
    public float Duration;
    public int Priority;
}
