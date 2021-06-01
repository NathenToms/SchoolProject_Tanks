using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE -- Any gameObjects with a Bullet script should also have a Damage script
//		   The Damage script holds the damage members for any collidables including bullets

// The Bullet Class keeps track of how long a projectile should stay alive and how / how fast it should move
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ShotInfo))]
public class Bullet : MonoBehaviour
{
    public GameObject shooter;

    private void Awake()
    {
        //Propel bullet on spawn
        Vector3 rotation = transform.forward;
        rotation.Normalize();

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = rotation * GetComponent<ShotInfo>().Speed; // propel forward with set speed
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(DestroyBullet());
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForEndOfFrame();

        Destroy(gameObject);
    }
}
