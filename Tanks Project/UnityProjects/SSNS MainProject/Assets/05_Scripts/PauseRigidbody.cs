using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PauseRigidbody : MonoBehaviour
{
	Rigidbody _rigidbody;

	Vector3 _cachedVelocity = Vector3.zero;
	Quaternion _cachedRotation = Quaternion.identity;

	// Start is called before the first frame update
	void Awake()
    {
		_rigidbody = GetComponent<Rigidbody>();
	}

	public void Pause()
	{
		_cachedVelocity = _rigidbody.velocity;
		_cachedRotation = _rigidbody.rotation;
		_rigidbody.Sleep();
	}

	public void UnPause()
	{
		_rigidbody.WakeUp();
		_rigidbody.velocity = _cachedVelocity;
		_rigidbody.rotation = _cachedRotation;
	}
}
