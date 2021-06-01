using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

/// <summary>
/// Sets warp effect and transitions
/// </summary>
public class WarpEffectBehaviour : MonoBehaviour
{
    public static WarpEffectBehaviour instance;

    [SerializeField] float warpTransitionSpeed = 4f;
    [SerializeField] VisualEffectAsset switchToEffect;
    [SerializeField] private bool startOnLoad;

    AudioSource audioSource;
    VisualEffect vfx;
    float min = 1.2f;
    float max = 60f;
    //bool warping = false;
    //float lerpVal = 30f;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) instance = this;

        vfx = GetComponent<VisualEffect>();

        vfx.enabled = startOnLoad;

        vfx.SetFloat("WarpRadius", min);

        TryGetComponent(out audioSource);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartWarp()
    {
        Debug.Log("Started warp");
        if (audioSource) audioSource.Play();
        StartCoroutine(Enable());
    }

    public void EndWarp()
    {
        Debug.Log("Ended warp");
        StartCoroutine(Disable());
    }

    IEnumerator Enable()
    {
        float lerpVal = max;
        vfx.enabled = true;

        while(lerpVal != min)
        {
            lerpVal = Mathf.Clamp(lerpVal - warpTransitionSpeed * Time.deltaTime, min, max);
            vfx.SetFloat("WarpRadius", lerpVal);
            yield return null;
        }
    }

    IEnumerator Disable()
    {
        vfx.SetFloat("WarpRadius", max);
        yield return new WaitForSeconds(2f);
        vfx.enabled = false;

        if (switchToEffect != null && vfx.visualEffectAsset != switchToEffect)
        {
            vfx.visualEffectAsset = switchToEffect;
        }
    }
}
