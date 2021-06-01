using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ShieldGeneratorPlate : MonoBehaviour
{
	private HealthAndShields _healthAndShields;
	private Material _material;

	public bool Destroyed = false;

	private Color _emissiveColor;
	private Color _baseColor;

	private void Awake()
	{
		_healthAndShields = GetComponentInChildren<HealthAndShields>();
		
		_material = GetComponent<MeshRenderer>().materials[1];

		_emissiveColor = _material.GetColor("_EmissiveColor");
		_baseColor = _material.GetColor("_BaseColor");
	}

	private void Start()
	{
		_healthAndShields.onDeath += DestroyPlate;
		Destroyed = false;
	}

	public void DestroyPlate()
	{
		_material.SetColor("_EmissiveColor", Color.black);
		_material.SetColor("_BaseColor", Color.black);
		Destroyed = true;
	}

	public void RepairPlate()
	{
		_material.SetColor("_EmissiveColor", _emissiveColor);
		_material.SetColor("_BaseColor", _baseColor);
		Destroyed = false;
	}
}
