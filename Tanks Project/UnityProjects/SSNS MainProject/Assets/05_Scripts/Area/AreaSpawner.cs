using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

/// <summary>
/// TODO: Make it spawn randomly
/// </summary>
public class AreaSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] protected GameObject fighterPrefab;
    [SerializeField] protected GameObject chargerPrefab;
    [SerializeField] protected GameObject swarmerPrefab;
    [SerializeField] protected GameObject cruiserPrefab;
    [SerializeField] protected GameObject cargoPrefab;

    [SerializeField] protected WaveBehaviour waveBehaviour;

    [SerializeField] private Area area;

    [SerializeField] protected Transform[] spawnpoints;
    [SerializeField] protected Transform[] waypoints;

    protected int fighterCount = 0;
    protected int chargerCount = 0;
    protected int swarmerCount = 0;
    protected int cruiserCount = 0;
    protected int cargoCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (area == null)
            TryGetComponent(out area);

        StartCoroutine(CreateArea());
    }

    private void Update()
    {
        if (GameManager.Instance.GameState == GameState.BATTLE)
        {
            if (AreaManager.Instance.EnemiesDead)
            {
                GameManager.Instance.SwitchState(GameState.BATTLE_END);
                DialogueSystem.Instance.AddDialogue(7);
            }
        }
    }

    private IEnumerator CreateArea()
    {
        spawnpoints = null;
        waypoints = null;

        if (area && area.spawns && area.spawns.childCount > 0)
        {
            spawnpoints = area.Spawns;
        }
        else
        {
            GenerateSpawnpoints();
        }

        if (area && area.waypoints && area.waypoints.childCount > 0)
        {
            waypoints = area.Waypoints;
        }
        else
        {
            GenerateWaypoints();
        }

        yield return StartCoroutine(SpawnWave(0));
    }

    protected void GenerateSpawnpoints()
    {
        List<Transform> sp = new List<Transform>();

        for (int i = 0; i < waveBehaviour.GetMaxEnemyCount(0); i++)
        {
            Transform spawnpoint = new GameObject("Spawnpoint").transform;
            spawnpoint.parent = area.spawns;

            Vector3 position = AreaManager.Instance.FindRandomPosition;

            if (!Physics.CheckSphere(position, 200, 1 << 8))
            {
                spawnpoint.position = position;

                sp.Add(spawnpoint);
            }
        }

        spawnpoints = sp.ToArray();
    }

    protected void GenerateWaypoints()
    {
        List<Transform> wp = new List<Transform>();

        for (int i = 0; i < Random.Range(5, 15); i++)
        {
            Transform waypoint = new GameObject("Waypoint").transform;
            waypoint.parent = area.waypoints;

            Vector3 position = AreaManager.Instance.FindRandomPosition;

            if (!Physics.CheckSphere(position, 200, 1 << 8))
            {
                waypoint.position = position;

                wp.Add(waypoint);
            }
        }

        waypoints = wp.ToArray();
    }

    protected GameObject SpawnEnemy(GameObject prefab, EnemyType type, Vector3 spawnpoint, Vector3 destination)
    {
        GameObject enemy = Instantiate(prefab);

        AreaManager.Instance.OnObjectAdd(enemy, true);

        enemy.transform.position = spawnpoint;

        if (enemy.TryGetComponent(out EnemyController enemyController))
        {
            enemyController.Spawn = spawnpoint;
            enemyController.SpawnDestination = destination;
            enemyController.waypoints = waypoints;
            enemyController.Health.onDeath += () =>
            {
                switch (type)
                {
                    case EnemyType.FIGHTER:
                        fighterCount--;
                        break;
                    case EnemyType.CHARGER:
                        chargerCount--;
                        break;
                    case EnemyType.CRUISER:
                        cruiserCount--;
                        break;
                    case EnemyType.CARGO:
                        cargoCount--;
                        break;
                }
            };
        }
        else if (enemy.TryGetComponent(out Flock flock))
        {
            flock.SetSpawnDestination = spawnpoint;
            flock.SetSpawnDestination = destination;
            flock.WayPoints = waypoints;
            
            foreach(FlockAgent agent in flock.agents)
            {
                if (agent.TryGetComponent(out HealthAndShields health))
                {
                    health.onDeath += () =>
                    {
                        swarmerCount--;
                    };
                }
            }
        }

        return enemy;
    }

    protected virtual GameObject SpawnEnemy(GameObject prefab, EnemyType type, Transform spawnpoint)
    {
        GameObject enemy = SpawnEnemy(prefab, type, spawnpoint.position, spawnpoint.position);

        return enemy;
    }

    protected virtual IEnumerator SpawnWave(int waveIndex, bool safeSpawn = true)
    {
        WaveSpawn spawnBehaviour = waveBehaviour.Waves[waveIndex];

        int spawnIndex = 0;

        for (int i = 0; i < spawnBehaviour.Spawns.Length; i++)
        {
            SpawnInfo spawnInfo = spawnBehaviour.Spawns[i];

            if (spawnInfo.type == EnemyType.SWARMER)
            {
                Transform spawnpoint = safeSpawn ? area.FindSafeSpawn() : spawnpoints[spawnIndex];

                GameObject swarmer = SpawnEnemy(swarmerPrefab, spawnInfo.type, spawnpoint);

                if (swarmer == null) continue;

                if (swarmer.TryGetComponent(out Flock flock))
                {
                    flock.startingCount = spawnInfo.count;
                    swarmerCount+= flock.startingCount;
                }

                if (!safeSpawn)
                    spawnIndex = (spawnIndex + 1) % spawnpoints.Length;
            }
            else
            {
                for (int j = 0; j < spawnInfo.count; j++)
                {
                    Transform spawnpoint = safeSpawn ? area.FindSafeSpawn() : spawnpoints[spawnIndex];

                    switch (spawnInfo.type)
                    {
                        case EnemyType.FIGHTER:
                            SpawnEnemy(fighterPrefab, spawnInfo.type, spawnpoint);
                            fighterCount++;
                            break;
                        case EnemyType.CHARGER:
                            SpawnEnemy(chargerPrefab, spawnInfo.type, spawnpoint);
                            chargerCount++;
                            break;
                        case EnemyType.CRUISER:
                            SpawnEnemy(cruiserPrefab, spawnInfo.type, spawnpoint);
                            cruiserCount++;
                            break;
                        case EnemyType.CARGO:
                            SpawnEnemy(cargoPrefab, spawnInfo.type, spawnpoint);
                            cargoCount++;
                            break;
                    }

                    if (!safeSpawn)
                        spawnIndex = (spawnIndex + 1) % spawnpoints.Length;

                    yield return new WaitForSeconds(waveBehaviour.TimeBetweenEnemySpawns);
                }
            }
        }
    }

    public void StartWave(int waveIndex, bool safeSpawn = true)
    {
        StartCoroutine(SpawnWave(waveIndex, safeSpawn));
    }

    protected virtual void DrawGizmos()
    {
        if (spawnpoints != null)
        {
            foreach (Transform sp in spawnpoints)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(sp.position, 4.0f);
            }
        }

        if (waypoints != null)
        {
            Gizmos.color = Color.blue;

            foreach (Transform wp in waypoints)
            {
                Gizmos.DrawWireSphere(wp.position, 4.0f);
            }
        }
    }

    private void OnDrawGizmos()
    {
        DrawGizmos();
    }
}
