using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class EnemyController : AdvancedFSM
{
    [Header("Green: forward direction and collision check.")]
    [Header("Red: intercept calculation.")]
    [Header("Blue: Velocity and collision check.")]
    [SerializeField] bool debugDraw = false;

    public Transform[] waypoints;

    public EnemyStats myStats;

    [Header("Distance Checks")]
    [SerializeField] float collisionDistanceCheck = 150f;
    [SerializeField] float waypointDistanceCheck = 50f;
    [SerializeField] float playerDistanceCheck = 100f;
    [SerializeField] float attackDistanceCheck = 80f;
    [SerializeField] float patrolDistanceCheck = 200f;

    [Header("Collision Properties")]
    [SerializeField] LayerMask obstacleLayer;

    [Tooltip("Size of ray for collision checking. Larger numbers will mean the avoidance is larger")]
    [SerializeField] float raySize = 7.5f;

    [SerializeField] GameObject thrusters;

    private Rigidbody rbSelf;
    private HealthAndShields health;

    private Vector3 spawnpoint;

    private Vector3 spawnDestination;

    [HideInInspector] public bool showStats;
    [HideInInspector] public AIManager.AITypes aiType = AIManager.AITypes.Null;

    public virtual void ResetEnemy()
    {
        StopAllCoroutines();

        if (health)
            health.ResetValues();

        if (thrusters) thrusters.SetActive(false);
        transform.position = spawnpoint;
        transform.rotation = Quaternion.LookRotation(spawnDestination - spawnpoint);
        if (thrusters) thrusters.SetActive(true);

        PerformTransition(Transition.Reset);
    }

    protected abstract void ConstructFSM();

    private void Awake()
    {
        TryGetComponent(out rbSelf);
        TryGetComponent(out health);
    }

    protected override void Initialize()
    {
        ConstructFSM();

        //Removes from AIManager on death
        if (health != null)
        {
            health.onDeath += OnDeath;
        }
    }

    protected override void FSMFixedUpdate()
    {
        //Do this
        if (CurrentState != null && !MenuManager.Instance.Sleeping)
		{
            CurrentState.Act();
        }
    }
    protected override void FSMUpdate()
    {
        //Do this
        if (CurrentState != null && !MenuManager.Instance.Sleeping)
        {
            CurrentState.Reason();
        }
    }

    public bool AvoidObstacles(ref Vector3 dir)
    {
        bool hit = false;
        RaycastHit hitInfo;

        //Check direction facing
        if (Physics.SphereCast(transform.position, RaySize, transform.forward.normalized, out hitInfo, CollisionDistance, ObstacleLayer))/* || Physics.SphereCast(controller.transform.position, controller.RaySize, controller.rbSelf.velocity.normalized, out hitInfo, controller.CollisionCheckDistance, controller.ObstacleLayer))*/
        {
            // Get the desired direction we need to move to move around  the obstacle. Transform to world co-ordinates (gets the obstacleMoveDirection wrt the current foward direction).
            Vector3 turnDir = transform.TransformDirection(hitInfo.normal);
            turnDir.Normalize();

            dir += turnDir;
            hit = true;
        }
        if (rbSelf != null && Physics.SphereCast(transform.position, RaySize, rbSelf.velocity.normalized, out hitInfo, CollisionDistance, ObstacleLayer))
        {
            // Get the desired direction we need to move to move around  the obstacle. Transform to world co-ordinates (gets the obstacleMoveDirection wrt the current foward direction).
            Vector3 turnDir = transform.TransformDirection(hitInfo.normal);
            turnDir.Normalize();

            dir += turnDir;
            hit = true;
        }
        return hit;
    }

    public bool PlayerInVision()
    {
        Vector3 dir = Player.transform.position - transform.position;
        Ray ray = new Ray(transform.position, dir);
        RaycastHit hitInfo;
        LayerMask layerMask = LayerMask.GetMask("Obstacles");
        layerMask += LayerMask.GetMask("Player"); //Add player layer to obstacles
        layerMask += LayerMask.GetMask("Enemies"); //Add player layer to obstacles

        Physics.SphereCast(ray, raySize / 2, out hitInfo, Mathf.Infinity, layerMask);

        if (hitInfo.collider != null)
        {
            //Debug.Log(hitInfo.collider.gameObject.name);
            if (hitInfo.collider.gameObject.Equals(Player))
            {
                return true;
            }
        }
        return false;
    }

    //Draw debug rays
    private void OnDrawGizmos()
    {
        if (debugDraw)
        {
            Debug.DrawRay(transform.position, transform.forward.normalized * CollisionDistance, Color.green); //Forward ray
                                                                                                              //Debug.DrawRay(transform.position, interceptPoint - transform.position, Color.red); // Intercept point
            if (rbSelf)
            {
                //Vector3 dir = rbSelf.velocity;
                Debug.DrawRay(transform.position, rbSelf.velocity.normalized * CollisionDistance, Color.blue); //Velocity ray
            }
        }
    }

    private void OnDeath()
    {
        health.onDeath -= OnDeath;
        AIManager.aiManager.StopAttack(this.aiType);
    }

    public GameObject Player { get { return GameManager.Instance.Player.gameObject; } }
    public HealthAndShields Health { get { return health; } }
    public EnemyStats Stats { get { return myStats; } }
    public Rigidbody Rigid { get { return rbSelf; } }
    public LayerMask ObstacleLayer { get { return obstacleLayer; } }
    public Vector3 Spawn { get { return spawnpoint; } set { spawnpoint = value; } }
    public Vector3 SpawnDestination { get { return spawnDestination; } set { spawnDestination = value; } }
    public float CollisionDistance { get { return collisionDistanceCheck; } }
    public float WaypointDistance { get { return waypointDistanceCheck; } }
    public float AttackDistance { get { return attackDistanceCheck; } }
    public float PatrolDistance { get { return patrolDistanceCheck; } }
    public float PlayerDistance { get { return playerDistanceCheck; } }
    public float RaySize { get { return raySize; } }
}