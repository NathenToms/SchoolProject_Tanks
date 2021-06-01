using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

public class DreadnovaEscapeState : FSMState
{
    private DreadnovaController controller;

    private bool warped;

    public DreadnovaEscapeState(DreadnovaController enemyController)
    {
        controller = enemyController;
        stateID = FSMStateID.Running;
    }

    public override void EnterStateInit()
    {
        warped = false;
    }

    public override void Act()
    {

    }

    public override void Reason()
    {
        if (GameManager.Instance.GameState == SNSSTypes.GameState.BATTLE)
        {
            if (!warped)
            {
                warped = true;
                controller.WarpDreadnova();
            }
            else if (!controller.warping)
            {
                controller.PerformTransition(Transition.NoHealth);

                GameManager.Instance.SwitchState(GameState.BATTLE_END);
            }
        }
    }
}
