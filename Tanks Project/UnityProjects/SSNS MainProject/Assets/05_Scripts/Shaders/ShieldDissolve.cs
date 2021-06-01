using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldDissolve : MonoBehaviour
{
	float percent = 0;
	MeshRenderer renderer;

	private void Start()
	{
		renderer = GetComponent<MeshRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
		percent = Mathf.Clamp(percent += Time.deltaTime, 0, 1);
		renderer.material.SetFloat("_Percent", percent);
	}
}
