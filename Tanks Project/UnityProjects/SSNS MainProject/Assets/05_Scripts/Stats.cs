using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Health Stats", menuName = "Health/Stats")]
public class Stats : ScriptableObject
{
	// Ship description
	[SerializeField] private string shipName;
	[SerializeField] private string shipDescription;

	// Ship properties
	// The max values for this ships health and shield
	[Header("Health and Shields")]
	public float maxHealth = 100f;
	public float maxShield = 100f;

	// Shield regen properties
	[Range(0, 100)] public float shieldRegenPercent = 5;
	public float shieldRegenInterval = 5;

	[Header("Movement")]
	public float shipSpeed = 30f;

	public float rotationSpeed = 5f;

	[Header("Projectiles")]
	public float fireRate = 0.1f;

	public string ShipName { get { return shipName; } }
	public string ShipDescription { get { return shipDescription; } }
}
