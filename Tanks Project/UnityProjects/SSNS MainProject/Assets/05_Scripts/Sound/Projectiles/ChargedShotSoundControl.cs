using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This controls when the sound plays for charged shot, which sound, and how loud
/// </summary>
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ChargedShotBehaviour))]
public class ChargedShotSoundControl : MonoBehaviour
{
    ChargedShotBehaviour chargedShot;
    AudioSource audioSource;
    [SerializeField] AudioClip shotSound;

    bool hasShot = false;

    // Start is called before the first frame update
    void Start()
    {
        chargedShot = GetComponent<ChargedShotBehaviour>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (chargedShot.HasShot && !hasShot)
        {
            hasShot = true;
            if (shotSound)
            {
                audioSource.clip = shotSound;
                audioSource.volume = chargedShot.GetDamagePercentage();
                audioSource.Play();
            }
        }
    }
}
