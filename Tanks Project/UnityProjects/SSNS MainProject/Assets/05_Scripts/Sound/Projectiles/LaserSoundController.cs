using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This controller will change the audio source of the laser object based on when it is turned on or not.
/// </summary>
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(LaserBehaviour))]
public class LaserSoundController : MonoBehaviour
{
    [SerializeField] AudioSource StartStopSoundSource;
    [SerializeField] AudioSource constantSoundSource;

    [SerializeField] AudioClip startSound;
    [SerializeField] AudioClip disableSound;

    LaserBehaviour laser;
    bool startLaser = false;

    private void Start()
    {
        laser = GetComponent<LaserBehaviour>();
        //StartCoroutine(FadeLaserSound());
    }

    private void Update()
    {
        if(laser.fadeIn != startLaser)
        {
            startLaser = laser.fadeIn;

            if (startLaser)
                StartStopSoundSource.clip = startSound;
            else
                StartStopSoundSource.clip = disableSound;

            StartStopSoundSource.Play();
        }

        //Fade laser sounds
        if (startLaser)
        {
            constantSoundSource.volume = Mathf.Clamp(constantSoundSource.volume + 0.7f * Time.deltaTime, 0, 1);
        }
        else
        {
            constantSoundSource.volume = Mathf.Clamp(constantSoundSource.volume - 0.7f * Time.deltaTime, 0, 1);
        }
    }

    //IEnumerator FadeLaserSound()
    //{

    //    while (true)
    //    {
    //        yield return null;
    //        if(startLaser)
    //        {
    //            constantSoundSource.volume = Mathf.Clamp(constantSoundSource.volume + 0.7f * Time.deltaTime, 0, 1);
    //        }
    //        else
    //        {
    //            constantSoundSource.volume = Mathf.Clamp(constantSoundSource.volume - 0.7f * Time.deltaTime, 0, 1);
    //        }
    //    }
    //}
}
