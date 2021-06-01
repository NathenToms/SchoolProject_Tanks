using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Swarm/Behaviour/Stay In Radius")]
public class StayInRadiusBehaviour : FlockBehaviour
{
    //public float radius = 10f;
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock, List<Transform> obstacles)
    {
        Vector3 target = flock.FlockLeaderPosition;
        Vector3 centerOffset = target - agent.transform.position;
        float t = centerOffset.magnitude / flock.swarmFollowRadius;
        //If inside radius return a zero vector3 so that its not taken into account when moving
        if (t < 0.9f)
        {
            //Debug.Log("Inside radius.");
            return Vector3.zero;
        }
        //Debug.Log("Not inside radius.");

        return centerOffset * t * t;
    }
}
