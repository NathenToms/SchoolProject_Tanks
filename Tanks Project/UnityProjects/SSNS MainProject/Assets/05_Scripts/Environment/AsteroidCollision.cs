using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidCollision : MonoBehaviour
{
    [SerializeField] private GameObject broken;

    [SerializeField] private HealthAndShields health;

    private void Awake()
    {
        broken.gameObject.SetActive(false);

        if (TryGetComponent(out Rigidbody rigid))
        {
            rigid.mass = 1000;
            rigid.angularVelocity = Random.insideUnitSphere * 50000;
        }

        health.onDeath += Split;
    }

    private void Split()
    {
        broken.transform.position = health.transform.position;
        broken.transform.rotation = health.transform.rotation;

        broken.SetActive(true);

        foreach (Transform children in broken.transform)
        {
            Rigidbody rigid = children.gameObject.AddComponent<Rigidbody>();
            rigid.mass = 500;
            rigid.useGravity = false;
            rigid.drag = 0.05f;

            Vector3 dir = (children.position - transform.position).normalized;

            rigid.AddForce(dir * 100000, ForceMode.Force);
        }

        if (TryGetComponent(out health))
        {
            health.onDeath -= Split;
        }
    }


}
