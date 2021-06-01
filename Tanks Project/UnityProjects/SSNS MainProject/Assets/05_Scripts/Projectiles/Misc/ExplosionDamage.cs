using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ExplosionDamage : MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] LayerMask enemyLayers;
    [SerializeField] float radius = 40f;
    [SerializeField] float kineticDamage = 25f;
    [SerializeField] float energyDamage = 5f;

    [SerializeField] GameObject tempExplosionSoundObject;

    private void OnCollisionEnter(Collision collision)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, enemyLayers);
        Debug.Log("Explosion destroyed " + colliders.Length + " enemies.");

        foreach(Collider c in colliders)
        {
            if(c.TryGetComponent(out HealthAndShields hp))
            {
                hp.TakeDamage(kineticDamage, energyDamage);
            }
        }

        if (explosionPrefab)
        {
            GameObject go = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            go.transform.localScale = new Vector3(radius, radius, radius);
        }

        if (tempExplosionSoundObject) Instantiate(tempExplosionSoundObject, transform.position, Quaternion.identity, collision.transform);

        Destroy(gameObject);
    }
}
