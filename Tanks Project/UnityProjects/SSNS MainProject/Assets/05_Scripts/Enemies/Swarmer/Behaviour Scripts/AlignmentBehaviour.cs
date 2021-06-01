using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Swarm/Behaviour/Alignment")]
public class AlignmentBehaviour : FlockBehaviour
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock, List<Transform> obstacles)
    {
        //if no neighbours, maintain current alignment
        if (context.Count == 0) return agent.transform.forward;

        //Add all the points and average
        Vector3 alignmentMove = Vector3.zero;
        foreach (Transform t in context)
        {
            alignmentMove += t.transform.forward;
        }
        alignmentMove /= context.Count;

        return alignmentMove;
    }
}
