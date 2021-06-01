using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadnovaDistortionManager : MonoBehaviour
{
    [SerializeField] float distortTime = 5;
    [SerializeField] float maxStrength = 0.1f;

    Renderer rend;

    /// <summary>
    /// Starts distortion effect
    /// </summary>
    public void StartDistortion()
    {
        StartCoroutine(coStartDistortion());
    }

    /// <summary>
    /// Starts distortion effect. Takes a new time to use
    /// </summary>
    /// <param name="distortTime"></param>
    public void StartDistortion(float distortTime)
    {
        this.distortTime = distortTime;
        StartCoroutine(coStartDistortion());
    }

    IEnumerator coStartDistortion()
    {
        rend = GetComponent<Renderer>();
        Material mat = rend.material;
        float time = 0;

        while(time < distortTime)
        {
            Vector4 val = mat.GetVector("_distortionDirection");
            val.x = val.x + (maxStrength / distortTime * Time.deltaTime);
            val.y = val.x;

            mat.SetVector("_distortionDirection", val);

            time += Time.deltaTime;
            //Debug.Log(val.x);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
