using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(ShotInfo))]
public class ChargedShotBehaviour : MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab;
    [Range(1, 15)]
    [SerializeField] float maxChargeTime = 5f;
    
    //Damage values
    [SerializeField] float minDamage = 10f;
    [SerializeField] float maxDamage = 50f;
    private float currentDamage;
    private float increasePerSecond;

    //Scale values
    [SerializeField] float minScale = 0.5f;
    [SerializeField] float maxScale = 5f;
    private Vector3 scalePerSecond;

    private float speed;
    private bool hasShot = false;            

    public bool HasShot
    {
        get { return hasShot; }
        set
        {
            hasShot = value;
            if(value) transform.parent = null;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        currentDamage = minDamage;
        increasePerSecond = (maxDamage - minDamage) / maxChargeTime;
        speed = GetComponent<ShotInfo>().Speed;

        Vector3 scale = new Vector3(minScale, minScale, minScale);
        transform.localScale = scale;

        float scaleSecond = (maxScale - minScale) / maxChargeTime;
        scalePerSecond = new Vector3(scaleSecond, scaleSecond, scaleSecond);

        StartCoroutine(StartCharge());
    }

    // Update is called once per frame
    void Update()
    {
		if (!hasShot)
		{
			currentDamage = Mathf.Clamp(currentDamage + increasePerSecond * Time.deltaTime, minDamage, maxDamage);
			transform.localScale += scalePerSecond * Time.deltaTime;
			transform.localPosition = Vector3.zero;

		}
		else
		{
			transform.position += transform.forward.normalized * speed * Time.deltaTime;
			transform.localScale += Vector3.one * 10 * Time.deltaTime;
		}
	}

    IEnumerator StartCharge()
    {
        yield return new WaitForSeconds(maxChargeTime);

        transform.parent = null;
        hasShot = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out HealthAndShields health))
        {
            health.TakeDamage(currentDamage, currentDamage);
        }

        if (explosionPrefab)
        {
            GameObject go = Instantiate(explosionPrefab, transform.position, Quaternion.identity); //spawn the explosion
            go.transform.localScale = transform.localScale;
        }

        Destroy(gameObject);
    }

    public float GetDamagePercentage()
    {
        return currentDamage / maxDamage;
    }
}
