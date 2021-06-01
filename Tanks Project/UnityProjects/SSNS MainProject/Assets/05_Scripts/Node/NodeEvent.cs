using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

/// <summary>
/// ScriptableObject that holds information for generating areas
/// </summary>
[CreateAssetMenu(fileName = "Node Event", menuName = "Nodes/New Event")]
public class NodeEvent : ScriptableObject
{
    [Tooltip("These prefabs are spawned and should include their own scripts for spawning stuff that reference AreaManager")]
    public GameObject[] prefabsToSpawn;

    [Tooltip("The size of the area.")]
    [Range(1000, 5000)]
    public int areaSize = 1000;

    [Tooltip("The amount of smoke to spawn")]
    public int smokeAmount = 200;

    [Tooltip("The amount of asteroids to spawn")]
    public int asteroidAmount = 100;
}
