using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveShaderPoint : MonoBehaviour
{
	GameObject player;
	Material material;

	// Start is called before the first frame update
	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		material = GetComponent<MeshRenderer>().material;
	}

	// Update is called once per frame
	void Update()
	{
		if (player)
		{
			material.SetVector("_Position", player.transform.position);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, transform.localScale.x / 2);
	}
}
