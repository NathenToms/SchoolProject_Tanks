using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupMenu : MonoBehaviour
{
	[SerializeField] private GameObject playerOne;
	[SerializeField] private GameObject playerTwo;

	List<Player> players;

	private void OnEnable()
	{
		players = PlayerConnectionManager.Instance.players;
	}

	private void Update()
	{
		if (players.Count == 0)
		{
			playerOne.SetActive(false);
			playerTwo.SetActive(false);
		}
		else if (players.Count == 1)
		{
			playerOne.SetActive(true);
			playerTwo.SetActive(false);
		}
		else if (players.Count == 2)
		{
			playerOne.SetActive(true);
			playerTwo.SetActive(true);
		}
	}
}
