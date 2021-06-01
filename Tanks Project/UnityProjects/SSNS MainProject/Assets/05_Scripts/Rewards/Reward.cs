using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Reward", menuName = "Rewards/New Reward")]
public class Reward : ScriptableObject
{
	public string rewardName = "";
	public string rewardDescription = "";

	// the value of the reward
	public float value = 0.0f;

	// The type of reward this is
	public RewardsType type = RewardsType.NoReward;

	/// <summary>
	/// Use this reward on a gameobject
	/// </summary>
	public void UseReward(PlayerStats target)
	{
		Debug.Log("You are applying the reward " + type.ToString() + " to the player!");

		if (type == RewardsType.NoReward)
		{
			return;
		}
		else
		{
			switch (type)
			{

				case RewardsType.ExtraHealth:
					target.maxHealth += value;
					break;

				case RewardsType.ExtraShield:
					target.maxShield += value;
					break;

				case RewardsType.ExtraDamage:
					target.baseDamage += value;
					break;

				case RewardsType.ShipAcceleration:
					target.shipAcceleration += value;
					break;

				case RewardsType.FiringSpeed:
					target.fireRate += value;
					break;

				case RewardsType.HealthDamageReduction:
					target.healthDamageReduction += value;
					break;

				case RewardsType.ShieldDamageReduction:
					target.shieldDamageReduction += value;
					break;

				case RewardsType.IncreasedRegenSpeed:
					target.shieldRegenPercent += value;
					break;

				case RewardsType.RegenDelayReduction:
					target.shieldRegenInterval -= value;
					break;

				case RewardsType.BoostGaugeIncrease:
					target.maxBoostGauge += value;
					break;

				case RewardsType.ReserveShield:
					target.reserveShield = true;
					break;

				case RewardsType.ShieldBreakEMP:
					target.shieldBreakEMP = true;
					break;
			}
		}
	}
}

// The different rewards you can have you can have
public enum RewardsType
{
	// The Default
	NoReward,

	// Flat increases
	ExtraHealth,
	ExtraShield,
	ExtraDamage,

	ShipAcceleration,
	BoostAcceleration,

	FiringSpeed,

	// Damage reduction
	HealthDamageReduction,
	ShieldDamageReduction,

	// Fast heals
	FullHealthRestore,

	// Shield Regen
	IncreasedRegenSpeed,
	RegenDelayReduction,

	// Boost gauge
	BoostGaugeIncrease,

	// Special effects
	ReserveShield,
	ShieldBreakEMP
}
