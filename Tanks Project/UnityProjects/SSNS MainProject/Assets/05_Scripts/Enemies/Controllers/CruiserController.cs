using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Controller for the Cruiser Enemy
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class CruiserController : EnemyController
{
	// What should I be doing when an enemy is in range
	[Header("Cruiser State")]
	public CruiserState myState = CruiserState.Defensive;

	/// <summary>
	/// Start is called before the first frame update
	/// </summary>
	protected override void Initialize()
	{
		// Create our states
		ConstructFSM();
	}

	/// <summary>
	/// Create the state machine and behavior for this enemy
	/// </summary>
	protected override void ConstructFSM()
	{
		// Dead State
		DeadState deadState = new DeadState(this);

		// Attacking
		CruiserAttackState attackState = new CruiserAttackState(this);
		attackState.AddTransition(Transition.Patrol, FSMStateID.Patrolling);
		attackState.AddTransition(Transition.NoHealth, FSMStateID.Dead);

		// Patrolling
		CruiserPatrolState patrolState = new CruiserPatrolState(this);
		patrolState.AddTransition(Transition.SawPlayer, FSMStateID.Attacking);
		patrolState.AddTransition(Transition.NoHealth, FSMStateID.Dead);

		// Add the States
		AddFSMState(patrolState);
		AddFSMState(attackState);
		AddFSMState(deadState);
	}

	/// <summary>
	/// Update this ship based on its State
	/// Aggro of Defensive
	/// </summary>
	public void InitializeShipStats()
	{

	}

	/// <summary>
	/// Update this ship based on its State
	/// Aggro of Defensive
	/// </summary>
	public void AimGuns()
	{
		//if (player == null) {
		//	return;
		//}

		//foreach (Transform gun in guns)
		//{
		//	Debug.DrawLine(gun.position, player.transform.position);
		//	gun.transform.LookAt(player.transform.position, Vector3.up);
		//	gun.Rotate(90, 0, 0);
		//}
	}

	#region Helper Methods

	// Check if 2 gameobjects are within range
	public bool InRange(GameObject a, GameObject b, float distance)
	{
		return (a.transform.position - b.transform.position).sqrMagnitude < (distance * distance);
	}

	// Check if the player is in range AND the ship can see the player
	public bool CheckPlayer(float distance)
	{
		if (Player == null)
		{
			return false;
		}
		else
		{
			// If we made it here we know we have a player
			// We want to raycast to see if there's an obstacle between this enemy and the player
			// Else we either don't have a player in range OR an obstacle is blocking the bath between us

			// If the player is in range
			if (InRange(gameObject, Player, distance))
			{
				#region Check for Obstacles

				LayerMask mask = new LayerMask();

				mask += LayerMask.GetMask("Obstacles");
				mask += LayerMask.GetMask("Player");

				RaycastHit hit;

				// Waycast from this enemy to the player
				if (Physics.Raycast(transform.position, (Player.transform.position - transform.position).normalized * distance, out hit, mask)) {

					// NOTE -- I'm not doing a null check on the collider. I might be wrong,but I dont think there will ever be a hit on something without a collider

					// If we hit the player
					if (hit.collider.gameObject.Equals(Player)) {
						return true;
					}
					
					//Something Is blocking the player
					return false;
				}
				#endregion

				// If we dident hit anything, we know we "should" have found the player
				return true;
			}
			else
			{
				// Not in range
				return false;
			}
		}
	}
	#endregion
}

/// <summary>
/// The different behaviour states for the cruiser enemy
/// </summary>
public enum CruiserState
{
	Defensive,
	Offensive
}
