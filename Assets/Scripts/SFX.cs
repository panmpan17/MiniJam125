using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[CreateAssetMenu(menuName="Game/SFX")]
public class SFX : ScriptableObject
{
    public AudioClip Clip;
    [Range(0, 1)]
    public float Volume = 1;
    public bool IsPlayer;

    public void Play()
    {
        if (Clip)
            SFXManager.ins.Play(Clip, Volume, IsPlayer);
    }
}
