using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FighterController : EnemyController
{
    [SerializeField] float closeDistanceCheck = 90;

    [SerializeField] GameObject bullet;
    [SerializeField] GameObject bulletShootPos;

    protected override void ConstructFSM()
    {
        aiType = AIManager.AITypes.Fighter;
        DeadState deadState = new DeadState(this);
        SpawnState spawnState = new SpawnState(this);
        FighterPatrolState patrol = new FighterPatrolState(this, true);
        FighterAttackState attack = new FighterAttackState(this, bullet, bulletShootPos);

        spawnState.AddTransition(Transition.Patrol, FSMStateID.Patrolling);

        patrol.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        patrol.AddTransition(Transition.SawPlayer, FSMStateID.Attacking); //Change this

        attack.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        attack.AddTransition(Transition.Patrol, FSMStateID.Patrolling);

        //What's the difference between saw player and attack transition?
        AddFSMState(spawnState);

        AddFSMState(patrol);
        AddFSMState(attack);
        AddFSMState(deadState);
    }

    //Getters
    public float CloseDistance { get { return closeDistanceCheck; } }
}
