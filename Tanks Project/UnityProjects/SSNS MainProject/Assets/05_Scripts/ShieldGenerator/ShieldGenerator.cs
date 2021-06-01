using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldGenerator : MonoBehaviour
{
	[SerializeField] private GameObject _weakPoint;
	[SerializeField] private GameObject _generator;
	[SerializeField] private GameObject _shield;

	[SerializeField] private HealthAndShields _lifeSystems;

	[SerializeField] private List<GameObject> _armorPanels;

	[SerializeField] private bool _shieldAlive;
	[SerializeField] private bool _platesAlive;
	[SerializeField] private bool _generatorAlive;
	[SerializeField] private bool _boom;

	public bool PlatesAlive
	{
		get
		{
			foreach (GameObject plate in _armorPanels)
			{
				if (plate.activeSelf == true) return true;
			}

			return false;
		}
	}

	// Start is called before the first frame update
	private void Awake()
	{
		GetComponentInChildren<ShieldProjector>().onShieldHit += OnHit;
	}

	private void Start()
	{
		InitializeGenerator();
	}

	public void InitializeGenerator(bool panels = true)
	{
		_shieldAlive = true;
		_platesAlive = false;
		_generatorAlive = false;

		_boom = true;

		_shield.SetActive(true);

		_weakPoint.SetActive(false);
		_generator.SetActive(false);

		foreach (GameObject go in _armorPanels)
		{
			go.SetActive(true);
			ShieldGeneratorPlate shieldGeneratorPlate = go.GetComponentInParent<ShieldGeneratorPlate>();
			shieldGeneratorPlate.RepairPlate();
			go.SetActive(false);
		}
	}

	public void DestroyGenerator()
	{
		_shieldAlive = false;
		_platesAlive = false;
		_generatorAlive = true;

		_boom = false;

		_shield.SetActive(false);

		_weakPoint.SetActive(false);
		_generator.SetActive(false);

		foreach (GameObject go in _armorPanels)
		{
			go.SetActive(true);
			ShieldGeneratorPlate shieldGeneratorPlate = go.GetComponentInParent<ShieldGeneratorPlate>();
			shieldGeneratorPlate.DestroyPlate();
			go.SetActive(false);
		}
	}

	private void Update()
	{
		// If we're in the shield phase
		if (_shieldAlive && _shield.activeSelf == false)
		{
			_shieldAlive = false;
			_platesAlive = true;
			_generatorAlive = false;

			foreach (GameObject go in _armorPanels)
			{
				if (go == null) continue;
				go.SetActive(true);
			}

			_weakPoint.SetActive(true);
			_shield.SetActive(false);
		}

		// If we're in the plates phase and the plates are alive
		if (_platesAlive && PlatesAlive == false)
		{
			_shieldAlive = false;
			_platesAlive = false;
			_generatorAlive = true;

			foreach (GameObject go in _armorPanels)
			{
				if (go.activeSelf == false) continue;

				go.GetComponent<ShieldGeneratorPlate>().DestroyPlate();
				go.SetActive(false);
			}

			_weakPoint.SetActive(false);
			_generator.SetActive(true);
		}

		// If we're in the generator phase
		if (_generatorAlive && _generator.activeSelf == false)
		{
			if (_boom) {
				_boom = false;

				DialogueSystem.Instance.AddDialogue(22);

				// TODO -- Explode
				//InitializeGenerator();
			}
		}
	}

	public void OnHit(GameObject attacker)
	{
		if (attacker.TryGetComponent<Damage>(out Damage damage))
		{
			_lifeSystems.TakeDamage(damage.kineticDamage, damage.energyDamage);
		}
	}

	// Hit all plates
	public void WeakPointHit(GameObject attacker)
	{
		if (attacker.TryGetComponent<Damage>(out Damage damage))
		{
			foreach (GameObject plate in _armorPanels)
			{

				if (plate == null) continue;

				plate.GetComponent<FlashOnHit>().Flash();

				plate.GetComponent<HealthAndShields>().TakeDamage(damage.kineticDamage, damage.energyDamage);
			}
		}
	}

	public bool IsDead { get { return _generatorAlive && !_generator.activeSelf; } }

}
