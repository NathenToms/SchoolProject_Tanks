using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehaviour : MonoBehaviour
{
    [SerializeField] LineRenderer laserLine;
    [SerializeField] ParticleSystemRenderer laserParticle;
    [SerializeField] ParticleSystemRenderer ringParticle;
    [SerializeField] ParticleSystem emissionParticle;
    ParticleSystem.EmissionModule emission;

    public float radius = 15f;
    [SerializeField] float laserLength = 500f;
    [SerializeField] float laserDamagePerSecond = 5f;
    float particleCount;
    public bool fadeIn = false;

    Material[] laserMaterials;
    public float Length { get { return laserLength; } }
    public float Damage { get { return laserDamagePerSecond * Time.deltaTime; } }

    private void Start()
    {
        laserMaterials = new Material[3];
        laserMaterials[0] = laserLine.material;
        laserMaterials[1] = laserParticle.material;
        laserMaterials[2] = ringParticle.material;

        emission = emissionParticle.emission;
        particleCount = emission.rateOverTime.constant;
    }

    private void Update()
    {
        float fade;
        float fade2;
        float alpha;

        if (fadeIn)
        {
            fade = Mathf.Clamp((laserMaterials[0].GetFloat("Vector1_2C1D604B") - 0.5f * Time.deltaTime), 0.5f, 1); //Using alphaClip
            fade2 = Mathf.Clamp((laserMaterials[1].GetFloat("Vector1_2C1D604B") - 0.7f * Time.deltaTime), 0, 1);
            alpha = Mathf.Clamp((laserMaterials[2].GetFloat("Vector1_2DFED300") + 0.2f * Time.deltaTime), 0, 0.5f); //Using alpha
            emission.rateOverTime = particleCount;
        }
        else
        {
            fade = Mathf.Clamp((laserMaterials[0].GetFloat("Vector1_2C1D604B") + 0.5f * Time.deltaTime), 0.5f, 1);
            fade2 = Mathf.Clamp((laserMaterials[1].GetFloat("Vector1_2C1D604B") + 0.7f * Time.deltaTime), 0, 1);
            alpha = Mathf.Clamp((laserMaterials[2].GetFloat("Vector1_2DFED300") - 0.5f * Time.deltaTime), 0, 0.5f);
            emission.rateOverTime = 0;
        }

        laserMaterials[0].SetFloat("Vector1_2C1D604B", fade);
        laserMaterials[1].SetFloat("Vector1_2C1D604B", fade2);
        laserMaterials[2].SetFloat("Vector1_2DFED300", alpha);
    }

    private void OnEnable()
    {
        StartCoroutine(Reset());
    }

    IEnumerator Reset()
    {
        fadeIn = false;
        yield return null;
        yield return null;
        laserMaterials[0].SetFloat("Vector1_2C1D604B", 1);
        laserMaterials[1].SetFloat("Vector1_2C1D604B", 1);
        laserMaterials[2].SetFloat("Vector1_2DFED300", 0);
    }
}
