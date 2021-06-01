using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoPatrolState : PatrolState<CargoController>
{
    private bool reachedShip;
    float waitToDestroy = 0.0f;
    HealthAndShields health;
    GameObject thisGO;

    public CargoPatrolState(CargoController enemyController, HealthAndShields has, GameObject thisGo) : base(enemyController)
    {
        thisGO = thisGo;
        health = has;
    }


    //Do this always
    public override void Act()
    {
        Move(controller.CapitalShip.transform);
    }

    //Initialize on entering state
    public override void EnterStateInit()
    {
        //Debug.Log("Patrolling");
        controller.hitPlayer = false;
    }


    public override void Reason()
    {
        //Check distance to the capital ship
        if (Vector3.Distance(controller.transform.position, controller.CapitalShip.transform.position) < controller.WaypointDistance)
        {
            if (!reachedShip)
            {
                //Do reaching capitol ship things
                reachedShip = true;
                Debug.Log("reached the ship");
                DrdRandomShooty.manager.Heal();
                foreach (Collider c in thisGO.GetComponentsInChildren<Collider>())
                {
                    c.enabled = false;
                }
            }

            waitToDestroy += Time.deltaTime;
            
            if(waitToDestroy > 10.0f)
            {
                health.TakeDamage(Mathf.Infinity, Mathf.Infinity);
            }
        }

        //If dead transition to dead
        if (controller.Health.IsDead)
        {
            controller.PerformTransition(Transition.NoHealth);
        }
    }
}