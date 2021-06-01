using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockLeaderController : EnemyController
{
    [SerializeField] float attackStateRadius;
    [SerializeField] float patrolStateRadius;
    [SerializeField] float defenseStateRadius;

    public float AttackRadius { get { return attackStateRadius; } }
    public float PatrolRadius { get { return patrolStateRadius; } }
    public float DefenseRadius { get { return defenseStateRadius; } }

    protected override void ConstructFSM()
    {
        aiType = AIManager.AITypes.Swarmer;
        Flock swarm = transform.parent.GetComponent<Flock>();
        //States
        SpawnState spawn = new SpawnState(this);
        SwarmLeaderPatrolState patrol = new SwarmLeaderPatrolState(this, swarm);
        SwarmLeaderAttackState attack = new SwarmLeaderAttackState(this, swarm);
        SwarmLeaderDefendState defend = new SwarmLeaderDefendState(this, swarm);

        spawn.AddTransition(Transition.Defend, FSMStateID.Defend);

        //Transitions
        patrol.AddTransition(Transition.Attack, FSMStateID.Attacking);
        patrol.AddTransition(Transition.Patrol, FSMStateID.Patrolling);
            
        attack.AddTransition(Transition.Patrol, FSMStateID.Patrolling);
        attack.AddTransition(Transition.Defend, FSMStateID.Defend);

        defend.AddTransition(Transition.Patrol, FSMStateID.Patrolling);

        //Add states
        AddFSMState(spawn);
        AddFSMState(defend);
        AddFSMState(patrol);
        AddFSMState(attack);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 2.0f);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * 50);
    }
}