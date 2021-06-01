using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private int speed = 20;
    [SerializeField] private Vector3 rotationDir = Vector3.zero;

    private void Awake()
    {
        if (rotationDir == Vector3.zero)
            rotationDir = Vector3.up;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation *= Quaternion.Euler(rotationDir * speed * Time.deltaTime);
    }
}
