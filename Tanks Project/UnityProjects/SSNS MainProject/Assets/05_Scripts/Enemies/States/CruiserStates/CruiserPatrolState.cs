using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The attack state for the Cruiser enemy
/// </summary>
public class CruiserPatrolState : PatrolState<CruiserController>
{
	// Reference to the parent controller
	CruiserController myController;

	// The points to go to when Im patrolling
	List<Vector3> patrolPoints = new List<Vector3>();

	#region Members

	float counter = 0.0f;

	#endregion

	/// <summary>
	/// Constructor
	/// </summary>
	public CruiserPatrolState(CruiserController controller) : base(controller)
	{
	}

	/// <summary>
	/// Change State
	/// </summary>
	public override void Reason()
	{
		// Death
		if (myController.Health.IsDead) {
			myController.PerformTransition(Transition.NoHealth);
			return;
		}

		// Saw Player
		if ((counter += Time.deltaTime) > 0.25f)
		{
			if (myController.CheckPlayer(myController.PlayerDistance) == true) {
				myController.PerformTransition(Transition.SawPlayer);
			}

			counter = 0.0f;
		}

		// If we're in range of the next patrol point, we move to the next one
		if (Vector3.Distance(myController.transform.position, patrolPoints[patrolID]) < myController.WaypointDistance)
		{
			if (randomPoint)
			{
				patrolID = Random.Range(0, patrolPoints.Count);
			}
			else
			{
				patrolID++;

                Debug.Log("Changing States");

				if (patrolID >= patrolPoints.Count) patrolID = 0;
			}
		}
	}
}
