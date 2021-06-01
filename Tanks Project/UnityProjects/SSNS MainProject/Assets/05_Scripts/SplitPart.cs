using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class SplitPart : MonoBehaviour
{
    [SerializeField] private GameObject partsParent;
    [SerializeField] private float speed = 1000;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform part in partsParent.transform)
        {
            Vector3 dir = (part.transform.position - partsParent.transform.position).normalized;

            dir += Random.insideUnitSphere * 10;

            dir = dir.normalized;

            Vector3 rotation = Random.insideUnitSphere * speed;

            if (part.TryGetComponent(out Rigidbody rigid))
            {
                rigid.AddForce(dir * Random.Range(speed / 1.3f, speed), ForceMode.VelocityChange);

                rigid.AddTorque(rotation, ForceMode.VelocityChange);
            }

            if (part.TryGetComponent(out VisualEffect vfx))
            {
                vfx.SetVector3("Force Amount", -dir);
            }
        }
    }
}
