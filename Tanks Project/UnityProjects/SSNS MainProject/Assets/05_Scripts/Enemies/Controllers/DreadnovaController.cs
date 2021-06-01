using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

public class DreadnovaController : EnemyController
{
    [SerializeField] private ShieldGenerator[] shieldGenerators;
    public GameObject dreadnovaShield;
    public GameObject dreadnovaModel;
    public GameObject dreadnovaThrusters;
    [SerializeField] DreadnovaDistortionManager distort;
    [SerializeField] WarpEffectBehaviour warpEffect;
    [SerializeField] AudioSource warpSound;

    [SerializeField] private DreadnovaState dreadnovaState;

    private DreadnovaSpawner spawner;

    public bool warping;

    public override void ResetEnemy()
    {
        if (dreadnovaState == DreadnovaState.SHIELD_STAGE)
        {
            foreach (ShieldGenerator generator in shieldGenerators)
            {
                generator.InitializeGenerator();
            }

            dreadnovaShield.SetActive(true);
        }

        dreadnovaModel.SetActive(true);
        dreadnovaThrusters.SetActive(false);

        warping = false;

        spawner.enabled = true;

        base.ResetEnemy();
    }

    protected override void Initialize()
    {
        Spawn = transform.position;
        SpawnDestination = transform.position + transform.forward;

        dreadnovaThrusters.SetActive(true);

        TryGetComponent(out spawner);

        base.Initialize();
    }

    protected override void ConstructFSM()
    {
        DeadState dead = new DeadState(this);
        DreadnovaSpawnState spawn = new DreadnovaSpawnState(this);
        DreadnovaShieldState shield = new DreadnovaShieldState(this);
        DreadnovaEscapeState escape = new DreadnovaEscapeState(this);
        DreadnovaAttackState attack = new DreadnovaAttackState(this);

        dead.AddTransition(Transition.Reset, FSMStateID.Spawned);

        spawn.AddTransition(Transition.Defend, FSMStateID.Defend);
        spawn.AddTransition(Transition.Reset, FSMStateID.Spawned);
        spawn.AddTransition(Transition.Attack, FSMStateID.Attacking);

        //shield.AddTransition(Transition.Attack, FSMStateID.Attacking);
        shield.AddTransition(Transition.NoShield, FSMStateID.Running);
        shield.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        shield.AddTransition(Transition.Reset, FSMStateID.Spawned);

        escape.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        escape.AddTransition(Transition.Reset, FSMStateID.Spawned);

        attack.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        attack.AddTransition(Transition.Reset, FSMStateID.Spawned);

        AddFSMState(spawn);
        AddFSMState(shield);
        AddFSMState(escape);
        AddFSMState(attack);
        AddFSMState(dead);
    }

    public void WarpDreadnova()
    {
        StartCoroutine(WarpOut());
    }

    public void DestroyGenerators()
    {
        foreach(ShieldGenerator generator in shieldGenerators)
        {
            generator.DestroyGenerator();
        }
    }

    private IEnumerator WarpOut()
    {
        if (warping) yield break;

        warping = true;

        GameObject dreadnovaParent = new GameObject();
        dreadnovaParent.transform.parent = transform.parent;
        dreadnovaParent.transform.localPosition = new Vector3(0, 0, 2500);
        transform.parent = dreadnovaParent.transform;
        transform.localPosition = new Vector3(0, 0, -2500);

        // TODO: Warp effects
        dreadnovaThrusters.SetActive(true);

        //Start distortion

        int chargeTime = 10;

        distort.gameObject.SetActive(true);
        distort.StartDistortion(chargeTime);
        //warpEffect.StartWarp();

        Vector3 orScale = transform.localScale;
        Vector3 newScale = new Vector3(1, 1, 1.1f);

        float chargeScale = 0.05f;

        for (int i = 0; i < chargeTime / chargeScale; i++)
        {
            dreadnovaParent.transform.localScale = Vector3.Slerp(dreadnovaParent.transform.localScale, newScale, chargeScale * Time.deltaTime);

            if (i >= chargeTime / chargeScale) break;

            yield return null;
        }

        dreadnovaParent.transform.localScale = newScale;

        int warpTime = 5;

        Vector3 orPos = transform.position;
        Vector3 newPos = Vector3.zero;

        float warpScale = 5f;

        for (int i = 0; i < warpTime / warpScale; i++)
        {
            dreadnovaParent.transform.position = Vector3.Lerp(dreadnovaParent.transform.position, newPos, warpScale * Time.deltaTime);

            if (i >= warpTime / warpScale) break;

            yield return null;
        }

        dreadnovaParent.transform.position = newPos;
        warpSound.Play();

        DialogueSystem.Instance.AddDialogue(3);

        //wait for shield dissolve
        yield return new WaitForSeconds(1.5f);

        yield return new WaitForSeconds(5.0f);

        warping = false;
    }

    public DreadnovaSpawner Spawner { get { return spawner; } }
    public DreadnovaState State { get { return dreadnovaState; } }
    public ShieldGenerator[] Generators { get { return shieldGenerators; } }
}
