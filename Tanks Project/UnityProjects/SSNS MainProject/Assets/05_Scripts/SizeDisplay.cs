using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeDisplay : MonoBehaviour
{
	public int size;

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, size);
	}
}
