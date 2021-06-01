using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

[RequireComponent(typeof(SphereCollider))]

public class ShieldProjector : MonoBehaviour
{
	#region Events ( OnShieldHit, OnShieldBreak, OnShieldRegen)
	// When the shield is hit
	public delegate void OnShieldHit(GameObject attacker);
	public OnShieldHit onShieldHit;

	// When the shield goes from any value to 0
	public delegate void OnShieldBreak();
	public OnShieldBreak onShieldBreak;

	// When the shield goes from 0 to any value
	public delegate void OnShieldRegen();
	public OnShieldRegen onShieldRegen;
	#endregion

	[Header("Shield Behaviour")]
	[SerializeField] FadeType fadeType = FadeType.NO_FADE;

	/// <summary> The color of this shield over its lifetime </summary>
	[Header("Shield Color")]
	public Gradient ShieldColor;

	[Header("Shield Effects")]
	[SerializeField] private GameObject dissolveVFX;
	[SerializeField] private GameObject impactVFX;
	[SerializeField] private GameObject hitVFX;

	[SerializeField] private Collider[] shipColliders;
	[SerializeField] private bool resizeCollider = false;
	[SerializeField] private bool updateShipColliders = true;

	// Get the current color of the shield between the base color and the broken color
	public Color GetColor => ShieldColor.Evaluate(_damagePercent);

	#region Private Members

	private List<Material> _objectsToFade = new List<Material>();

	private SphereCollider _shieldCollider;
	private Material _shieldMaterial;

	private float _damagePercent = 1;

	#endregion

	#region Unity Events
	// Start is called before the first frame update
	void Start()
	{
		_shieldMaterial = GetComponent<MeshRenderer>().material;

		_shieldMaterial.SetColor("_BaseColor", ShieldColor.Evaluate(0));
		_shieldMaterial.SetColor("_FresnelColor", ShieldColor.Evaluate(1));

        _shieldCollider = GetComponent<SphereCollider>();

		if (shipColliders != null)
		{
			foreach(Collider c in shipColliders)
			{
				IgnoreCollider(c);
			}
		}

		UpdateCollider(true);
	}

	// Update is called once per frame
	void Update()
	{
		// The shader for the impact objects used object space, I believe that is why the object using the shader (the shield)
		// needs to not be rotated. This like makes sure the impact shader works
		transform.rotation = Quaternion.identity;

		FadeShieldImpacts();
		FadeShieldShader();
	}

	// When the shield is hit
	private void OnCollisionEnter(Collision collision)
	{
		SpawnShieldImpact(collision);
		ResetShieldShader();

		if (onShieldHit != null)
		{
			onShieldHit.Invoke(collision.gameObject);
		}
	}
	#endregion

	// Update the shield 
	public void UpdateShieldPercent(float current, float max)
	{
		float newPercent = 1 / max * current;

		// If our shield is at 0%
		if (newPercent <= 0 && _damagePercent > 0)
		{
			// Invoke the On Shield Break Event
			if (onShieldBreak != null)
			{
				onShieldBreak.Invoke();
			}

			if (fadeType == FadeType.HALF_FADE)
			{
				// HALF_FADE means we fade the shield out so that only a small ring of color can be seen
				_shieldMaterial.SetColor("_BaseColor", Color.clear);
			}

			SpawnShieldDissolve();

			UpdateCollider(false);
		}

		// If we currently have a 0% shield and our new percent is anything above 0%
		if (_damagePercent <= 0 && newPercent > _damagePercent)
		{
			// Invoke the On Shield Regen Event
			if (onShieldRegen != null)
			{
				onShieldRegen.Invoke();
			}

			_shieldMaterial.SetColor("_BaseColor", GetColor);

			UpdateCollider(true);
		}

		// Update the damage percent
		_damagePercent = newPercent;
	}

	// Ignore a Collider
	public void IgnoreCollider(Collider collider)
	{
		if (collider) {
			Physics.IgnoreCollision(_shieldCollider, collider);
		}
	}

	// Resize this shields hitbox depending on the shield damage percent
	public void UpdateCollider(bool shieldEnabled)
	{
		if (resizeCollider)
		{
			if (shipColliders != null && updateShipColliders)
			{
				foreach (Collider c in shipColliders)
				{
					c.enabled = !shieldEnabled;
				}
			}

			_shieldCollider.enabled = shieldEnabled;
		}
	}

	#region Private Methods
	// Handle the spawning and despawning of of a impact effect
	void SpawnShieldImpact(Collision collision)
	{
		if (_damagePercent <= 0)
		{
			// Do nothing
			return;
		}

		// Spawn in a Impact effect object
		GameObject impact = Instantiate(impactVFX, transform.parent) as GameObject;
		impact.transform.localScale = Vector3.one * transform.localScale.magnitude * _shieldCollider.radius;

		transform.parent = transform.parent;

		//GameObject hit = Instantiate(HitVFX, collision.contacts[0].point, Quaternion.identity) as GameObject;

		// Get that objects material
		Material material = impact.GetComponent<Renderer>().material;

		// Add the impactObject to a list of materials
		// This is used to fade out hits to the shield
		_objectsToFade.Add(material);

		// Set the position of the sphere mask in the shield shader to display the new hit
		material.SetVector("_ImpactPosition", (transform.position - collision.contacts[0].point).normalized / -2f);

		// Destroy the new impact object after 2 seconds
		Destroy(impact, 2); 
	}

	// Spawn a shield dissolve object
	void SpawnShieldDissolve()
	{
		if (dissolveVFX == null)
		{
			// Do nothing
			return;
		}

		GameObject dissolve = Instantiate(dissolveVFX, transform.parent) as GameObject;
		
		dissolve.transform.localScale = Vector3.one * transform.localScale.magnitude * _shieldCollider.radius;
		dissolve.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", GetColor);

		Destroy(dissolve, 2);
	}

	// Fade the shield impact objects
	void FadeShieldImpacts()
	{
		// Loop through each ImpactObjects
		foreach (Material material in _objectsToFade)
		{
			// If that material is destroyed we remove it from the list
			if (material == null)
			{
				_objectsToFade.Remove(material);
			}
			else
			{
				// Fade the Impact objects 
				Color newColor = material.GetColor("_ImpactColor") + new Color(0, 0, 0, -2f) * Time.deltaTime;

				if (newColor.a < 0) {
					newColor.a = 0;
				}

				material.SetColor("_ImpactColor", newColor);
			}
		}
	}

	// Fade the values of the shield depending on the fade type
	void FadeShieldShader()
	{
		if (fadeType == FadeType.FULL_FADE)
		{
			// FULL_FADE means we fade the shield to nothing
			_shieldMaterial.SetColor("_BaseColor", Color.Lerp(_shieldMaterial.GetColor("_BaseColor"), Color.clear, 0.1f));
			_shieldMaterial.SetColor("_FresnelColor", Color.Lerp(_shieldMaterial.GetColor("_FresnelColor"), Color.clear, 0.1f));
		}
		else if (fadeType == FadeType.HALF_FADE)
		{
			// HALF_FADE means we fade the shield out so that only a small ring of color can be seen
			_shieldMaterial.SetFloat("_FresnelSize", Mathf.Lerp(_shieldMaterial.GetFloat("_FresnelSize"), 5, 0.1f));
		}
	}

	// Reset the values of the shield depending on the fade type
	void ResetShieldShader()
	{
		if (_damagePercent == 0)
		{
			// Do nothying
			return;
		}

		// When the shield is hit. We set the shield to full visibility
		_shieldMaterial.SetColor("_BaseColor", GetColor);
		_shieldMaterial.SetColor("_FresnelColor", GetColor);

		if (fadeType == FadeType.HALF_FADE)
		{
			// HALF_FADE means we fade the shield out so that only a small ring of color can be seen
			_shieldMaterial.SetFloat("_FresnelSize", 5);
		}
	}
	#endregion
	
	public Collider[] ShipColliders { get { return shipColliders; } }
}
