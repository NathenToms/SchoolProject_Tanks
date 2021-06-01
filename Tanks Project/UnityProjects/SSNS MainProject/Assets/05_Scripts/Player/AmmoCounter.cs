using SNSSTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AmmoCounter
{
    [SerializeField] private int[] ammo = new int[(int)WeaponType.End];
    float laserCounter = 0.0f;  //time in seconds
    const float maxLaserTime = 1.0f;  //1 seconds
    public int GetAmmo(WeaponType weapon)
    {
        return ammo[(int)weapon];
    }

    public bool HasAmmo(WeaponType weapon)
    {
        if (weapon == WeaponType.Regular || weapon == WeaponType.Energy)
        {
            return true;
        }
        return ammo[(int)weapon] > 0;
    }

    public void SetAmmo(WeaponType weapon, int toAdd)
    {
        ammo[(int)weapon] += toAdd;
    }

    public void Take1Ammo(WeaponType weapon)
    {
        if (weapon == WeaponType.Laser && laserCounter >= maxLaserTime)
        {
            laserCounter = 0;

            ammo[(int)weapon] -= 1;
            if (ammo[(int)weapon] < 0) ammo[(int)weapon] = 0;
        }
        else if (weapon != WeaponType.Laser)
        {
            ammo[(int)weapon] -= 1;
            if (ammo[(int)weapon] < 0) ammo[(int)weapon] = 0;
        }
        else
        {
            laserCounter += Time.deltaTime;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        ammo = new int[(int)WeaponType.End];
    }

}
