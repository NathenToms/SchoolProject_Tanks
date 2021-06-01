using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackState<T> : FSMState where T : EnemyController
{
    protected T controller;
    public Vector3 interceptPoint;

    //Obstacle variables
    protected Vector3 obstacleAvoidDirection = Vector3.right;
    protected bool obstacleHit = false;
    protected float obstacleTimer = 0;
    protected float avoidTime = 2f;

    //constructor
    public AttackState(T chaserController)
    {
        controller = chaserController;

        stateID = FSMStateID.Attacking;

        EnterStateInit();
    }

    public override void EnterStateInit()
    {
        //Debug.Log("Attacking");
        obstacleHit = false;
        obstacleTimer = 0;
    }

    protected abstract void CalculateIntercept();

    protected abstract void Move();
}