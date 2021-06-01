using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState<T> : FSMState where T : EnemyController
{
    protected T controller;
    protected int patrolID = 0;
    protected bool randomPoint;

    //Obstacle variables
    protected bool obstacleHit = false;
    protected float obstacleTimer = 0;
    protected float avoidTime = 2f;

    //Timer for staying in patrol;
    protected float timer1 = 0f;
    protected float timeAfterTransition = 5f;
    //Timer for how often to check for seeing the player
    protected float timer2 = 0f;
    protected float timeOftenCheck = 0.5f;

    //Constructor
    public PatrolState(T enemyController, bool randomizePoint = false)
    {
        controller = enemyController;
        randomPoint = randomizePoint;
        stateID = FSMStateID.Patrolling;

        EnterStateInit();
    }

    //Do this always
    public override void Act()
    {
		if (MenuManager.Instance.Sleeping) return;
        Move(controller.waypoints[patrolID]);
    }

    //Initialize on entering state
    public override void EnterStateInit()
    {
        //Debug.Log("Patrolling");
        timer1 = 0f;
        timer2 = 0f;
    }


    public override void Reason()
	{
		if (MenuManager.Instance.Sleeping) return;
		if (timer1 < timeAfterTransition)
        {
            timer1 += Time.deltaTime;
        }
        if (timer2 < timeOftenCheck)
        {
            timer2 += Time.deltaTime;
        }

        //Check distance to waypoint
        if (Vector3.Distance(controller.transform.position, controller.waypoints[patrolID].transform.position) < controller.WaypointDistance)
        {
            if (randomPoint)
            {
                patrolID = Random.Range(0, controller.waypoints.Length);
            }
            else
            {
                patrolID++;

                if (patrolID >= controller.waypoints.Length) patrolID = 0;
            }
        }

        if (controller.Player != null)
        {
            float pDist = Vector3.Distance(controller.transform.position, controller.Player.transform.position);

            //Check distance to player
            if (pDist < controller.PlayerDistance && timer1 >= timeAfterTransition)
            {
                if (timer2 >= timeOftenCheck)
                {
                    timer2 = 0f;
                    if (controller.PlayerInVision()) // in vision and has been patrolling for minimum time. This is to prevent the ai staying in attack mode and acting weird
                    {
                        timer1 = 0f;
                        if (AIManager.aiManager.CanAttack(controller.aiType))
                        {
                            controller.PerformTransition(Transition.SawPlayer);
                        }
                    }
                }
            }
        }
        //Else dead transition to dead
        else if (controller.Health.IsDead)
        {
            controller.PerformTransition(Transition.NoHealth);
        }
    }

    //Moves
    protected virtual void Move(Transform target)
    {
        if (target != null)
        {
            //Calculate direction
            Vector3 direction = controller.transform.forward; // sets forward
            direction.Normalize();

            if (controller.AvoidObstacles(ref direction)) // will change direction towards the right if an obstacle is in the way
            {
                obstacleHit = true;
            }

            //Rotation
            if (!obstacleHit && obstacleTimer == 0)
            {
                direction = target.transform.position - controller.transform.position; // sets desired direction to target intercept point
            }
            else
            {
                //if obstacles, ignore desired direction and move to the right of obstacles
                obstacleTimer += Time.deltaTime;
                if (obstacleTimer > avoidTime)
                {
                    obstacleTimer = 0;
                    obstacleHit = false;
                }
            }

            Vector3 newDir = Vector3.RotateTowards(controller.transform.forward, direction, controller.Stats.rotationSpeed * Time.deltaTime, 0);
            Quaternion rot = Quaternion.LookRotation(newDir);
            controller.transform.rotation = Quaternion.Lerp(controller.transform.rotation, rot, controller.Stats.rotationSpeed * Time.deltaTime);

            //Movement
            controller.Rigid.AddForce(controller.transform.forward.normalized * controller.Stats.shipSpeed, ForceMode.Acceleration); // move regular speed if obstacle is in the way or player is not target
        }
    }
}