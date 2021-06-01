using SNSSTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GunController : MonoBehaviour
{
	[SerializeField] private Animator gunAnimator;
	[SerializeField] private Animator swapAnimator;
	[SerializeField] private ShootingSoundController soundController;

	public GameObject standardShot;
	public GameObject energyShot;
	public GameObject missileShot;
	public GameObject chargedShot;

	public LaserBehaviour laserShot;

	[SerializeField] private Transform barrelL;
	[SerializeField] private Transform barrelR;

	[SerializeField] private ShieldProjector shieldProjector;

	public AmmoCounter ammoCount;

	private Transform _chargeShotTransform;

	[SerializeField] private ShipController _shipController;

	AudioSource audioSource;

	private WeaponType currentWeapon;

	public bool Attacking = false;

	public bool CanAttack = true;
	public bool CanSwap = true;

	private bool inReturnAnimation = false;
	private bool inSwapAnimation = false;

	public void EndReturnAnimation() { inReturnAnimation = false; }
	public void EndSwapAnimation() { inSwapAnimation = false; }

	void Start()
	{
		gunAnimator = gunAnimator ? gunAnimator : GetComponent<Animator>();

		currentWeapon = WeaponType.Regular;

		transform.Find("Gunners Camera").TryGetComponent(out soundController);

		TryGetComponent(out audioSource);
	}

	private void Update()
	{
        if(currentWeapon == WeaponType.Laser && CheckForPlayer())
        {
            EndLaser();
        }

        if (laserShot.fadeIn)
		{
			Ray ray = new Ray(barrelL.transform.position, barrelL.transform.forward);

			LayerMask enemyLayer = new LayerMask();
			enemyLayer += LayerMask.GetMask("Enemies");
			enemyLayer += LayerMask.GetMask("Swarm");

			RaycastHit[] hits = Physics.RaycastAll(ray, laserShot.Length, enemyLayer);

			foreach (RaycastHit hit in hits) {
				if (hit.collider.TryGetComponent(out HealthAndShields hp))
				{
					hp.TakeDamage(laserShot.Damage / 3, laserShot.Damage);
				}
			}

			ammoCount.Take1Ammo(currentWeapon);
			CheckAmmo();
		}	
	}

	private void OnGUI()
	{
		GUI.Box(new Rect(0, Screen.height - 25, 350, 35), "Ammo" +
			" ( " + "SS " + "n/a" + " )" +
			" ( " + "ES " + "n/a" + " )" +
			" ( " + "MS " + ammoCount.GetAmmo(WeaponType.Missiles) + " )" +
			" ( " + "CS " + ammoCount.GetAmmo(WeaponType.Charged) + " )" +
			" ( " + "LS " + ammoCount.GetAmmo(WeaponType.Laser) + " )");
		;
	}

	public void UpdateAttacking(bool state)
	{
		Attacking = state;
		gunAnimator.SetBool("Attacking", Attacking);
	}

    // Shoot from Ship
    public void ShootFromShip()
	{
		if (MenuManager.Instance.Sleeping) return;

		audioSource.Stop();

		GameObject shot = Instantiate(standardShot, _shipController.ShipModel.position + _shipController.ShipModel.forward * 10, _shipController.ShipModel.rotation);

        InitShot(shot);

        if (shot.TryGetComponent(out ShotInfo info))
        {
            info.role = PlayerRole.Pilot;
		}

		audioSource.Play();
	}

	public void ResetSpeed()
	{
		gunAnimator.speed = 1;
	}

	public bool CheckForPlayer()
	{
		RaycastHit hit;

		Vector3 origin = _shipController.GunnerCamera.transform.position;
		Vector3 direction = _shipController.GunnerCamera.transform.forward;

		if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity))
		{
			if (hit.collider.tag == "Player") {
				return true;
			}
		}
		
		return false;
	}

	public void CheckAmmo()
	{
		if (ammoCount.HasAmmo(currentWeapon) == false) {
			_shipController.SwapWeapon(new Vector2(0, -1));
			UpdateAttacking(false);
		}
	}

    private void OnEnable()
    {
        StartCoroutine(EnableMeBois());
    }

    IEnumerator EnableMeBois()
    {
        yield return null;
        yield return null;
        UpdateAttacking(false);
    }

    // Standard Shot
    public float FireRateStandard = 5f;
	public float FireRateEnergy = 5f;
	public void UpdateStandardShot()
	{
		gunAnimator.speed = currentWeapon == WeaponType.Regular ? FireRateStandard : FireRateEnergy;
	}

	// Missile Shot
	public float FireRateMissile = 1f;
	public void UpdateMissileShot()
	{
		gunAnimator.speed = FireRateMissile;
	}

	// Charge Shot
	public float FireRateCharge = 2f;
	public void UpdateChargeShot()
	{
		gunAnimator.speed = FireRateCharge;
	}

	// Charge Shot
	public float FireRateLaser = 0.2f;
	public void UpdateLaserShot()
	{
		gunAnimator.speed = FireRateLaser;
	}

	public void ShootStandard()
	{
		if (MenuManager.Instance.Sleeping) return;
		if (CheckForPlayer()) return;

		// TODO -- Play Sound

		GameObject shotPrefab = currentWeapon == WeaponType.Regular ? standardShot : energyShot;
		InitShot(Instantiate(shotPrefab, barrelL.position, barrelL.rotation));
		soundController.PlayShot(WeaponType.Regular);
	}

	public void ShootMissileL()
	{
		if (MenuManager.Instance.Sleeping) return;
		if (CheckForPlayer()) return;

		// TODO -- Play Sound

		ammoCount.Take1Ammo(currentWeapon);
		InitShot(Instantiate(missileShot, barrelL.position, barrelL.rotation));
		CheckAmmo();
		soundController.PlayShot(WeaponType.Missiles);
	}

	public void ShootMissileR()
	{
		if (MenuManager.Instance.Sleeping) return;
		if (CheckForPlayer()) return;

		// TODO -- Play Sound

		ammoCount.Take1Ammo(currentWeapon);
		InitShot(Instantiate(missileShot, barrelR.position, barrelR.rotation));
		CheckAmmo();
		soundController.PlayShot(WeaponType.Missiles);
	}

	public void StartCharge()
	{
		if (CheckForPlayer()) return;

		// TODO -- Play Sound

		GameObject newChargeShot = Instantiate(chargedShot, barrelL.position, barrelL.rotation, barrelL.transform);
		InitShot(newChargeShot);

		_chargeShotTransform = newChargeShot.transform;

		soundController.PlayShot(WeaponType.Charged);
	}

	public void EndCharge()
	{
		if (_chargeShotTransform != null)
		{
			ChargedShotBehaviour behaviour = _chargeShotTransform.GetComponent<ChargedShotBehaviour>();
			behaviour.HasShot = true;

			// TODO -- Stop Sound

			_chargeShotTransform = null;

			ammoCount.Take1Ammo(currentWeapon);
			CheckAmmo();
			soundController.Stop();
		}
	}

	public void StartLaser()
	{
		//// TODO -- Play Sound
		//LayerMask enemyLayer = new LayerMask();
		//enemyLayer += LayerMask.GetMask("Enemies");
		//enemyLayer += LayerMask.GetMask("Swarm");

		laserShot.fadeIn = true;
		soundController.PlayShot(WeaponType.Laser);

		//RaycastHit[] hits = Physics.SphereCastAll(barrelL.transform.position, laserShot.radius, barrelL.transform.forward.normalized, laserShot.Length, enemyLayer);

		//// The Initial cone in front of the laser
		//foreach (RaycastHit hit in hits) {
		//	if (hit.collider.TryGetComponent(out HealthAndShields hp))
		//	{
		//		hp.TakeDamage(laserShot.Damage / 3, laserShot.Damage);
		//	}
		//}
	}

	public void EndLaser()
	{
		// TODO -- Stop Sound
		laserShot.fadeIn = false;
		soundController.Stop();
	}

	public void SwapWeapon( WeaponType newWeapon)
	{
		// Do Nothing
		if (currentWeapon == newWeapon) return;

		if (CanSwap == true)
		{
			CanSwap = false;
			StartCoroutine(coSwapWeapon(newWeapon));
		}
	}

	private IEnumerator coSwapWeapon(WeaponType newWeapon)
	{        
		// Wait to stop attacking
		while (Attacking == true) { yield return null; }

		CanAttack = false;

		// Show the Swap UI
		swapAnimator.SetBool("ShowUI", true);

		// If we're NOT in the default state..
		// We need to change back to the default state first
		if (currentWeapon != WeaponType.Regular && 
			currentWeapon != WeaponType.Energy)
		{
			// Start the return animation
			inReturnAnimation = true;
			gunAnimator.SetTrigger("Reset");
			swapAnimator.SetTrigger("Return");

			// Wait for the return animation to end
			while (inReturnAnimation) { yield return null; }
		}
		else
		{
			if (currentWeapon == WeaponType.Energy) {
				gunAnimator.SetBool("Energy", true);
			}

			if (currentWeapon == WeaponType.Regular) {
				gunAnimator.SetBool("Energy", true);
			}
		}

		// Now we can start the swap animation
		inSwapAnimation = true;

		// Call the appropriate  animation trigger
		switch (newWeapon)
		{
			case WeaponType.Laser:
				gunAnimator.SetTrigger("Change_Laser");
				swapAnimator.SetTrigger("Idle_Laser");
				break;

			case WeaponType.Charged:
				gunAnimator.SetTrigger("Change_Charged");
				swapAnimator.SetTrigger("Idle_Charge");
				break;

			case WeaponType.Missiles:
				gunAnimator.SetTrigger("Change_Rockets");
				swapAnimator.SetTrigger("Idle_Missile");
				break;

			default: inSwapAnimation = false; break;
		}

		// Wait for the swap animation to end
		while (inSwapAnimation) { yield return null; }

		// Hide the Swap UI
		swapAnimator.SetBool("ShowUI", false);

		CanAttack = true;
		CanSwap = true;

		currentWeapon = newWeapon;
	}

	public void InitShot(GameObject newShot)
	{
		if (newShot.TryGetComponent(out ShotThing shotThing)) {
			shotThing.whoSent = ShotThing.shotFrom.Player;
        }

		if (newShot.TryGetComponent(out ShotInfo info))
		{
			info.role = PlayerRole.Gunner;
		}

        shieldProjector.IgnoreCollider(newShot.GetComponent<Collider>());
	}
}
