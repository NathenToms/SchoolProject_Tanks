using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CruiserTargetArea : MonoBehaviour
{
	[SerializeField] private List<GameObject> _obstacles = new List<GameObject>();

	public GameObject GetObstacle()
	{
		if (_obstacles.Count == 0)
		{
			return null;
		}
		else
		{
			GameObject newObstacle = _obstacles[0];
			_obstacles.Remove(newObstacle);
			return newObstacle;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
			if (_obstacles.Contains(other.gameObject) == false) {
				_obstacles.Add(other.gameObject);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		_obstacles.Remove(other.gameObject);
	}
}
