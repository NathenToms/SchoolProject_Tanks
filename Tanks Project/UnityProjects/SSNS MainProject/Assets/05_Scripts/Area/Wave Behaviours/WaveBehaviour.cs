using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave Behaviour", menuName = "Spawner/Wave Behaviour")]
public class WaveBehaviour : ScriptableObject
{
    [SerializeField] private float timeBetweenWaves = 30;
    [SerializeField] private float timeBetweenEnemySpawns = 0.4f;
    [SerializeField] private float timeBetweenCargoSpawns = 40;

    [SerializeField] private WaveSpawn[] waves;

    public int GetMaxEnemyCount(int waveIndex)
    {
        int count = 0;

        foreach(SpawnInfo spawn in waves[waveIndex].Spawns)
        {
            count += spawn.count;
        }

        return count;
    }

    public float TimeBetweenWaves { get { return timeBetweenWaves; } }
    public float TimeBetweenEnemySpawns { get { return timeBetweenEnemySpawns; } }
    public float TimeBetweenCargoSpawns { get { return timeBetweenCargoSpawns; } }
    public WaveSpawn[] Waves { get { return waves; } }
}
