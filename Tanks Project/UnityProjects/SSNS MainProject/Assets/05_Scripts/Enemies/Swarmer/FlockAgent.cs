using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

[RequireComponent(typeof(Collider))]
public class FlockAgent : MonoBehaviour
{
    public Flock swarm;
    Collider agentCollider;
    public Collider AgentCollider { get { return agentCollider; } }

    // Start is called before the first frame update
    void Start()
    {
        agentCollider = GetComponent<Collider>();
    }

    public void Initialize(Flock swarmObj)
    {
        swarm = swarmObj;
        transform.parent = swarm.transform;
    }

    public void Move(Vector3 velocity, float shipSpeed, float rotationSpeed)
    {
        transform.forward =  Vector3.Lerp(transform.forward, velocity, Time.deltaTime);

        Quaternion newRot = Quaternion.LookRotation(velocity, transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRot, rotationSpeed * Time.deltaTime);

        transform.position += transform.forward * shipSpeed * Time.deltaTime;
    }

    private void OnDestroy()
    {
        if (swarm)
            swarm.agents.Remove(this);
    }
}
