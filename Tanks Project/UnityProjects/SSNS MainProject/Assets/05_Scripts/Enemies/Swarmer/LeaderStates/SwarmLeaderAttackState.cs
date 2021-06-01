using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SwarmLeaderAttackState : FSMState
{
    FlockLeaderController controller;
    Flock swarm;

    bool obstacleHit = false;
    float obstacleTimer;
    float avoidTime = 1f;

    public SwarmLeaderAttackState(FlockLeaderController leader, Flock swarmObj)
    {
        controller = leader;
        swarm = swarmObj;

        stateID = FSMStateID.Attacking;
    }

    public override void Act()
    {
        Move(swarm.player.transform);
    }

    public override void Reason()
    {
        if (swarm.defenseTarget != null)
        {
            //Enter defend state mode
            AIManager.aiManager.StopAttack(controller.aiType);
            controller.PerformTransition(Transition.Defend);
        }
        
        if (AreaManager.Instance.CurrentArea.IsTransformOutside(controller.transform))
        {
            AIManager.aiManager.StopAttack(controller.aiType);
            controller.PerformTransition(Transition.Patrol);
        }

        if (swarm.player == null)
        {
            swarm.player = GameObject.FindGameObjectWithTag("Player");

            if (swarm.player == null)
            {
                //If no player can be found then go to patrol state
                AIManager.aiManager.StopAttack(controller.aiType);
                controller.PerformTransition(Transition.Patrol);
                return;
            }
        }

        //Determining agent freedom from leader
        if(Vector3.Distance(controller.transform.position, swarm.player.transform.position) <= 100f)
        {
            swarm.swarmFollowRadius = controller.AttackRadius;
        }
        else if(swarm.swarmFollowRadius == controller.AttackRadius)
        {
            swarm.swarmFollowRadius = controller.PatrolRadius;
        }
    }

    public override void EnterStateInit()
    {
        //Do this when entering the state
        //swarm.swarmFollowRadius = controller.AttackRadius;
    }

    //void Move()
    //{
    //    if (swarm.player != null)
    //    {
    //        //Move towards position. No need to worry about obstacles or 
    //        controller.transform.position = Vector3.MoveTowards(controller.transform.position, swarm.player.transform.position, controller.Stats.attackShipSpeed * Time.deltaTime);
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

            controller.transform.position += controller.transform.forward * controller.Stats.attackShipSpeed * Time.deltaTime;
        }
    }
}