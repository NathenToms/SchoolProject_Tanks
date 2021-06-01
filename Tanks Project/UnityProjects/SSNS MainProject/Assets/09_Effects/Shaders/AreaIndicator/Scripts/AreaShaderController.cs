using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaShaderController : MonoBehaviour
{
	[SerializeField] private AnimationCurve lerpCurve;
	[SerializeField] private MeshRenderer _proximityMeshRenderer;
	[SerializeField] private GameObject player;

	[ColorUsage(true, true)]
	[SerializeField] private Color startColor;
	[ColorUsage(true, true)]
	[SerializeField] private Color transitionColor;
	[ColorUsage(true, true)]
	[SerializeField] private Color finalColor;

	private Material _hologramMaterial;
	private Material _proximityMaterial;

	public float Duration = 5;

	private void Start()
	{
		// If we don't provide a material, try to get one from our children
		_hologramMaterial = _hologramMaterial ? _hologramMaterial : GetComponentInChildren<MeshRenderer>().material;
		_proximityMaterial = _proximityMeshRenderer.material;

		player = GameObject.FindGameObjectWithTag("Player");

		StartCoroutine(coSpawnArea());
	}

	private void Update()
	{
		if (player != null)
		{
			_proximityMaterial.SetVector("_Position", player.transform.position );
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, transform.localScale.x);
	}

	private IEnumerator coSpawnArea()
	{
		float counter = 0f;

		while ((counter += Time.deltaTime) <= Duration)
		{
			// Pass the new percent to the shader
			_hologramMaterial.SetFloat("_Percent", lerpCurve.Evaluate(counter / Duration));

			_hologramMaterial.SetColor("_Color", GetTransitionColor(counter / Duration));

			yield return null;
		}
		
		counter = 0;

		while ((counter += Time.deltaTime) <= Duration)
		{
			_hologramMaterial.SetColor("_Color", GetFinalColor(counter / Duration));

			yield return null;
		}

		_hologramMaterial.SetColor("_Color", GetFinalColor(1));
	}

	private Color GetTransitionColor(float percent)
	{
		return Color.Lerp(transitionColor, startColor, lerpCurve.Evaluate(percent));
	}

	private Color GetFinalColor(float percent)
	{
		return Color.Lerp( finalColor, transitionColor, lerpCurve.Evaluate(percent));
	}
}
