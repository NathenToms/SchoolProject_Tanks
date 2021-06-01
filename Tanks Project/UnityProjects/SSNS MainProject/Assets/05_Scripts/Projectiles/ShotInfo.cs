using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotInfo : MonoBehaviour
{
    public SNSSTypes.WeaponType weapon;
    public SNSSTypes.PlayerRole role;
    [SerializeField] private float fireRate;
    [Tooltip("Life in seconds, 0 for infinite life")]
    [Range(0, 120)]
    [SerializeField] private float lifeTime;
    [SerializeField] private float speed;
    private float lifeTimer = 0;

    public float FireRate { get { return fireRate; } }
    public float Speed { get { return speed; } }

	[SerializeField] private bool destroyOnDeath = true;

    private void Update()
    {
		if (MenuManager.Instance.Sleeping) return;
        if (weapon != SNSSTypes.WeaponType.Laser || lifeTime != 0)
        {
            lifeTimer += Time.deltaTime;

            if (lifeTimer >= lifeTime && destroyOnDeath) Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.GetMask("Enemies") && role != SNSSTypes.PlayerRole.None && GameManager.Instance.Player.TryGetComponent(out PlayerHUDHandler hud))
        {
            hud.BlinkRedicle(role);
        }
    }
}
