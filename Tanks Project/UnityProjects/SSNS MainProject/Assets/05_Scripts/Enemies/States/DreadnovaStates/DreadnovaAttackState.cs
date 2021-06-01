using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

public class DreadnovaAttackState : FSMState
{
    private DreadnovaController controller;

    private bool shieldGone;

    private float shieldDissolve;

    private float waveTime;
    private int waveCount;
    private float cargoTime;

    public DreadnovaAttackState(DreadnovaController enemyController)
    {
        controller = enemyController;
        stateID = FSMStateID.Attacking;
    }

    public override void EnterStateInit()
    {
        shieldGone = false;
        shieldDissolve = 1;
        controller.dreadnovaShield.SetActive(false);
        controller.Spawner.enabled = true;

        waveTime = 0;
        waveCount = 0;
        cargoTime = 0;
    }

    public override void Reason()
    {
        // TODO: When dreadnova parts are implemented needs to determine when dreadnova is dead
        if (GameManager.Instance.GameState == GameState.BATTLE)
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

            if (!controller.Spawner.CargoExists)
            {
                cargoTime += Time.deltaTime;

                if (cargoTime >= controller.Spawner.Wave.TimeBetweenCargoSpawns)
                {
                    cargoTime = 0;

                    controller.Spawner.SpawnCargo();
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
}
