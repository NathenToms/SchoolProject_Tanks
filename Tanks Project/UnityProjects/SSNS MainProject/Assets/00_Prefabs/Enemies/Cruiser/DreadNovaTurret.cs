using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadNovaTurret : MonoBehaviour
{
	[SerializeField] private Transform target;

	[SerializeField] private GameObject projectile;
	[SerializeField] private Transform barrel;

	[SerializeField] private float minRotationAngle = -55.0f;
	[SerializeField] private float maxRotationAngle = 5.0f;
	[SerializeField] private float maxRotation = 1.5f;

	[SerializeField] private float fireRate = 0.2f;

	[SerializeField] private List<Collider> ignore;

	private float _shotCounter;

	// Update is called once per frame
	void Update()
	{
		// If we dont have a target do nothing
		if (target == null) return;

		AIUtilities.LookAtTarget(transform, target.position, maxRotation);
		AIUtilities.ClampTurretRotation(transform, minRotationAngle, maxRotationAngle);

		float angleToPosition = AIUtilities.GetAngleToTarget(transform, target.position);

		if (Vector3.Distance(transform.position, target.position) < 600)
		{
			if (Mathf.Abs(angleToPosition) < 1) {
				if ((_shotCounter += Time.deltaTime) > fireRate)
				{
					_shotCounter = 0.0f;
					Shoot();
				}
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player") {
			target = other.gameObject.transform;
		}
	}

	void Shoot()
	{
		RaycastHit hit;

		if (Physics.Raycast(transform.position, target.transform.position - transform.position, out hit, Mathf.Infinity))
		{
			GameObject shot = Instantiate(projectile, barrel.position, barrel.rotation);
			Collider shotCollider = shot.GetComponent<Collider>();
			foreach (Collider collider in ignore)
			{
				Physics.IgnoreCollision(shotCollider, collider);
			}
		}
	}
}
