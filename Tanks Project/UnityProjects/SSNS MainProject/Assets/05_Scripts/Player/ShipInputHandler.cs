using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

public class ShipInputHandler : MonoBehaviour
{
	private ShipController _shipController;

	private void Awake()
	{
		_shipController = GetComponent<ShipController>();
	}

	/// <summary> Add a player to the ship </summary>
	public void JoinShip(Player newPlayer)
	{
		// Check if we are the first player to join
		if (_shipController.Player1)
		{
			// If we have a pilot

			// Make the new player a gunner
			if (_shipController.Player1.myRole == PlayerRole.Pilot)
			{
				_shipController.Player2 = newPlayer;
				_shipController.Player2.myRole = PlayerRole.Gunner;
			}
			else
			{
				_shipController.Player2 = newPlayer;
				_shipController.Player2.myRole = PlayerRole.Pilot;
			}
		}
		else
		{
			// If we dont have a pilot

			// Make the new player a pilot
			_shipController.Player1 = newPlayer;
			_shipController.Player1.myRole = PlayerRole.Pilot;
		}

		// Increment the number of players
		_shipController.NumberOfPlayers++;
	}

	/// <summary> Make the ship shoot </summary>
	public void ShootGun(bool pressed)
	{
		_shipController.ShootGun(pressed);
	}

	/// <summary> Make the ship shoot </summary>
	public void ShootShip( bool pressed)
	{
		_shipController.ShootShip(pressed);
	}

	/// <summary> Swap the ships weapon  </summary>
	public void SwapWeapon(Vector2 Input)
	{
		_shipController.SwapWeapon(Input);
	}

	/// <summary> Steer the ship </summary>
	public void SteerShip(Vector2 velocity)
	{
		// Check if we are steering
		if (velocity == Vector2.zero)
		{
			_shipController.RotateDirection = Vector2.zero;
			_shipController.Rotating = false;
		}
		else
		{
			// Check if we want inverted controls
			if (_shipController.InvertedControls == true)
			{
				_shipController.RotateDirection = new Vector2(velocity.y, velocity.x) * _shipController.TurnMultiplayer;
			}
			else
			{
				_shipController.RotateDirection = new Vector2(-velocity.y, velocity.x) * _shipController.TurnMultiplayer;
			}

			_shipController.Rotating = true;
		}
	}

	/// <summary> Move the gunners camera </summary>
	public void AimGun(Vector2 velocity)
	{
		_shipController.GunVelocity = new Vector2(velocity.x, velocity.y) * _shipController.Properties.GunRotationSpeed;
	}

	/// <summary> Move the Ship up, down, left or right </summary>
	public void StrafeShip(Vector2 velocity)
	{
		if (velocity.sqrMagnitude >= 1)
		{
			_shipController.StrafeDirection = velocity;
			_shipController.Strafing = true;
		}
		else
		{
			_shipController.Strafing = false;
		}
	}

	/// <summary> Rotate the ship on the Z axis </summary>
	public void RotateShip(float direction)
	{
		_shipController.RollInput = direction;
	}

	/// <summary> Change the roles of the players </summary>
	public void SwapRoles()
	{
		if (_shipController.Player1) {
			_shipController.Player1.SwapRole();
		}

		if (_shipController.Player2) {
			_shipController.Player2.SwapRole();
		}

		_shipController.GunVelocity = Vector3.zero;
	}

	/// <summary> Trigger a role swap request </summary>
	public void TriggerRoleSwap(bool pressed)
	{
		// If we only have one player..
		// Just let them swap on command
		if (_shipController.NumberOfPlayers == 1 && pressed == true)
		{
			_shipController.SwapRoles();
			return;
		}

		// If were currently looking for a role swap
		if (_shipController.RoleSwap == true && pressed == true)
		{
			// This means that BOTH players are holding down the role swap button
			_shipController.SwapRoles();
		}
		else
		{
			// Else we want to set looking for roleSwap to whatever the last input was

			// This could be a button press -> start a roleSwap
			// This could be a button release -> cancel the roleSwap

			_shipController.RoleSwap = pressed;
		}
	}

	/// <summary> Toggle the boosting member </summary>
	public void Boost(bool pressed)
	{
		// If our boost gauge is more than 5% full we allow the player to boost
		// This is to avoid stuttering  on a 0% gauge
		if (_shipController.BoostGauge > _shipController.Properties.maxBoostGauge * 0.1f)
		{
			if (pressed)
			{
				_shipController.WarpEffect1.Play();
				_shipController.WarpEffect2.Play();
			}
			else
			{
				_shipController.WarpEffect1.Stop();
				_shipController.WarpEffect2.Stop();
			}

			_shipController.Boosting = pressed;
		}
		else
		{
			_shipController.WarpEffect1.Stop();
			_shipController.WarpEffect2.Stop();

			_shipController.Boosting = false;
		}
	}

	internal void SlowCamera(bool isPressed)
	{
		_shipController.SlowCamera(isPressed);
	}

	/// <summary> Toggle the ships map </summary>
	public void ToggleMap(bool pressed)
	{
	}

	/// <summary> Toggle the lockOn member </summary>
	public void LockOn(bool pressed)
	{
	}
}
