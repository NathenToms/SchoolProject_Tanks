using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Stats", menuName = "Ship/Enemy Stats")]
public class EnemyStats : Stats
{
    [Header("Movement")]
    public float attackShipSpeed = 50f;

    public float attackRotationSpeed = 2f;

    [Header("Projectiles")]
    public float accuracy;
}
