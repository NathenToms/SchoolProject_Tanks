using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthAndShields : MonoBehaviour
{
    /// <summary> When this GameObjects life or shields change values </summary>
    public delegate void OnLifeChange(float current, float max);
    public OnLifeChange onLifeChange;

    /// <summary> When this GameObjects life or shields change values </summary>
    public delegate void OnShieldChange(float current, float max);
    public OnShieldChange onShieldChange;

    /// <summary> When this GameObject dies </summary>
    public delegate void OnDeath();
    public OnDeath onDeath;
	private bool killed = false;

    [SerializeField] private Stats shipStats;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] float explosionScale = 1f;
    private GameObject destroyedObject;

    // The MAX life the ship has
    public float MaxLife { get { return shipStats.maxHealth; } }

    // The MAX shield the ship has
    public float MaxShield { get { return shipStats.maxShield; } }

    public float shieldRegenDelay = 3f;


    // Can we regenerate our shield
    public bool CanRegen { get { return Time.realtimeSinceStartup > shieldRegenDelay; } }

    public float currentLife;
    public float currentShield;

    public bool Invincible = false;
    public bool DestroyOnDeath = true;
    public bool KeepOnDeath = false;



    // Start is called before the first frame update
    void Start()
    {
        if (shipStats == null)
        {
            if (TryGetComponent(out ShipController player))
            {
                shipStats = player.Properties;
            }
            else if (TryGetComponent(out EnemyController enemy))
            {
                shipStats = enemy.Stats;
            }
            else if (TryGetComponent(out FlockAgent flockAgent))
            {
                shipStats = flockAgent.swarm.FlockLeader.Stats;
            }
            else
            {
                Debug.LogError("Couldn't get ship stats");
            }
        }

        currentLife = MaxLife;
        currentShield = MaxShield;

        ShieldProjector shieldProjector = gameObject.GetComponentInChildren<ShieldProjector>();

        if (shieldProjector)
        {
            // So we can take damage when this shield is hit
            shieldProjector.onShieldHit += OnShieldHit;

            // So we can update the color of the shield when we take damage
            onShieldChange += shieldProjector.UpdateShieldPercent;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If we have more then 0 life we can regen shields
        if (CanRegen && currentLife >= 0)
        {
            // Calculating the amount we need to heal WITH regen Speed
            float amountToHeal = currentShield + (MaxShield * shipStats.shieldRegenPercent / 100f) * Time.deltaTime;

            // Clamp out shield to the max shield
            currentShield = Mathf.Clamp(amountToHeal, 0, MaxShield);
        }
    }

    /// <summary>
    /// Reinitializes this scripts values
    /// </summary>
    /// <param name="invokeEvents"> Do you want to call the OnLifeChange events </param>
    public void ResetValues(bool invokeEvents = false)
    {
        currentLife = MaxLife;
        currentShield = MaxShield;
        killed = false;

        gameObject.SetActive(true);

        if (invokeEvents == true)
        {
            // Invoke the On Life Change Event
            onLifeChange?.Invoke(currentLife, MaxLife);

            // Invoke the On Shield Change Event
            onShieldChange?.Invoke(currentShield, MaxShield);
        }
    }

    /// <summary> When the shield it hit </summary>
    public void OnShieldHit(GameObject attacker)
    {
        if (attacker.TryGetComponent(out Damage damage))
        {
            // PrintDamage(damage.kineticDamage, damage.energyDamage);
            TakeDamage(damage.KineticDamage, damage.EnergyDamage);
        }
    }

    // Damage the ship
    public void TakeDamage(float kineticDamage, float energyDamage)
    {
        if (!Invincible)
        {
            shieldRegenDelay = Time.realtimeSinceStartup + shipStats.shieldRegenInterval;

            // Damage the shield
            currentShield = Mathf.Clamp(currentShield - energyDamage, 0, MaxShield);

            // If we have negative shields we can take it away from your life pool
            if (currentShield == 0)
            {
                currentLife -= kineticDamage;
            }

            // If we are dead cann OnDeath()
            if (currentLife <= 0 && !killed)
            {
                currentLife = 0;
				killed = true;
                HandleDeath();
            }
        }

        // Invoke the On Life Change Event
        if (onLifeChange != null)
        {
            onLifeChange.Invoke(currentLife, MaxLife);
        }

        // Invoke the On Shield Change Event		
        if (onShieldChange != null)
        {
            onShieldChange.Invoke(currentShield, MaxShield);
        }
    }

    // When life is 0 this is called by TakeDamage or Update
    void HandleDeath()
    {
        if (gameObject.tag == "Player")
        {
            currentLife = MaxLife;
            currentShield = MaxShield;
        }

        // Invoke the On Death Event
        if (onDeath != null)
        {
            onDeath.Invoke();
        }

        if (DestroyOnDeath == true)
        {
            // Destroy the gameobject
            Destroy(gameObject);
        }
        else if(!KeepOnDeath)
        {
            // Respawn the gameobject?
            gameObject.SetActive(false);
        }

        if (explosionPrefab)
        {
            destroyedObject = Instantiate(explosionPrefab);
            destroyedObject.transform.position = transform.position;
            destroyedObject.transform.rotation = transform.rotation;
            destroyedObject.transform.localScale = new Vector3(explosionScale, explosionScale, explosionScale);

            AreaManager.Instance.OnObjectAdd(destroyedObject);
        }
    }

    // Heal this GameObjects current life by a given value
    public void Heal(int amountToHeal)
    {
        currentLife += amountToHeal;

        // Clamp that value
        if (currentLife > MaxLife) currentLife = MaxLife;

        // Invoke the On Life Change Event
        onLifeChange.Invoke(currentLife, MaxLife);
    }

    /// <summary> Print out the damage taken </summary>
    public void PrintDamage(float kineticDamage, float energyDamag)
    {
        Debug.Log("Damage Taken:( KineticDamage: " + kineticDamage + ", EnergyDamage: " + energyDamag + ")");
    }

    public bool IsDead { get { return currentLife <= 0; } }
}
