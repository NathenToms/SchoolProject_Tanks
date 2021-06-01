using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ExplosionDamage))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ShotInfo))]
public class MissileBehaviour : MonoBehaviour
{
    [SerializeField] LayerMask enemyLayers;
    [SerializeField] float maxDetectionRadius = 400f;
    [SerializeField] float radiusIncreasePerSecond = 25f;
    [SerializeField] float rotationSpeed = 2f;

    public GameObject target;
    ShotInfo info;
    float currentRadius = 25f;
    float extraSpeed = 0;
    Rigidbody rb;

    private void Start()
    {
        info = GetComponent<ShotInfo>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        LookForTarget();
        Move();
    }

    /// <summary>
    /// Sets the target of the missile if no target is already set.
    /// </summary>
    /// <param name="newTarget"></param>
    public void SetInitialTarget(GameObject newTarget)
    {
        if (target == null) target = newTarget;
    }

    private void LookForTarget()
    {
        if (!target) //if no target
        {
            if (currentRadius < maxDetectionRadius) currentRadius += radiusIncreasePerSecond * Time.deltaTime; //Increase detection radius up until max

            Collider[] detected = Physics.OverlapSphere(transform.position, currentRadius, enemyLayers); //Ping for enemies on layer with colliders within the radius

            GameObject temp = null;
            float distance = Mathf.Infinity;

            //Look for closest enemy in range
            foreach (Collider c in detected)
            {
                float dist = Vector3.Distance(c.gameObject.transform.position, transform.position);
                if (dist < distance)
                {
                    distance = dist;
                    temp = c.gameObject;
                }
            }

            if (temp != null) target = temp; //Set to the closest enemy
        }
    }

    private void Move()
    {
        if (target)
        {
            Vector3 intercept = target.transform.position;

            //Try and get rigidbody. Then calculate an intercept point for the missiles
            if (target.TryGetComponent(out Rigidbody rigid))
            {
                intercept = InterceptCalculationClass.FirstOrderIntercept(transform.position, Vector3.zero, rb.velocity.magnitude, target.transform.position, rigid.velocity);
            }

            Vector3 newDir = intercept - transform.position;
            Vector3 newRot = Vector3.RotateTowards(transform.forward, newDir, rotationSpeed * Time.deltaTime, 0);

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(newRot), rotationSpeed * Time.deltaTime); //look towards target
        }

        rb.velocity = (transform.forward.normalized * (info.Speed + extraSpeed)); //using rigidbody for intercept calculations
		extraSpeed += Time.deltaTime * 5;
		transform.localScale += Time.deltaTime * Vector3.one * 1;

	}
}
