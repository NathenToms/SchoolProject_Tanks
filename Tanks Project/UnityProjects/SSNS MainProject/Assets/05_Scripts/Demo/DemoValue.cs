using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoValue : MonoBehaviour
{
	Vector3 value = Vector3.zero;

	public Vector3 GetValue { get { return value; } }

	public float GetX { get { return value.x; } }
	public float GetY { get { return value.y; } }
	public float GetZ { get { return value.z; } }

	public void SetX(string x) { value.x = float.Parse(x); }
	public void SetY(string y) { value.y = float.Parse(y); }
	public void SetZ(string z) { value.z = float.Parse(z); }
}
