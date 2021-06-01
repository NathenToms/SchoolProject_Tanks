using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSystems : MonoBehaviour
{
	private List<Material> impactObjects = new List<Material>();

	[SerializeField] private MeshRenderer meshRenderer;
	[SerializeField] private GameObject HitVFX;

	private Color baseColor;
	private Color fresnelColor;

	// Start is called before the first frame update
	void Start()
	{
		baseColor = meshRenderer.material.GetColor("_BaseColor");
		fresnelColor = meshRenderer.material.GetColor("_FresnelColor");
	}

	// Update is called once per frame
	void Update()
	{
		transform.rotation = Quaternion.identity;

		// Loop through each impactObjects
		foreach (Material material in impactObjects)
		{
			// If that material is destroyed we remove it from the list
			if (material == null)
			{
				impactObjects.Remove(material);
			}
			else
			{
				// Fade the impact  objects 
				Color currentColor = material.GetColor("_ImpactColor");
				Color newColor = currentColor + new Color(0, 0, 0, -(2f * Time.deltaTime));

				if (newColor.a < 0) {
					newColor.a = 0;
				}

				material.SetColor("_ImpactColor", newColor);
			}
		}

		// Slowly fade out the objects over time
		meshRenderer.material.SetColor("_BaseColor", Color.Lerp(meshRenderer.material.GetColor("_BaseColor"),Color.clear, 0.05f));
		meshRenderer.material.SetColor("_FresnelColor", Color.Lerp(meshRenderer.material.GetColor("_FresnelColor"), Color.clear, 0.05f));
	}

	private void OnCollisionEnter(Collision collision)
	{
		// When the shield is hit. We set the shield to full visibility
		meshRenderer.material.SetColor("_BaseColor", baseColor);
		meshRenderer.material.SetColor("_FresnelColor", fresnelColor);

		// Spawn in a Impact effect object
		GameObject impactObject = Instantiate(HitVFX, transform) as GameObject;

		// Get that objects material
		Material material = impactObject.GetComponent<Renderer>().material;

		// Add the impactObject to a list of materials
		// This is used to fade out hits to the shield
		impactObjects.Add(material);

		// Impact Position
		Vector3 position = -(transform.position - collision.contacts[0].point).normalized / 2f;
		material.SetVector("_ImpactPosition", new Vector4(position.x, position.y, position.z, 0));

		// Destroy the new impact object after 2 seconds
		Destroy(impactObject, 2);
	}
}
