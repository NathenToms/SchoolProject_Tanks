using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CargoController : EnemyController
{
    GameObject capitalShip;
    public bool hitPlayer = false;

    protected override void ConstructFSM()
    {
        gameObject.TryGetComponent<HealthAndShields>(out HealthAndShields has);

        DeadState deadState = new DeadState(this);
        CargoPatrolState patrol = new CargoPatrolState(this, has, gameObject);

        patrol.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        patrol.AddTransition(Transition.SawPlayer, FSMStateID.Attacking); //Change this

        AddFSMState(patrol);
        AddFSMState(deadState);
    }

    protected override void Initialize()
    {
        base.Initialize();

        capitalShip = GameObject.FindGameObjectWithTag("CapitalShip");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.Equals(Player) && CurrentStateID == FSMStateID.Attacking)
        {
            hitPlayer = true;
        }
    }

    //Getters
    public GameObject CapitalShip { get { return capitalShip; } }
}
