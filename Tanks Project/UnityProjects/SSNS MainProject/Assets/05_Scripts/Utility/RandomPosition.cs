using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPosition : MonoBehaviour
{
    public bool debug = false;
    public float radius = 500; // Radiuas from world center
    // Start is called before the first frame update
    void Start()
    {
        radius *= 12;
        Vector3 newPos = Vector3.zero;

        newPos.x = Random.Range(-radius, radius);
        newPos.y = Random.Range(-radius, radius);
        newPos.y = Random.Range(-radius, radius);

        transform.position = newPos;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
