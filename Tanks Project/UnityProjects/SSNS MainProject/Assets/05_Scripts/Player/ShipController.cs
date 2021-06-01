using SNSSTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{
	[SerializeField] private Camera pilotCamera;
	[SerializeField] private Camera gunnerCamera;

	[SerializeField] private ShootingSoundController shootingSoundController;

	[SerializeField] private ShieldProjector shieldProjector;

	[SerializeField] private WeaponsSystem weaponsSystem;

	[SerializeField] private PlayerHUDHandler playerHUD;

	[SerializeField] private HealthAndShields shipHP;

	[SerializeField] private Rigidbody rigidbody;

	[SerializeField] private ParticleSystem warpEffect1;
	[SerializeField] private ParticleSystem warpEffect2;
	[SerializeField] private ParticleSystem sonicBoostEffect1;
	[SerializeField] private ParticleSystem sonicBoostEffect2;
	[SerializeField] private GameObject sonicBoomEffect;

	[SerializeField] private Transform shipModel;

	[SerializeField] private ShipBehaviour myBehaviour;
	[SerializeField] private PlayerStats myProperties;

	[SerializeField] private Transform gunnerslockUI;
	[SerializeField] private Transform gunnerPivot;
	[SerializeField] private Transform gunnerObject;

	[SerializeField] private GameObject lockOnTarget;

	[SerializeField] private AudioSource audioSource;
	[SerializeField] private AudioClip boostSound;
	[SerializeField] private AudioClip sonicSound;

	private bool boostPlayed;
	private bool sonicPlayed;

	public Player Player1;
	public Player Player2;

	public bool InvertedControls;
	public bool UnlimitedBoost;
	public bool AimAssist;

	#region Ship Members
	private Vector2 strafeDirection;
	private Vector2 rotateDirection;
	private Vector2 gunnerCenter;
	private Vector2 gunVelocity;
	private Vector2 gunRotation;

	private Vector3 finalStrafeVelocity;
	private Vector3 finalThrustVelocity;
	private Vector3 finalRollRotation;
	private Vector3 finalRotation;
	private Vector3 collisionVelocity;
	private Vector3 collisionDirection;

	private int numberOfPlayers;

	private float thrustSpeed;
	private float strafeSpeed;
	private float rotationSpeed;
	private float rollDirection;
	private float rollInput;
	private float rollSpeed;
	private float spinSpeed;
	private float boostGauge;
	private float turnMultiplayer;
	private float sonicBoomTime;
	private float boostDepleteDelay;

	private bool strafing;
	private bool rotating;
	private bool boosting;
	private bool sonicBoom;
	private bool warping;
	private bool roleSwap;
	private bool stopThrust;
	private bool stopRotation;
	private bool freezeShip;
	private bool slowCamera;
	private bool shooting_Gunner;
	private bool shooting_Pilot;
	private bool collision;
	#endregion

	#region Properties
	public Camera PilotCamera { get { return pilotCamera; } }
	public Camera GunnerCamera { get { return gunnerCamera; } }

	public ShipBehaviour Behaviour { get { return myBehaviour; } }

	public PlayerStats Properties { get { return myProperties; } }

	public ParticleSystem WarpEffect1 { get { return warpEffect1; } }
	public ParticleSystem WarpEffect2 { get { return warpEffect2; } }

	public Transform ShipModel { get { return shipModel; } }

	public int NumberOfPlayers { get; set; }

	public bool Boosting { get { return boosting; } set { boosting = boostDepleteDelay >= myProperties.boostDepleteDelay ? value : false; } }
	public bool Warping { get { return warping; } set { warping = value; } }
	public bool Strafing { get { return strafing; } set { strafing = value; } }
	public bool Rotating { get { return rotating; } set { rotating = value; } }
	public bool RoleSwap { get { return roleSwap; } set { roleSwap = value; } }
	public bool StopThrust { get { return stopThrust; } set { stopThrust = value; } }
	public bool Freeze { get { return freezeShip; } set { freezeShip = value; } }

	public float RollInput { get { return rollInput; } set { rollInput = value; } }
	public float BoostGauge { get { return boostGauge; }  set { boostGauge = value; } }

	public float TurnMultiplayer { get { return turnMultiplayer; }  set { turnMultiplayer = value; } }

	public Vector2 StrafeDirection { get { return strafeDirection; } set { strafeDirection = value; } }
	public Vector2 RotateDirection { get { return rotateDirection; } set { rotateDirection = value; } }

	public Vector3 GunVelocity { get { return gunVelocity; }  set { gunVelocity = value; } }
	#endregion

	// Unity Methods and Events
	#region Unity Events and Methods

	/// <summary>
	/// Initialize and Get dependencies
	/// </summary>
    /// 
	private void Start()
	{
		if (shieldProjector == null) {
			shieldProjector = GetComponentInChildren<ShieldProjector>();
		}

		if (weaponsSystem == null) {
			weaponsSystem = gameObject.GetComponent<WeaponsSystem>();
		}

		if (shieldProjector == null) {
			shieldProjector = GetComponentInChildren<ShieldProjector>();
		}

		if (playerHUD == null) {
			playerHUD = gameObject.GetComponent<PlayerHUDHandler>();
		}

		if (rigidbody == null) {
			rigidbody = gameObject.GetComponent<Rigidbody>();
		}

		foreach (Player player in GameObject.FindObjectsOfType<Player>()) {
			player.FindShip();
		}

		// Set the cameras size and positions
		gunnerCamera.rect = new Rect(0, 0.0f, 1.0f, 0.5f);
		pilotCamera.rect = new Rect(0, 0.5f, 1.0f, 0.5f);

		gunnerCenter = gunnerslockUI.transform.position;

		boostGauge = myProperties.maxBoostGauge;

		if (!shipHP) TryGetComponent(out shipHP);

		if (!audioSource) TryGetComponent(out audioSource);

		boostDepleteDelay = myProperties.boostDepleteDelay;
    }

	/// <summary>
	/// Used for all the non physics calculations
	/// </summary>
	private void Update()
	{
		if (MenuManager.Instance.Sleeping) return;

		UpdateShipValues();
		
		UpdateShipModel(rotateDirection);

		UpdateTurnMultiplier();

		if (warping)
		{
			WarpEffect1.Play();
			WarpEffect2.Play();
		}
		else if (!boosting)
		{
			WarpEffect1.Stop();
			WarpEffect2.Stop();
		}

		if (warping || sonicBoom)
		{
			sonicBoostEffect1.Play();
			sonicBoostEffect2.Play();

			if (!sonicPlayed && audioSource && !audioSource.isPlaying)
			{
				audioSource.clip = sonicSound;
				audioSource.Play();

				sonicPlayed = true;
			}
		}
		else
		{
			sonicBoostEffect1.Stop();
			sonicBoostEffect2.Stop();

			sonicPlayed = false;
		}
	}

	/// <summary>
	/// FixedUpdate is used for physic calculations
	/// </summary>
	private void FixedUpdate()
	{
		if (MenuManager.Instance.Sleeping) return;

		UpdateShipPhysics();
	}

    /// <summary>
    /// LateUpdate is used to set the gunners camera, the reason for this is
    /// because we need to know where the ship is after physics are applied
    /// </summary>
    private void LateUpdate()
	{
		if (MenuManager.Instance.Sleeping) return;

		UpdateCamera();
	}

	private void OnCollisionEnter(Collision c)
	{
		if (c.gameObject.tag != "Projectile" && c.gameObject.layer != 11 && !collision)
		{
			Vector3 dir = c.contacts[0].point - transform.position;

			dir = -dir.normalized;

			collisionVelocity = dir * 50000;

			collisionDirection = dir;

			collision = true;
		}
	}

	#endregion

	// Update transformations for the gun or the ship
	#region Ship Movement Methods

	/// <summary> Change the roles of the players </summary>
	public void SwapRoles()
	{
		if (Player1) Player1.SwapRole();
		if (Player2) Player2.SwapRole();

		ResetShip();
	}

	/// <summary>
	/// Use INPUT and update the camera
	/// </summary>
	public void UpdateCamera()
	{
        if (!warping)
        {
            // Where the camera SHOULD be relative to the ship model
            Vector3 shipPos = shipModel.transform.position;

            Vector3 yPos = -6 * shipModel.transform.up;
            Vector3 zPos = -1 * shipModel.transform.forward;

            gunnerPivot.position = Vector3.Lerp(gunnerPivot.position, shipPos + yPos + zPos, 0.25f);

            gunnerPivot.rotation = Quaternion.identity;
            gunnerObject.Rotate(new Vector2(-gunVelocity.y, gunVelocity.x) * Time.deltaTime * (slowCamera ? 0.2f : 1f));

            //  gunnerObject.eulerAngles = new Vector3(gunnerObject.eulerAngles.x, gunnerObject.eulerAngles.y, 0);

            // If we don't have a target currently locking
            if (lockOnTarget == null)
            {
                CheckForTarget();
            }

            // (BLOCKER) If we cant Lock on
            if (AimAssist == false) return;

            if (lockOnTarget != null)
            {
                Vector2 newPoint = gunnerCamera.WorldToScreenPoint(lockOnTarget.transform.position);
                float unlockDist = 60;

                if (Vector2.Distance(gunnerCenter, newPoint) < unlockDist)
                {
                    gunnerslockUI.transform.position = newPoint;
                }
                else
                {
                    gunnerslockUI.transform.position = gunnerCenter;
                    lockOnTarget = null;
                }
            }
        }
	}

	/// <summary>
	/// Raycast forward and see if the gunners hovering an enemy
	/// </summary>
	public void CheckForTarget()
	{
		// (BLOCKER) If we cant Lock on
		if (AimAssist == false) return;

		RaycastHit hit;

		Vector3 origin = gunnerCamera.transform.position;
		Vector3 direction = gunnerCamera.transform.forward;

		if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity, LayerMask.GetMask("Enemies")))
		{
			Debug.Log("Locked ON!", hit.collider.gameObject);
			lockOnTarget = hit.collider.gameObject;
		}
	}

	/// <summary>
	/// Use INPUT and update the ships target values, not the actual rigidbody
	/// </summary>
	public void UpdateShipValues()
	{
		// Steer Ship
		#region Steer Ship


		if (rotating)
		{
			rotationSpeed = Mathf.Clamp(rotationSpeed + myProperties.shipRotAcceleration , 0, myProperties.rotationSpeed);
		}
		else
		{
			rotationSpeed = Mathf.Clamp(rotationSpeed - myProperties.shipRotAcceleration , 0, myProperties.rotationSpeed);
		}

		finalRotation = rotateDirection * rotationSpeed;
		#endregion

		// Rotate Ship
		#region  Rotate Ship

		if (rollInput > 0)
		{
			rollDirection = 1;
			rollSpeed = Mathf.Clamp(rollSpeed + myProperties.shipRotAcceleration, 0, myProperties.rotationSpeed);
		}
		else if (rollInput < 0)
		{
			rollDirection = -1;
			rollSpeed = Mathf.Clamp(rollSpeed + myProperties.shipRotAcceleration, 0, myProperties.rotationSpeed);
		}
		else
		{
			rollSpeed = Mathf.Clamp(rollSpeed - myProperties.shipRotDeceleration, 0, myProperties.rotationSpeed);
		}

		finalRollRotation = new Vector3(0, 0, rollDirection) * rollSpeed;
		#endregion

		// Strafe Ship
		#region Strafe Ship

		if (strafing)
		{
			strafeSpeed = Mathf.Clamp(strafeSpeed + myProperties.shipAcceleration * 2f, 0, myProperties.strafeSpeed);
		}
		else
		{
			strafeSpeed = Mathf.Clamp(strafeSpeed - myProperties.shipDeceleration, 0, myProperties.strafeSpeed);
		}
		finalStrafeVelocity = strafeDirection * strafeSpeed;
		#endregion

		// Ship Thrust
		#region Ship Thrust

		if (stopThrust || freezeShip)
		{
			thrustSpeed = Mathf.Clamp(thrustSpeed - myProperties.shipDeceleration, 0, myProperties.shipSpeed);
		}
		// If were boosting
		else if (!warping && boosting && boostDepleteDelay >= myProperties.boostDepleteDelay)
		{

			if (!boostPlayed && audioSource && !audioSource.isPlaying)
			{
				audioSource.clip = boostSound;
				audioSource.Play();
				boostPlayed = true;
			}

			float maxSpeed = myProperties.boostSpeed / 2.0f;
			float acceleration = myProperties.shipAcceleration * 1.5f;

			if (sonicBoom)
			{
				maxSpeed = myProperties.boostSpeed;
				acceleration = myProperties.shipAcceleration * 2.5f;
			}
			else if (!sonicBoom && sonicBoomTime > myProperties.sonicBoomTime)
			{
				sonicBoom = true;
				sonicBoomTime = 0;

				Instantiate(sonicBoomEffect, transform);
			}
			else
			{
				sonicBoomTime += Time.deltaTime;
			}

			// Increase our speed when boosting
			if (rigidbody.velocity.magnitude < myProperties.boostSpeed)
				thrustSpeed = Mathf.Clamp(thrustSpeed + (acceleration * Time.deltaTime), myProperties.shipSpeed, maxSpeed);

			// Reduce the boost gauge
			boostGauge = Mathf.Clamp(boostGauge - myProperties.boostGaugeConsumeAmount * Time.deltaTime, 0, myProperties.maxBoostGauge);
			playerHUD._slider_Boost.value = 1 / myProperties.maxBoostGauge * boostGauge;

			// Set the color of the boost slider
			playerHUD._boostImage.color = Color.Lerp(Color.red, Color.yellow, 1 / myProperties.maxBoostGauge * boostGauge);

			// Turn off boosting
			if (boostGauge <= 0 && UnlimitedBoost == false)
			{
				boosting = false;
				boostDepleteDelay = 0;
			}
		}
		// Else if we are NOT boosting and our thrustSpeed is over our maxThrustSpeed
		// We need to smooth the transition from boosting to not boosting
		else if (thrustSpeed > myProperties.shipSpeed)
		{
			// Clamp our thrust Speed to our max thrust speed
			thrustSpeed = Mathf.Clamp(thrustSpeed - myProperties.shipDeceleration, myProperties.shipSpeed, myProperties.boostSpeed);

			sonicBoom = false;
			sonicBoomTime = 0;
			boostPlayed = false;
		}
		// Else were not boosting so we increase our ships normal speed
		else
		{
			thrustSpeed = myProperties.shipSpeed;
			boostPlayed = false;
		}

		if (boostDepleteDelay < myProperties.boostDepleteDelay)
		{
			boostDepleteDelay = Mathf.Clamp(boostDepleteDelay + Time.deltaTime, 0, myProperties.boostDepleteDelay);
		}

		finalThrustVelocity = transform.forward * thrustSpeed;
		#endregion
	}

	/// <summary>
	/// Update the ships movement
	/// </summary>
	public void UpdateShipPhysics()
	{
		if (!freezeShip)
		{
			// Apply physics to rigidbody
			Quaternion rollRotation = Quaternion.Euler(finalRollRotation);
			Quaternion rotateRotation = Quaternion.Euler(finalRotation);

			rigidbody.rotation *= rollRotation * rotateRotation;

			if (collision)
			{
				rigidbody.AddForce(collisionVelocity, ForceMode.Acceleration);

				collisionVelocity = Vector3.zero;

				Quaternion direction = Quaternion.LookRotation(collisionDirection);

				rigidbody.rotation = Quaternion.Lerp(rigidbody.rotation, direction, 6 * Time.deltaTime);

				if (Mathf.Abs(Vector3.Angle(transform.forward, collisionDirection)) < 2.5f || rotating)
					collision = false;
			}
			else
			{
				rigidbody.AddForce(finalThrustVelocity, ForceMode.VelocityChange);
				rigidbody.AddRelativeForce(finalStrafeVelocity, ForceMode.VelocityChange);
			}
		}
		else if (warping)
		{
			rigidbody.AddForce(finalThrustVelocity, ForceMode.VelocityChange);
		}
	}

	/// <summary>
	/// Move the ships model so it looks more like your controlling the ship
	/// </summary>
	public void UpdateShipModel(Vector3 velocity)
	{
		// Rotation
		Quaternion currentRot = shipModel.transform.localRotation;
		Quaternion targetRot = Quaternion.Euler
			(
				 velocity.x * myBehaviour.xRot,
				 velocity.z * myBehaviour.yRot,
				-velocity.y * myBehaviour.zRot
			);

		shipModel.transform.localRotation = Quaternion.Slerp(currentRot, targetRot, myBehaviour.RotSpeed);

		// Translation
		Vector3 currentPos = shipModel.transform.localPosition;
		Vector3 targetPos = new Vector3(-velocity.y, velocity.x, 0) * myBehaviour.moveScale;

		if (targetPos.y < 0) {
			targetPos.y *= 0.4f;
		}

		shipModel.transform.localPosition = Vector3.Lerp(currentPos, targetPos, myBehaviour.moveSpeed);

		// Camera
		Vector3 targetCameraPos = warping ? myBehaviour.warpPos : sonicBoom ? myBehaviour.sonicPos : boosting ? myBehaviour.boostPos : myBehaviour.normalPos;

		pilotCamera.transform.localPosition = Vector3.Slerp(pilotCamera.transform.localPosition, targetCameraPos, myBehaviour.cameraSpeed * Time.deltaTime);
	}

	#endregion

	private void UpdateTurnMultiplier()
	{
		if (boosting)
		{
			turnMultiplayer = Mathf.Lerp(turnMultiplayer, 0.4f, 0.03f);
		}
		else
		{
			turnMultiplayer = Mathf.Lerp(turnMultiplayer, 1f, 0.03f);
		}
	}

	private void ResetShip()
	{
		gunVelocity = Vector3.zero;
	}

	public void SwapWeapon(Vector2 Input)
	{
		weaponsSystem.SwapWeapon(Input);
	}

	public void ShootGun(bool pressed)
	{
		weaponsSystem.ShootGun(pressed);
	}

	public void ShootShip(bool pressed)
	{
		weaponsSystem.ShipShooting = pressed;
	}
	
	public void SlowCamera(bool pressed)
	{
		slowCamera = pressed;
	}

    private void OnEnable()
    {
        if (weaponsSystem == null)
        {
            weaponsSystem = gameObject.GetComponent<WeaponsSystem>();
        }

        weaponsSystem.ShipShooting = false;
    }

    /// <summary>
    /// Makes gunner face the same direction as the pilot
    /// </summary>
    public void AlignGunnerWithPilot()
    {
        gunnerObject.transform.rotation = gameObject.transform.rotation;
    }

	// Editor values
	[HideInInspector] public bool showBehaviour = false;
	[HideInInspector] public bool showStats = false;
	[HideInInspector] public bool useDefaultEditor = false;
}
