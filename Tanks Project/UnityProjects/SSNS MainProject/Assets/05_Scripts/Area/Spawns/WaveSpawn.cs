using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

[System.Serializable]
[CreateAssetMenu(fileName = "Wave Spawn", menuName = "Spawner/Wave Spawns")]
public class WaveSpawn : ScriptableObject
{
    [Tooltip("Set the amount of enemies to spawn with a certain type. Swarmer is the actual amount in the flock.")]
    [SerializeField] private SpawnInfo[] spawns;

    public SpawnInfo[] Spawns { get { return spawns; } }
}

[System.Serializable]
public struct SpawnInfo
{
    public EnemyType type;
    public int count;
}
