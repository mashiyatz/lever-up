using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioHandling : MonoBehaviour
{
    bool idlePlay;
    bool startUpPlay;
    [SerializeField] private AudioSource gameAudioSource;

    [SerializeField] private AudioClip[] JaegerSounds;

    //sounds are as follows:
    //    0. engineidle_loop
    //    1. cannonfire1
    //    2. cannonfire2
    //    3. loadcannon_pullback
    //    4. loadcannon_pushforward
    //    5. leftturn
    //    6. rightturn
    //    7. leftstep
    //    8. rightstep
    //    9. enginestart
    //    10. enemydeath

    void Start()
    {
        gameAudioSource = GetComponent<AudioSource>();
        idlePlay = true;
    }

    private void OnEnable()
    {
        GameManager.OnPlay += () => gameAudioSource.Play();
        BulletBehavior.OnEnemyDeath += PlayEnemyDeath;
    }

    private void OnDisable()
    {
        GameManager.OnPlay -= () => gameAudioSource.Play();
        BulletBehavior.OnEnemyDeath -= PlayEnemyDeath;
    }

    void PlayEnemyDeath()
    {
        gameAudioSource.PlayOneShot(JaegerSounds[10]);
    }
}
