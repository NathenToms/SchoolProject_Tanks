using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldGeneratorGeoSphere : MonoBehaviour
{
	private ShieldGenerator _shieldGenerator;

	private void Awake()
	{
		_shieldGenerator = GetComponentInParent<ShieldGenerator>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (_shieldGenerator.PlatesAlive) {
			_shieldGenerator.WeakPointHit(collision.gameObject);
		}
	}
}
