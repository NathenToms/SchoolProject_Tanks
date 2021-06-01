using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConnectionManager : MonoBehaviour
{
	public List<Player> players;

	public static PlayerConnectionManager Instance;

	// Start is called before the first frame update
	void Start()
    {
		if (Instance)
		{
			Destroy(Instance.gameObject);		
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void Join(Player newPlayer)
	{
		players.Add(newPlayer);
	}
}
