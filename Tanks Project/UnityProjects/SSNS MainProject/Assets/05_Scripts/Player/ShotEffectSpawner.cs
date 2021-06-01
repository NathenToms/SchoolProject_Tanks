using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotEffectSpawner : MonoBehaviour
{
    [SerializeField] GameObject missileEffect;
    [SerializeField] GameObject chargedEffect;
    [SerializeField] GameObject muzzleFlashEffect;

    [SerializeField] Transform spawnLocation;
    [Range(0, 3)]
    [SerializeField] float lifeTime = 1f;

    public void SpawnEffect(SNSSTypes.WeaponType weapon)
    {
        GameObject go = null;
        switch (weapon)
        {
            case SNSSTypes.WeaponType.Regular:
                go = Instantiate(muzzleFlashEffect, spawnLocation.position, spawnLocation.rotation, spawnLocation);
                break;
            case SNSSTypes.WeaponType.Energy:
                go = Instantiate(muzzleFlashEffect, spawnLocation.position, spawnLocation.rotation, spawnLocation);
                break;
            case SNSSTypes.WeaponType.Charged:
                go = Instantiate(chargedEffect, spawnLocation.position, spawnLocation.rotation, spawnLocation);
                break;
            case SNSSTypes.WeaponType.Missiles:
                go = Instantiate(missileEffect, spawnLocation.position, spawnLocation.rotation, spawnLocation);
                break;
        }

        if(go) Destroy(go, lifeTime);
    }
}
