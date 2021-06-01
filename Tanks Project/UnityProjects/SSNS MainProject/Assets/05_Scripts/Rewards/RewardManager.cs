using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
	/// <summary> A list of all possible rewards in the game </summary>
	[SerializeField] private List<Reward> rewards = new List<Reward>();

	/// <summary> The Reward manager singleton </summary>
	public static RewardManager Instance;

    public RewardUI rewardUI;

	// Start is called before the first frame update
	void Awake()
	{
		if (Instance != null) {
			Destroy(Instance.gameObject);
		}

		Instance = this;
	}

	/// <summary>
	/// Get a random reward from the reward manageer
	/// </summary>
	/// <param name="removeFromList"> Do we want to remove the reward from the reward list </param>
	/// <returns>The new reward</returns>
	public Reward GetReward(bool removeFromList = true)
	{
		// Get the index for the random reward
		int index = Random.Range(0, rewards.Count);

		if (rewards.Count > 0) {

			// Get the new reward for a random position in the rewards list
			Reward rewardToReturn = rewards[index];

			// Remove it from the list
			if (removeFromList)
			{
				rewards.RemoveAt(index);
			}

			return rewardToReturn;
		}

		return null;	
	}
}
