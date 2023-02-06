using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager ins;

    [SerializeField]
    private AudioSource playerAudioSource;
    [SerializeField]
    private AudioSource enemyAudioSource;

    void Awake()
    {
        ins = this;
    }


    public void Play(AudioClip clip, float volume, bool _isPlayer)
    {
        if (_isPlayer)
            playerAudioSource.PlayOneShot(clip, volume);
        else
            enemyAudioSource.PlayOneShot(clip, volume);
    }
}
