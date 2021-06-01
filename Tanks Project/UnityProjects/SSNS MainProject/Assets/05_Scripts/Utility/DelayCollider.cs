using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DelayCollider : MonoBehaviour
{
	Collider collider;

    // Start is called before the first frame update
    void Start()
    {
		collider = GetComponent<Collider>();
		collider.enabled = false;
		StartCoroutine(Delay());
	}

	IEnumerator Delay()
	{
		yield return new WaitForSeconds(0.1f);
		collider.enabled = true;
	}
}
