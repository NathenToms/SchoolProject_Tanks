using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

public class DreadnovaSpawner : AreaSpawner
{
    [SerializeField] private WaveBehaviour shieldWaveBehaviour;
    [SerializeField] private WaveBehaviour attackWaveBehaviour;

    [SerializeField] private float distanceToTravelAway = 200;

    [Header("Max Enemy Counts")]
    [SerializeField] private int fighterMaxCount = 10;
    [SerializeField] private int chargerMaxCount = 10;
    [SerializeField] private int swarmerMaxCount = 200;

    private CargoController cargoController;

    // Start is called before the first frame update
    void Start()
    {
        waveBehaviour = shieldWaveBehaviour;

        Transform dreadnova = transform.Find("Dreadnova");

        if (TryGetComponent(out DreadnovaController controller))
        {
            controller.StateChanged += DreadnovaStateChange;
        }
    }

    private void Update()
    {
        // Leave empty
    }

    private void DreadnovaStateChange(FSMStateID stateID)
    {
        if (stateID == FSMStateID.Defend)
        {
            waveBehaviour = shieldWaveBehaviour;
        }
        else if (stateID == FSMStateID.Attacking)
        {
            waveBehaviour = attackWaveBehaviour;
        }

        if (!cargoController) SpawnCargo();
    }

    public void SpawnCargo()
    {
        Transform spawnpoint = AreaManager.Instance.CurrentArea.FindSafeSpawn("CargoSpawn");

        Vector3 swarmerSpawnpoint = spawnpoint.position + Random.insideUnitSphere * 20;

        GameObject cargo = SpawnEnemy(cargoPrefab, EnemyType.CARGO, spawnpoint.position, spawnpoint.position);
        GameObject swarmer = SpawnEnemy(swarmerPrefab, EnemyType.SWARMER, swarmerSpawnpoint, swarmerSpawnpoint);

        cargo.TryGetComponent(out cargoController);

        if (swarmer.TryGetComponent(out Flock flock))
        {
            flock.defenseTarget = cargo;
            flock.startingCount = 75;

            //flock.FlockLeader.gameObject.transform.position = swarmerSpawnpoint;
        }

        DialogueSystem.Instance.AddDialogue(2);
    }

    protected override GameObject SpawnEnemy(GameObject prefab, EnemyType type, Transform spawnpoint)
    {
        switch (type)
        {
            case EnemyType.FIGHTER:
                if (fighterCount >= fighterMaxCount) return null;
                break;
            case EnemyType.CHARGER:
                if (chargerCount >= chargerMaxCount) return null;
                break;
            case EnemyType.SWARMER:
                if (swarmerCount >= swarmerMaxCount) return null;
                break;
        }

        GameObject enemy = SpawnEnemy(prefab, type, spawnpoint.position, spawnpoint.position + spawnpoint.forward * distanceToTravelAway);

        enemy.transform.rotation = spawnpoint.rotation;

        return enemy;
    }

    protected override void DrawGizmos()
    {
        base.DrawGizmos();

        if (spawnpoints != null)
        {
            foreach (Transform sp in spawnpoints)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(sp.position, sp.forward * distanceToTravelAway);
            }
        }
    }

    public WaveBehaviour Wave { get { return waveBehaviour; } }
    public bool CargoExists { get { return cargoController; } }
}