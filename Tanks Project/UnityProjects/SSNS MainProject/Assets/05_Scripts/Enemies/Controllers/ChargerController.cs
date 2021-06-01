using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ChargerController : EnemyController
{
    public bool hitPlayer = false;

    ChargerAttackState attack;

    protected override void ConstructFSM()
    {
        base.aiType = AIManager.AITypes.Charger;
        DeadState deadState = new DeadState(this);
        SpawnState spawnState = new SpawnState(this);
        ChargerPatrolState patrol = new ChargerPatrolState(this, true);
        attack = new ChargerAttackState(this);

        spawnState.AddTransition(Transition.Patrol, FSMStateID.Patrolling);

        patrol.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        patrol.AddTransition(Transition.SawPlayer, FSMStateID.Attacking); //Change this

        attack.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        attack.AddTransition(Transition.Patrol, FSMStateID.Patrolling);
        //What's the difference between saw player and attack transition?

        AddFSMState(spawnState);
        AddFSMState(patrol);
        AddFSMState(deadState);
        AddFSMState(attack);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Player && collision.gameObject.Equals(Player) && CurrentStateID == FSMStateID.Attacking)
        {
            hitPlayer = true;
            AIManager.aiManager.StopAttack(aiType);
            PerformTransition(Transition.Patrol);
        }
    }

    private void OnDrawGizmos()
    {
        if (attack != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, attack.interceptPoint);
        }
    }
}