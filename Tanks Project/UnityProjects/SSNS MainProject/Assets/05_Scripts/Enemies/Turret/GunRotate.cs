using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRotate : InterceptCalculationClass
{
    public bool debug = false;

    public GameObject bulletSpawnPos;
    public GameObject bulletPrefab;
    [SerializeField] Axis axes;
    public float threatDistance = 200f;
    public float rotateSpeed = 0.1f;

    public float calculateInterval = 0.2f;

    private int[] axis = new int[3];
    GameObject target;
    GameObject player;

    Rigidbody rbTarget;
    Vector3 interceptPoint;

    public float accuracy = 4f;
    public float shotInterval = 1f;
    float shotTimer;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (axes.x) axis[0] = 1;
        if (axes.y) axis[1] = 1;
        if (axes.z) axis[2] = 1;

        StartCoroutine(CalculateIntercept());
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.paused) return;

        if (axes.x) axis[0] = 1;
        else axis[0] = 0;
        if (axes.y) axis[1] = 1;
        else axis[1] = 0;
        if (axes.z) axis[2] = 1;
        else axis[2] = 0;

        if (player == null) player = GameObject.FindGameObjectWithTag("Player");

        if (Vector3.Distance(transform.position, player.transform.position) < threatDistance)
        {
            Rotate();
            Shoot();
        }
        else
        {
            target = null;
        }
    }

    void Rotate()
    {
        target = player;
        rbTarget = target.GetComponent<Rigidbody>();

        Vector3 dir = interceptPoint - transform.position;
        Vector3 rot = Vector3.RotateTowards(transform.forward, dir, rotateSpeed * Time.deltaTime, 0.0f);
        Quaternion newRot = Quaternion.LookRotation(rot);
        rot.Normalize();        

        float dot = Vector3.Dot(rot, transform.parent.transform.right);

        if (dot > 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, newRot, rotateSpeed * Time.deltaTime);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x * axis[0], transform.localEulerAngles.y * axis[1], transform.localEulerAngles.z * axis[2]);
        }
    }
    void Shoot()
    {
        if (target.Equals(player))
        {
            shotTimer += Time.deltaTime;

            float distanceFromSight = Vector3.Cross(transform.forward, interceptPoint - transform.position).magnitude;

            if (distanceFromSight <= accuracy && shotTimer >= shotInterval)
            {
                Quaternion lookRot = Quaternion.LookRotation(transform.forward);
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPos.transform.position, lookRot);
                bullet.GetComponent<Bullet>().shooter = gameObject;

                shotTimer = 0;
            }
        }
    }


    IEnumerator CalculateIntercept()
    {
        while (true)
        {
            yield return new WaitForSeconds(calculateInterval);
            //positions
            if (target)
            {
                Vector3 targetPosition = target.transform.position;
                //velocities
                //Vector3 velocity = rbSelf ? rbSelf.velocity : Vector3.zero;
                Vector3 velocity = Vector3.zero;
                Vector3 targetVelocity = rbTarget ? rbTarget.velocity : Vector3.zero;

                //calculate intercept
                interceptPoint = FirstOrderIntercept(transform.position, velocity, bulletPrefab.GetComponent<ShotInfo>().Speed, targetPosition, targetVelocity);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Debug.DrawRay(transform.position, transform.forward.normalized * threatDistance, Color.red);
        }
    }
}
