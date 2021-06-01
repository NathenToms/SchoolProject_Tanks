using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMove : MonoBehaviour
{
    public Transform target;
    public NumberRange numRange;
    float rotationAnglePerSec;

    private void Start()
    {
        rotationAnglePerSec = Random.Range(numRange.min, numRange.max);
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(target.position, Vector3.forward, rotationAnglePerSec * Time.deltaTime);
    }

}

[System.Serializable]
public class NumberRange
{
    public float min;
    public float max;
}