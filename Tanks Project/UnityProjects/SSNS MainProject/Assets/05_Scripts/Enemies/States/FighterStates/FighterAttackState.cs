using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterAttackState : AttackState<FighterController>
{
    float shotTimer = 0.0f;
    float intervalTime = 0.0f;
    float calculateInterval = 0.3f;
    GameObject bulletPrefab;
    GameObject bulletSpawnPos;
    Rigidbody rbPlayer;

    public FighterAttackState(FighterController enemyController, GameObject bullet, GameObject bulletShootPos) : base(enemyController)
    {
        bulletPrefab = bullet;
        bulletSpawnPos = bulletShootPos;

        rbPlayer = controller.Player.GetComponent<Rigidbody>();
    }

    public override void Act()
	{
		Move();
        Shoot();
    }

    public override void Reason()
	{
		if (controller.Player == null)
        {
            AIManager.aiManager.StopAttack(controller.aiType);
            controller.PerformTransition(Transition.Patrol);
        }
        else if (AreaManager.Instance.CurrentArea.IsTransformOutside(controller.transform))
        {
            AIManager.aiManager.StopAttack(controller.aiType);
            controller.PerformTransition(Transition.Patrol);
        }
        else
        {

            CalculateIntercept();

            if (Vector3.Distance(controller.Player.transform.position, controller.transform.position) < controller.AttackDistance)
            {
                AIManager.aiManager.StopAttack(controller.aiType);
                controller.PerformTransition(Transition.Patrol);
            }
        }

        //Else dead transition to dead
        if (controller.Health.IsDead)
        {
            controller.PerformTransition(Transition.NoHealth);
        }
    }

    public override void EnterStateInit()
    {
    }

    //Moves
    protected override void Move()
    {
        if (controller.Player != null)
        {
            //Calculate direction
            Vector3 direction = controller.transform.forward; // sets forward
            direction.Normalize();

            if (controller.AvoidObstacles(ref direction)) // will change direction towards the right if an obstacle is in the way
            {
                obstacleHit = true;
            }

            //rotation
            if (!obstacleHit && obstacleTimer == 0)
            {
                direction = interceptPoint - controller.transform.position; // sets desired direction to target intercept point
            }
            else
            {
                //if obstacles, ignore desired direction and move to the right of obstacles
                obstacleTimer += Time.deltaTime;
                if (obstacleTimer > avoidTime)
                {
                    obstacleTimer = 0;
                    obstacleHit = false;
                }
            }

            Vector3 newDir = Vector3.RotateTowards(controller.transform.forward, direction, controller.Stats.rotationSpeed * Time.deltaTime, 0);
            Quaternion rot = Quaternion.LookRotation(newDir);
            controller.transform.rotation = Quaternion.Lerp(controller.transform.rotation, rot, controller.Stats.rotationSpeed * Time.deltaTime);

            //Move
            controller.Rigid.AddForce(controller.transform.forward.normalized * controller.Stats.shipSpeed , ForceMode.Acceleration);
        }
    }

    void Shoot()
    {
        if (!obstacleHit && controller.PlayerInVision())
        {
            shotTimer += Time.deltaTime;

            float distanceFromSight = Vector3.Cross(controller.transform.forward, interceptPoint - controller.transform.position).magnitude;

            if (distanceFromSight <= controller.Stats.accuracy && shotTimer >= controller.Stats.fireRate)
            {
                Quaternion lookRot = Quaternion.LookRotation(controller.transform.forward);
                GameObject bullet = Object.Instantiate(bulletPrefab, bulletSpawnPos.transform.position, lookRot);
                bullet.GetComponent<Bullet>().shooter = controller.gameObject;

                controller.GetComponent<ShootingSoundController>().PlayShot(SNSSTypes.WeaponType.Energy);

                shotTimer = 0;
            }
        }
    }

    //Calculates the intercept point
    protected override void CalculateIntercept()
    {
        intervalTime += Time.deltaTime;
        //while (true)
        if (intervalTime >= calculateInterval)
        {
            intervalTime = 0.0f;
            //yield return new WaitForSeconds(calculateInterval);
            //positions
            Vector3 targetPosition = controller.Player.transform.position;
            //velocities
            Vector3 velocity = controller.Rigid ? controller.Rigid.velocity : Vector3.zero;
            //Vector3 velocity = Vector3.zero;
            Vector3 targetVelocity = rbPlayer ? rbPlayer.velocity : Vector3.zero;

            //calculate intercept
            interceptPoint = InterceptCalculationClassNoMono.FirstOrderIntercept(controller.transform.position, velocity, bulletPrefab.GetComponent<ShotInfo>().Speed, targetPosition, targetVelocity);
        }
    }
}
