using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
	[SerializeField] private Text text;

	float time;

	// Update is called once per frame
	void Update()
	{
		time += Time.deltaTime;

		System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(time);

		text.text = timeSpan.ToString("mm\\:ss");

	}
}
