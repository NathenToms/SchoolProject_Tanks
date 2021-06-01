using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShootingSoundController : MonoBehaviour
{
    [SerializeField] AudioSource shotSource;
    [SerializeField] AudioClip energyShotSound;
    [SerializeField] AudioClip regularShotSound;
    [SerializeField] AudioClip missileShotSound;

    public void PlayShot(SNSSTypes.WeaponType weaponType)
    {
        shotSource.clip = null;

        if(weaponType == SNSSTypes.WeaponType.Regular && regularShotSound)
        {
            shotSource.clip = regularShotSound;
        }
        else if(weaponType == SNSSTypes.WeaponType.Energy && energyShotSound)
        {            
            shotSource.clip = energyShotSound;
        }
        else if(weaponType == SNSSTypes.WeaponType.Missiles && missileShotSound)
        {
            shotSource.clip = missileShotSound;
        }

        if(shotSource.clip != null) shotSource.Play();
    }

    public void Stop()
    {
        shotSource.Stop();
    }
}
