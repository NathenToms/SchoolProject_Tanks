using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPickup : MonoBehaviour
{
    [Range(0,100)]
    [SerializeField] float percentile = 10f;
    [SerializeField] Pickup[] pickups;

	private void Start()
	{
		if (TryGetComponent<HealthAndShields>(out HealthAndShields health))
		{
			health.onDeath += AttemptPickupSpawn;
		}
	}

	public void AttemptPickupSpawn()
    {
        if (Random.Range(1, 101) < percentile)
        {
            int rand = Random.Range(0, pickups.Length);

            AreaManager.Instance.OnObjectAdd(Instantiate(pickups[rand].gameObject, transform.position, Quaternion.identity));
        }
    }
}
