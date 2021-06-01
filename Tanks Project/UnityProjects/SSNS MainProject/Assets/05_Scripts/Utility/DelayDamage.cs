using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayDamage : MonoBehaviour
{
    [SerializeField] private float delayTime = 1.0f;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        if (TryGetComponent(out HealthAndShields health))
        {
            health.Invincible = true;

            yield return new WaitForSeconds(delayTime);

            health.Invincible = false;
        }
        else
        {
            yield break;
        }
    }
}
