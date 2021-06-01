using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

public class DreadnovaShieldState : FSMState
{
    private DreadnovaController controller;

    private bool shieldGone;

    private float shieldDissolve;

    private float waveTime;
    private int waveCount;
    private float cargoTime;

    public DreadnovaShieldState(DreadnovaController enemyController)
    {
        controller = enemyController;
        stateID = FSMStateID.Defend;
    }

    public override void EnterStateInit()
    {
        shieldGone = false;
        shieldDissolve = 1;
        controller.dreadnovaShield.SetActive(true);
        controller.Spawner.enabled = true;

        waveTime = controller.Spawner.Wave.TimeBetweenWaves;
        waveCount = 0;
        cargoTime = 0;

        foreach (ShieldGenerator generator in controller.Generators)
        {
            generator.InitializeGenerator();
        }
    }

    public override void Reason()
    {
        if (IsGeneratorsDead())
        {
            controller.Spawner.enabled = false;

            if (shieldGone)
            {
                controller.dreadnovaShield.SetActive(false);

                if (controller.State == DreadnovaState.SHIELD_STAGE)
                    controller.PerformTransition(Transition.NoShield);
                else
                    controller.PerformTransition(Transition.Attack);
            }
            else
            {
                shieldDissolve = Mathf.Max(shieldDissolve - 0.2f * Time.deltaTime, 0);

                if (shieldDissolve == 0)
                    shieldGone = true;
            }
        }
        else if (GameManager.Instance.GameState == GameState.BATTLE)
        {
            if (AreaManager.Instance.EnemyCount <= controller.Spawner.Wave.GetMaxEnemyCount(waveCount) * 0.30f)
            {
                waveTime += Time.deltaTime;

                if (waveTime >= controller.Spawner.Wave.TimeBetweenWaves)
                {
                    waveTime = 0;
                    waveCount = (waveCount + 1) % controller.Spawner.Wave.Waves.Length;

                    controller.Spawner.StartWave(waveCount, false);
                }
            }
        }
    }

    public override void Act()
    {
        foreach (Transform child in controller.dreadnovaShield.transform)
        {
            if (child.TryGetComponent(out MeshRenderer renderer))
            {
                renderer.material.SetFloat("_Dissolve", shieldDissolve);
            }
        }
    }

    private bool IsGeneratorsDead()
    {
        bool dead = true;

        foreach (ShieldGenerator generator in controller.Generators)
        {
            if (!generator.IsDead)
            {
                dead = false;
                break;
            }
        }

        return dead;
    }
}
