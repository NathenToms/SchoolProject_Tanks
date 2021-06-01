using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

// NOTE -- Things to know..
// 1. The Cruiser will destroy obstacles when patrolling
// 2. The Cruiser will ONLY! destroy obstacles when patrolling
// 3. The Cruiser attack the player with a large attack when in the attacking state
// this is followed by a transition into the escape state where the cruiser will run away

public class CruiserEnemy : MonoBehaviour
{
	private Animator _animator;

	public CruiserObstacleTurret obstacleTurretL;
	public CruiserObstacleTurret obstacleTurretR;

	public float _initialSpeed = 100.0f;
	public float _currentSpeed;

	public float _initialMinTurn = 0.20f;
	public float _currentMinTurn;

	public bool _moveUp;

	public List<Vector3> Waypoints;

	public float TargetDistanceToPoint = 150f;
	public float MinTimeToIdle = 35f;
	public float EscapeRange = 700f;
	public float AggroRange = 500f;

	[SerializeField] private Transform shield;
	[SerializeField] private Transform obstacleScanner;
	[SerializeField] private Collider shieldCollider;

	public void shieldUp() => shieldCollider.enabled = true;
	public void shieldDown() => shieldCollider.enabled = false;

	public Vector3 TargetPos;

	// Start is called before the first frame update
	private void Awake()
	{
		_animator = GetComponent<Animator>();

		_currentSpeed = _initialSpeed;
		_currentMinTurn = _initialMinTurn;
	}

	// Start is called before the first frame update
	private void Start()
	{
		Waypoints = AIUtilities.GenerateWaypoints(transform.position, 1000, 25);
	}

	private void Update()
	{
		shield.transform.rotation = Quaternion.identity;

		RaycastHit hit;

		// Check for the dreadnova
		// _moveUp = Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, 1 << 8);
	}

	// Draw Gizmos each frame
	private void OnDrawGizmos()
	{
		if (Application.isPlaying == false) return;

		//Gizmos.color = Color.red;
		//Gizmos.DrawWireSphere(transform.position, AggroRange);

		//Gizmos.color = Color.blue;
		//Gizmos.DrawWireSphere(transform.position, EscapeRange);

		foreach (Vector3 point in Waypoints)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(point, 5);
		}

		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position, TargetPos);
	}

	private void OnCollisionEnter(Collision collision)
	{
		// TODO: If an astroide enter my rande, take damage and destroy it
	}

	// Move and Rotate the cruiser ship
	public void Move(Vector3 newPosition)
	{
		if (_moveUp)
		{
			transform.rotation *= Quaternion.AngleAxis( -Time.deltaTime * 20, new Vector3(1, 0, 0));
		}
		else
		{
			Vector3 rot = Vector3.RotateTowards(transform.forward, newPosition - transform.position, _currentMinTurn * Time.deltaTime, 0.0f);
			transform.rotation = Quaternion.LookRotation(rot, transform.up);
		}

		transform.Translate(Vector3.forward * Time.deltaTime * _currentSpeed);
	}

	public void CruiserAttack()
	{
		print("Big Attack");
	}
}

public static class AIUtilities
{
	public static List<Vector3> GenerateWaypoints(Vector3 center, float radius, int wpCount = 15)
	{
		List<Vector3> points = new List<Vector3>();

		for (int count = 0; count < wpCount; count++)
		{
			float x = Random.Range(-0.5f, 0.5f);
			float y = Random.Range(-0.5f, 0.5f);
			float z = Random.Range(-0.5f, 0.5f);

			Vector3 direction = new Vector3(x, y, z) * Random.Range(radius - (radius / 5f), radius);

			points.Add(center + direction);
		}

		return points;
	}

	public static List<GameObject> ScanForAllObstacle(Vector3 center, float radius)
	{
		List<GameObject> objects = new List<GameObject>();

		foreach (Collider collider in Physics.OverlapSphere(center, radius, LayerMask.GetMask("Obstacles")))
		{
			objects.Add(collider.gameObject);
		}

		return objects;
	}

	public static void LookAtTarget(Transform transform, Vector3 target, float maxRotation = 1.5f)
	{
		Vector3 currentForward = transform.forward;
		Vector3 targetPoint = target - transform.position;

		Vector3 rot = Vector3.RotateTowards(currentForward, targetPoint, maxRotation * Time.deltaTime, 0.0f);

		transform.rotation = Quaternion.LookRotation(rot, transform.up);
	}

	public static void ClampTurretRotation(Transform transform, float minAngle, float maxAngle)
	{
		Vector3 currentRotation = transform.localRotation.eulerAngles;

		// Clamp the Min Rotation
		if (currentRotation.x < 360 + minAngle && currentRotation.x > 180) {
			currentRotation.x = 360 + minAngle;
		}

		// Clamp the Max Rotation
		if (currentRotation.x > maxAngle && currentRotation.x < 180) {
			currentRotation.x = maxAngle;
		}

		// Keep the Z at 0
		currentRotation.z = 0;

		// Set the final Rotation
		transform.localRotation = Quaternion.Euler(currentRotation);
	}

	public static float GetAngleToTarget(Transform transform, Vector3 target)
	{
		Vector3 toPosition = (target - transform.position).normalized;
		return Vector3.Angle(transform.forward, toPosition);
	}
}
