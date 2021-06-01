using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SNSSTypes;

public class WeaponsSystem : MonoBehaviour
{
	private ShipController _shipController;

	[HideInInspector] public WeaponType currentWeapon;
	public GunController GunController;

	public bool ShipShooting = false;
	public bool GunShooting = false;

	public float ShipShotInterval = 0.1f;
	public float ShipShotCounter = 0.0f;

	public Texture2D defaultTexture;

	private void Awake()
	{
		_shipController = GetComponent<ShipController>();
		GunController = GetComponentInChildren<GunController>();
	}

	private void Update()
	{
		if ((ShipShotCounter += Time.deltaTime) > ShipShotInterval && ShipShooting) {
			GunController.ShootFromShip();
			ShipShotCounter = 0.0f;
		}
	}

	public void ShootGun(bool pressed)
	{
		GunController.UpdateAttacking(pressed);
	}

	/// <summary> Swap the ships weapon  </summary>
	public void SwapWeapon(Vector2 Input)
	{
		if (Input.x > 0)
		{
			if (GunController.CanSwap && GunController.ammoCount.HasAmmo(WeaponType.Laser))
			{
				currentWeapon = WeaponType.Laser;
				GunController.SwapWeapon(currentWeapon);
			}
		}
		else if (Input.x < 0)
		{
			if (GunController.CanSwap && GunController.ammoCount.HasAmmo(WeaponType.Missiles))
			{
				currentWeapon = WeaponType.Missiles;
				GunController.SwapWeapon(currentWeapon);
			}
		}
		else if (Input.y > 0)
		{
			if (GunController.CanSwap && GunController.ammoCount.HasAmmo(WeaponType.Charged))
			{
				currentWeapon = WeaponType.Charged;
				GunController.SwapWeapon(currentWeapon);
			}
		}
		else if (Input.y < 0)
		{
			if (GunController.CanSwap)
			{
				// If were using the regular weapon swap to the energy varient, else use the regular one
				currentWeapon = currentWeapon == WeaponType.Regular ? WeaponType.Energy : WeaponType.Regular;
				GunController.SwapWeapon(currentWeapon);
			}
		}
	}

    [HideInInspector] public bool useDefaultEditor = false;
	[HideInInspector] public bool showWeaponsSystem = false;
}