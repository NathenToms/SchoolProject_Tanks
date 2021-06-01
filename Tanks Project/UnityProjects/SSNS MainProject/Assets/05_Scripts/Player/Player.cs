using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using SNSSTypes;

public class Player : Controller
{
	// This players role (Pilot or Gunner)
	public PlayerRole myRole = PlayerRole.None;

	// The current gamestate
	// This is used to change the playing action map to a menu action map
	// public GameState currentState = GameState.BATTLE;

	public PlayerInput PlayerInput;
	public ShipInputHandler Controller;

	#region Unity Events

	private void Awake()
	{
		PlayerInput = GetComponent<PlayerInput>();
		FindShip();

		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		PlayerConnectionManager.Instance.Join(this);
	}

	private void Update()
	{
		//// Keep our GameState up to date with the GameManager
		//if (GameManager.Instance && currentState != GameManager.Instance.GameState)
		//{
		//	currentState = GameManager.Instance.GameState;
		//	SetPlayerActionMap(currentState);
		//}
	}
	#endregion

	public void FindShip()
	{
		// When a player starts the game this grabs an open ship
		foreach (GameObject ship in GameObject.FindGameObjectsWithTag("Player"))
		{
			if (ship.TryGetComponent(out ShipInputHandler shipController))
			{
				Controller = shipController;
				Controller.JoinShip(this);
				break;
			}
		}
	}

	public void SetPlayerActionMap(string targetMap)
	{
		PlayerInput.SwitchCurrentActionMap(targetMap);
	}

	public void SwapRole()
	{
		if (myRole == PlayerRole.None)
		{
			Debug.Log("You cannot swap rolls if you do not have one!");
		}
		else
		{
			if (myRole == PlayerRole.Pilot)
			{
				myRole = PlayerRole.Gunner;
				return;
			}

			if (myRole == PlayerRole.Gunner)
			{
				myRole = PlayerRole.Pilot;
				return;
			}
		}
	}

	#region [Action Map] Ship Controller

	// Used to move the player
	public override void OnLeftStick(InputValue input)
	{
		if (myRole == PlayerRole.Pilot)
		{
			// Rotate the player ship
			Controller.SteerShip(input.Get<Vector2>());
		}

		if (myRole == PlayerRole.Gunner)
		{
			// Move the player camera
			Controller.AimGun(input.Get<Vector2>());
		}
	}
	public override void OnRightStick(InputValue input)
	{
		if (myRole == PlayerRole.Pilot)
		{
			// Rotate the player ship
			Controller.StrafeShip(input.Get<Vector2>());
		}

		if (myRole == PlayerRole.Gunner)
		{
			// Move the player camera
			Controller.AimGun(input.Get<Vector2>());
		}
	}

	// Called when this player uses the DPad
	public override void OnDPad(InputValue input)
	{
		// NOTE -- The DPad does nothing for the pilot

		if (myRole == PlayerRole.Gunner)
		{
			// Change the current weapon
			Controller.SwapWeapon(input.Get<Vector2>());
		}
	}

	public override void OnA(InputValue input)
	{
		if (myRole == PlayerRole.Pilot)
		{
			// Boost the ship
			Controller.Boost(input.isPressed);
		}

		if (myRole == PlayerRole.Gunner)
		{
			// Make the gunner shoot
			Controller.ShootGun(input.isPressed);
		}
	}
	public override void OnB(InputValue input)
	{
		// TODO -- We gota go over what the B button does..

		if (myRole == PlayerRole.Pilot)
		{
			// Boost the ship
			Controller.Boost(input.isPressed);
		}

		if (myRole == PlayerRole.Gunner)
		{
			// Make the gunner shoot
			Controller.ShootGun(input.isPressed);
		}
	}
	public override void OnX(InputValue input)
	{
		if (!Controller)
			Controller.ToggleMap(input.isPressed);
	}
	public override void OnY(InputValue input)
	{
		if (myRole != PlayerRole.None) Controller.TriggerRoleSwap(input.isPressed);
	}

	public override void OnLeftTrigger(InputValue input)
	{
		if (myRole == PlayerRole.Pilot)
		{
			// Boost the ship
			Controller.RotateShip(input.Get<float>());
		}

		if (myRole == PlayerRole.Gunner)
		{
			// Make the gunner shoot
			Controller.SlowCamera(input.isPressed);
		}
	}
	public override void OnRightTrigger(InputValue input)
	{
		if (myRole == PlayerRole.Pilot)
		{
			// Boost the ship
			Controller.RotateShip(-input.Get<float>());
		}

		if (myRole == PlayerRole.Gunner)
		{
			// Make the gunner shoot
			Controller.SlowCamera(input.isPressed);
		}
	}

	public override void OnRightBumper(InputValue input)
	{
		if (myRole == PlayerRole.Pilot)
		{
			Controller.ShootShip(input.isPressed);
		}

		if (myRole == PlayerRole.Gunner)
		{
			Controller.ShootGun(input.isPressed);
		}
	}
	public override void OnLeftBumper(InputValue input)
	{
		if (myRole == PlayerRole.Pilot)
		{
			Controller.ShootShip(input.isPressed);
		}

		if (myRole == PlayerRole.Gunner)
		{
			Controller.LockOn(input.isPressed);
		}
	}
	#endregion

	#region [Action Map] Menu Navigation

	public void OnPause(InputValue input)
	{
		// Tell the menu manager to pause the game
		if (MenuManager.Instance)
		{
			MenuManager.Instance.PauseGame(!MenuManager.Instance.Sleeping);
		}
	}

	public void OnUnPause(InputValue input)
	{
		// Tell the menu manager to pause the game
		MenuManager.Instance.PauseGame(!MenuManager.Instance.Sleeping);
	}

	public void OnNavigate(InputValue input)
	{
		MenuManager menu = MenuManager.Instance;

		// Unity automatically does this
		if (menu.openMenu == OpenMenuType.NODE_MAP)
		{
			//menu.NavigateNodeMap(input.Get<Vector2>());
		}
		else
		{
			menu.NavigateMenu(input.Get<Vector2>());
		}
	}

	public void OnSelect(InputValue input)
	{
		MenuManager menu = MenuManager.Instance;
		menu.Select();
	}

	public void OnBack(InputValue input)
	{
		MenuManager menu = MenuManager.Instance;
		menu.Return();
	}
	#endregion
}
