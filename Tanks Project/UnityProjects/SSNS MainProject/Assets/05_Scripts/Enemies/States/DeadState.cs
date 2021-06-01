using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : FSMState
{
    private EnemyController controller;
    public DeadState(EnemyController enemyController)
    {
        controller = enemyController;
        stateID = FSMStateID.Dead;
    }

    public override void Act()
    {
            //Do nothing
    }

    public override void Reason()
    {
        //Do nothing
    }
}
