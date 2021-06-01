using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
	#region Static Methods

	public static void PrintTime(string s)
	{
		Debug.Log("[ " + (Time.realtimeSinceStartup).ToString("#####0.00") + "s ] " + s);
	}

	#endregion

	#region Enumeration Types

	public enum eMenuDirection { LEFT, RIGHT, UP, DOWN }

	#endregion
}
