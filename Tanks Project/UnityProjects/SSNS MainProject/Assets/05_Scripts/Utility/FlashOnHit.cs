using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class FlashOnHit : MonoBehaviour
{
	private Material _material;
	private Color _color;

	[ColorUsage(true, true)]
	public Color Color;

	private void Awake()
	{
		_material = GetComponent<MeshRenderer>().material;
		_color = _material.GetColor("_Color");
	}

	private void OnCollisionEnter(Collision collision)
	{
		Flash();
	}

	public void Flash()
	{
		if (gameObject.activeSelf)
		{
			StopCoroutine(coFlash());
			StartCoroutine(coFlash());
		}
	}

	private IEnumerator coFlash()
	{
		_material.SetColor("_Color", Color);
		yield return null;
		yield return null;
		_material.SetColor("_Color", _color);
	}
}