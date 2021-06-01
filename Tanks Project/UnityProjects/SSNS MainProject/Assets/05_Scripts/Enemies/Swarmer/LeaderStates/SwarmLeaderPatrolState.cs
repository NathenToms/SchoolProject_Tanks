using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmLeaderPatrolState : FSMState
{
    FlockLeaderController controller;
    Flock swarm;

    int patrolID = 0;
    bool randomPoint;
    bool obstacleHit = false;
    float obstacleTimer;
    float avoidTime = 1f;

    public SwarmLeaderPatrolState(FlockLeaderController leader, Flock swarmObj, bool randomize = true)
    {
        controller = leader;
        randomPoint = randomize;
        swarm = swarmObj;
        swarm.swarmFollowRadius = controller.PatrolRadius;

        if (randomPoint) patrolID = Random.Range(0, controller.waypoints.Length);


        stateID = FSMStateID.Patrolling;
    }

    public override void Act()
    {
        Move(controller.waypoints[patrolID]);
    }

    public override void Reason()
    {
        if (swarm.defenseTarget != null)
        {
            //Enter defend state mode
            controller.PerformTransition(Transition.Defend);
        }

        if (swarm.player != null)
        {
            //Check distance to player
            if (Vector3.Distance(controller.transform.position, swarm.player.transform.position) <= controller.PlayerDistance)
            {
                if (AIManager.aiManager.CanAttack(controller.aiType))
                {
                    controller.PerformTransition(Transition.Attack);
                }
            }
        }
        else
        {
            swarm.player = GameObject.FindGameObjectWithTag("Player");
        }

        //Check waypoint distance
        if (controller.waypoints.Length > 0)
        {
            if (Vector3.Distance(controller.transform.position, controller.waypoints[patrolID].transform.position) <= controller.WaypointDistance) //Check distance to current waypoint
            {
                if (randomPoint)
                {
                    patrolID = Random.Range(0, controller.waypoints.Length); //Choose random patrol point
                }
                else
                {
                    patrolID++;//Progress to next waypoint

                    if (patrolID >= controller.waypoints.Length) patrolID = 0; //Circle back to first waypoint
                }
            }
        }
    }

    public override void EnterStateInit()
    {
        //Do this when entering the state
        swarm.swarmFollowRadius = controller.PatrolRadius;
    }

    //void Move()
    //{
    //    if (controller.waypoints.Length > 0 && controller.waypoints[patrolID] != null)
    //    {
    //        //Move towards position. No need to worry about obstacles or 
    //        controller.transform.position = Vector3.MoveTowards(controller.transform.position, controller.waypoints[patrolID].transform.position, controller.Stats.shipSpeed * Time.deltaTime);
    //    }
    //}

    void Move(Transform target)
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

            Quaternion rot = Quaternion.LookRotation(direction.normalized, controller.transform.up);
            controller.transform.rotation = Quaternion.Lerp(controller.transform.rotation, rot, controller.Stats.rotationSpeed * Time.deltaTime);

            controller.transform.position += controller.transform.forward * controller.Stats.shipSpeed * Time.deltaTime;
        }
    }
}