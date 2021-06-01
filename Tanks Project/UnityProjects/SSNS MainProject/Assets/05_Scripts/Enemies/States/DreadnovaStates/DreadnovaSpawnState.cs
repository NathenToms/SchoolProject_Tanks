using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadnovaSpawnState : FSMState
{
    private DreadnovaController controller;

    public DreadnovaSpawnState(DreadnovaController enemyController)
    {
        controller = enemyController;
        stateID = FSMStateID.Spawned;
    }

    public override void Act()
    {
    }

    public override void Reason()
    {
        if (GameManager.Instance.GameState == SNSSTypes.GameState.BATTLE)
        {
            if (!controller.warping)
            {
                if (controller.State == SNSSTypes.DreadnovaState.SHIELD_STAGE)
                    controller.PerformTransition(Transition.Defend);
                else if (controller.State == SNSSTypes.DreadnovaState.FINAL_STAGE)
                    controller.PerformTransition(Transition.Attack);

                controller.gameObject.SetActive(true);
            }
        }
    }
}
