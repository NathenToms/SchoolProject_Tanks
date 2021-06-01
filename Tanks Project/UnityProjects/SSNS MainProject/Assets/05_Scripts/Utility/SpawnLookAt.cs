using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLookAt : MonoBehaviour
{
    [SerializeField] public Transform point;
    [SerializeField] bool usePointUpVector;
    // Start is called before the first frame update
    void Start()
    {
        if(usePointUpVector) transform.rotation = Quaternion.LookRotation(point.position - transform.position, point.up);
        else transform.rotation = Quaternion.LookRotation(point.position - transform.position);
    }
}
