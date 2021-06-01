using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// The attack state for the Cruiser enemy
/// </summary>
public class CruiserAttackState : AttackState<CruiserController>
{
	// Reference to the parent controller
	CruiserController myController;

	float stateCounter = 0.0f;
	float shotCounter = 0.0f;

	bool inAttackRange = false;

	public CruiserAttackState(CruiserController controller) : base(controller)
	{
	}

	/// <summary>
	/// Works as on enabled
	/// </summary>
	public override void EnterStateInit()
	{
		stateCounter = 0.0f;
		shotCounter = 0.0f;
	}

	/// <summary>
	/// Change State
	/// </summary>
	public override void Reason()
	{
		if (MenuManager.Instance.Sleeping) return;

		// Dead
		if (myController.Health.IsDead)
		{
			myController.PerformTransition(Transition.NoHealth);
			return;
		}
		
		// Patroling
		if ((stateCounter += Time.deltaTime) > 0.25f)
		{
			if (myController.CheckPlayer(myController.PatrolDistance) == false) {
				myController.PerformTransition(Transition.Patrol);
			}

			stateCounter = 0.0f;
		}
	}

	/// <summary>
	/// Works as Update
	/// </summary>
	public override void Act()
	{
		if (MenuManager.Instance.Sleeping) return;

		if (myController.Player != null)
		{
			Vector3 playerPos = myController.Player.transform.position;
			Vector3 currentPos = myController.transform.position;

			Quaternion targetRotation = Quaternion.LookRotation(myController.Player.transform.position - myController.transform.position);

			// Are we in range to shoot?
			inAttackRange = Vector3.Distance(playerPos, currentPos) < myController.AttackDistance;
			inAttackRange = Quaternion.Angle(myController.transform.rotation, targetRotation) < 5;

			if ((shotCounter += Time.deltaTime) > 1f) {

				// If we are, we want to Aim at the player
				if (inAttackRange == true)
				{
					Shoot();
				}
				else
				{
					Vector3 direction = (myController.Player.transform.position - myController.transform.position).normalized;

					float speed = 55f * 12f;

					myController.Rigid.AddForce(direction * speed * Time.deltaTime, ForceMode.Acceleration);
					myController.transform.rotation = Quaternion.Slerp(myController.transform.rotation, targetRotation, 0.01f);
				}

				shotCounter = 0.0f;
			}
		}
	}

	/// <summary>
	/// Aim and Shoot at the player
	/// </summary>
	public void Shoot()
	{
		if (myController.myState == CruiserState.Defensive)
		{
			// We are in normal mode

		}
		else
		{
			// We are in aggro mode

		}
	}

	protected override void CalculateIntercept()
	{
	}

	protected override void Move()
	{
	}
}
