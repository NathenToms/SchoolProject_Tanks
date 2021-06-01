using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX; //For visual effects graph

[RequireComponent(typeof(VisualEffect))]
public class AdjustParticleSpaceSmoke : MonoBehaviour
{
    VisualEffect smokeEffect;
    // Start is called before the first frame update
    void Awake()
    {
        smokeEffect = GetComponent<VisualEffect>();
    }

    public void ChangeCapacity(int capacity)
    {
        smokeEffect.SetInt("Capacity", capacity);
        smokeEffect.Play();
    }

    public void ChangeSize(Vector3 size)
    {
        smokeEffect.SetVector3("PositionFromCenter", size);
        smokeEffect.Play();
    }
}
