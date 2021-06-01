using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// When two gameObjects collide they look for this script
[RequireComponent(typeof(Collider))]
public class Damage : MonoBehaviour
{
    [SerializeField] bool destroyOnHit = false;
    [SerializeField] bool constantCollisionDamage = false;
    // The damage whatever collides with the gameObject holding this script should take
    public int kineticDamage = 5;
	public int energyDamage = 5;

    [Space(10)]
    [SerializeField] GameObject hitSoundObject;

    public int KineticDamage { get { return kineticDamage; } }
    public int EnergyDamage { get { return energyDamage; } }

    public void ChangeDamage(int kinetic, int energy)
    {
        kineticDamage = kinetic;
        energyDamage = energy;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(tag)) return;

        //Spawn the hit sound Object and parent to what it hit. Do this in case the object is destroyed on hitting things
        if (hitSoundObject)
        {
            AreaManager.Instance.OnObjectAdd(Instantiate(hitSoundObject, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal, collision.gameObject.transform.up)));
        }

        //Apply damage to things it hits
        if (collision.gameObject.TryGetComponent(out HealthAndShields hpTemp))
        {
            hpTemp.TakeDamage(kineticDamage, energyDamage);
        }
        else if (collision.gameObject.TryGetComponent(out ShieldProjector shield))
        {
            if (shield.ShipColliders != null && shield.ShipColliders[0].TryGetComponent(out HealthAndShields health))
            {
                health.TakeDamage(kineticDamage, energyDamage);
            }
        }

        if (destroyOnHit)
        {
            if (TryGetComponent(out HealthAndShields health))
            {
                health.TakeDamage(Mathf.Infinity, Mathf.Infinity);
            }
            else
                Destroy(gameObject);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        //Apply damage to things it hits
        if (collision.gameObject.TryGetComponent(out HealthAndShields hpTemp) && constantCollisionDamage)
        {
            hpTemp.TakeDamage(kineticDamage, energyDamage);
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{

    //    //Spawn the hit sound Object and parent to what it hit. Do this in case the object is destroyed on hitting things
    //    if (hitSoundObject)
    //        Instantiate(hitSoundObject, other.transform);

    //    //Apply damage to things it hits
    //    if (other.gameObject.TryGetComponent(out HealthAndShields hpTemp))
    //    {
    //        hpTemp.TakeDamage(kineticDamage * Time.deltaTime, energyDamage * Time.deltaTime);
    //    }

    //    if(destroyOnHit)
    //    {
    //        Destroy(gameObject);
    //    }
    //}
}
