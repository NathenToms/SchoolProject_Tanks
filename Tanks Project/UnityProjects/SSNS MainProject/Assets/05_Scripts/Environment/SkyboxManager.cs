using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;

/// <summary>
/// Manages all skyboxes and used for easily switching between them
/// </summary>
public class SkyboxManager : MonoBehaviour
{
    [SerializeField]StaticLightingSky skyLighting;
    public static SkyboxManager Instance;

    [SerializeField] Cubemap[] skyboxes; // List of skyboxes in game

    HDRISky skybox;
    Volume profile;

    int skyboxIndex;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;

        skyboxIndex = 0;

        // Try to get a reference to the Skybox
        if (profile = GameObject.FindObjectOfType<Volume>())
        {
            profile.profile.TryGet(out skybox);
        }
        else
        {
            Debug.LogError("We could not find the Volume profile in your scene");
        }
    }

    /// <summary>
    /// Switches to skybox
    /// </summary>
    /// <param name="index">The index of the skybox</param>
    public void SwitchToSkybox(int index)
    {
        skyboxIndex = index;

        skybox.hdriSky.Override(skyboxes[skyboxIndex]);

        StartCoroutine(ChangeSkyLighting());
    }

    /// <summary>
    /// Loops through all skyboxes
    /// Used for debugging
    /// </summary>
    public void LoopSkybox()
    {
        SwitchToSkybox((skyboxIndex + 1) % skyboxes.Length);
    }

    IEnumerator ChangeSkyLighting()
    {
        skyLighting.enabled = false;
        yield return new WaitForFixedUpdate();
        skyLighting.enabled = true;
    }

    public int SkyboxAmount { get { return skyboxes.Length; } }

    public int CurrentSkybox { get { return skyboxIndex; } }
}
