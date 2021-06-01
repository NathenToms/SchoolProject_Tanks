using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Swarm/Behaviour/Steered Cohesion")]
public class SteeredCohesionBehaviour : FlockBehaviour
{
    Vector3 currentVelocity;
    public float agentSmoothTime = 0.5f;
 
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock, List<Transform> obstacles)
    {
        //if no neighbours, return no adjustment
        if (context.Count == 0) return Vector3.zero;

        //Add all the points and average
        Vector3 cohesionMove = Vector3.zero;
        foreach (Transform t in context)
        {
            cohesionMove += t.position;
        }
        cohesionMove /= context.Count;

        //Create offset from agent position
        cohesionMove -= agent.transform.position;
        cohesionMove = Vector3.SmoothDamp(agent.transform.forward, cohesionMove, ref currentVelocity, agentSmoothTime);
        //cohesionMove = Vector3.Lerp(agent.transform.forward, cohesionMove, Time.deltaTime);

        return cohesionMove;
    }
}