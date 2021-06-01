using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerPatrolState : PatrolState<ChargerController>
{
    public ChargerPatrolState(ChargerController enemyController, bool randomizePoint = false) : base(enemyController, randomizePoint)
    {
    }

    //Initialize on entering state
    public override void EnterStateInit()
    {
        base.EnterStateInit();
        controller.hitPlayer = false;
    }
}
