using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ship Behaviour", menuName = "Ship/New Ship Behaviour")]
public class ShipBehaviour : ScriptableObject
{
	[Header("Description")]

	// Ship description
	public string shipName;
	public string shipDescription;

	[Header("Rotation")]

	// Rotation
	public float xRot = 0;
	public float yRot = 0;
	public float zRot = 0;

	public float RotSpeed = 0.1f;

	[Header("Translation")]

	// Translation
	public float moveScale = 0f;
	public float moveSpeed = 0f;

	[Header("Camera")]

	// Camera
	public Vector3 normalPos;
	public Vector3 boostPos;
	public Vector3 sonicPos;
	public Vector3 warpPos;
	public Vector3 deathPos;

	public float cameraSpeed = 0.01f;
}
