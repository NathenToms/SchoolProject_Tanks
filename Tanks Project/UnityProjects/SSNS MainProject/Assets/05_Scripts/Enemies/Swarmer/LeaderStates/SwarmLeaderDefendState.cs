using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a defense state that causes the swarm to move with and surround a given target to the flock script.
/// This state will override other states and is controlled by a boolean inside Flock.cs. This should be set by the Game Manager based on game state.
/// </summary>
public class SwarmLeaderDefendState : FSMState
{
    FlockLeaderController controller;
    Flock swarm;

    bool obstacleHit = false;
    float obstacleTimer;
    float avoidTime = 1f;

    public SwarmLeaderDefendState(FlockLeaderController leaderController, Flock flock)
    {
        controller = leaderController;
        swarm = flock;

        swarm.swarmFollowRadius = controller.DefenseRadius; //Set swarm freedom radius to defense radius

        stateID = FSMStateID.Defend;
    }

    public override void Act()
    {
        if(swarm.defenseTarget != null) Move(swarm.defenseTarget.transform);
    }

    public override void Reason()
    {
        if(swarm.defenseTarget == null)
        {
            controller.PerformTransition(Transition.Patrol);
        }
    }

    //private void Move()
    //{
    //    if(swarm.defenseTarget != null)
    //    {
    //        //Moves swarm towards the ship to defend
    //        controller.transform.position = Vector3.MoveTowards(controller.transform.position, swarm.defenseTarget.transform.position, controller.Stats.shipSpeed * Time.deltaTime);
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

            Vector3 newDir = Vector3.RotateTowards(controller.transform.forward, direction, controller.Stats.rotationSpeed * Time.deltaTime, 0);
            Quaternion rot = Quaternion.LookRotation(newDir);
            controller.transform.rotation = Quaternion.Lerp(controller.transform.rotation, rot, controller.Stats.rotationSpeed * Time.deltaTime);

            controller.transform.position = Vector3.MoveTowards(controller.transform.position, controller.transform.position + controller.transform.forward, controller.Stats.shipSpeed * Time.deltaTime);
        }
    }

    public override void EnterStateInit()
    {
        swarm.swarmFollowRadius = controller.DefenseRadius; //Set swarm freedom radius to defense radius
    }
}