using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Pickup : MonoBehaviour
{
    [SerializeField] SNSSTypes.WeaponType weapon;
    [SerializeField] int ammoCount = 10;

    [SerializeField] GameObject hitVFX;
    [SerializeField] float vfxScale = 1;

    private void OnTriggerEnter(Collider other)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player && other.gameObject.Equals(player)) //If player is found to exist and player is the one colliding
        {
			WeaponsSystem weaponsSystem = player.GetComponent<WeaponsSystem>();
			weaponsSystem.GunController.ammoCount.SetAmmo(weapon, ammoCount);

			if (hitVFX)
            {
                GameObject go = Instantiate(hitVFX, transform.position, Quaternion.identity);
                go.transform.localScale = new Vector3(vfxScale, vfxScale, vfxScale);
            }

            Destroy(gameObject);
        }
    }
}
