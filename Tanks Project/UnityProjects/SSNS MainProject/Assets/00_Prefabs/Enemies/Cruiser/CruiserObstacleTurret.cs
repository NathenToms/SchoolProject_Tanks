using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CruiserObstacleTurret : MonoBehaviour
{	
	[SerializeField] private CruiserTargetArea targetArea;
	[SerializeField] private Collider obstacleArea;

	[SerializeField] private Transform _target;

	[SerializeField] private GameObject projectile;
	[SerializeField] private Transform barrel;

	[SerializeField] private float minRotationAngle = -55.0f;
	[SerializeField] private float maxRotationAngle = 5.0f;
	[SerializeField] private float maxRotation = 1.5f;

	[SerializeField] private float fireRate = 0.2f;

	// Update is called once per frame
	void Update()
	{
		if (_target == null)
		{
			GameObject target = targetArea.GetObstacle();
			if (target) _target = target.transform;
		}
		else
		{
			AIUtilities.LookAtTarget(transform, _target.position, maxRotation);
			AIUtilities.ClampTurretRotation(transform, minRotationAngle, maxRotationAngle);

			float angleToPosition = AIUtilities.GetAngleToTarget(transform, _target.position);

			if (Mathf.Abs(angleToPosition) < 1)
			{ 
				if (coRunning == false) {
					StartCoroutine(coShoot());
				}
			}
		}
	}

	bool coRunning = false;
	IEnumerator coShoot()
	{
		coRunning = true;
		yield return new WaitForSeconds(1.5f);
		if (_target)
		{
			Destroy(_target.gameObject);
		}
		coRunning = false;
	}

	// Draw the Gizmos
	void OnDrawGizmos()
	{
		if (_target == null) return;

		Gizmos.DrawLine(transform.position, _target.position);
	}
}
