using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that handles all reward UI
/// TODO: Might be changed to a message UI
/// </summary>
public class RewardUI : MonoBehaviour
{
    [SerializeField] private RewardMessageUI rewardUI;

    /// <summary>
    /// Shows the UI with reward info
    /// </summary>
    /// <param name="reward">The reward to display</param>
    public void UpdateUI(Reward reward)
    {
        if (reward == null) return;

        if (!Shown)
        {
            rewardUI.groupFade.FadeActive(true);

            rewardUI.rewardName.text = reward.rewardName;
        }
    }

    public bool Shown { get { return rewardUI.groupFade.Shown; } }
}

/// <summary>
/// Struct that stores all reward UI components
/// </summary>
[System.Serializable]
public struct RewardMessageUI
{
    public Fade groupFade;

    [Header("Reward UI")]
    public Text rewardName;
    public Text rewardStats;
}
