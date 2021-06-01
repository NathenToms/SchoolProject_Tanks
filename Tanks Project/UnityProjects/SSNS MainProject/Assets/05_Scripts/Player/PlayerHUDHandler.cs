using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHUDHandler : MonoBehaviour
{
	private ShipController _shipController;
	private HealthAndShields _shipHealth;

    [SerializeField] private Image pilotRedicle;
    [SerializeField] private Image gunnerRedicle;

	[HideInInspector] public Slider _slider_Health;
	[HideInInspector] public Slider _slider_Shield;
	[HideInInspector] public Slider _slider_Boost;

	[HideInInspector] public Image _healthImage;
	[HideInInspector] public Image _shieldImage;
	[HideInInspector] public Image _boostImage;

	private void Awake()
	{
		_shipController = GetComponent<ShipController>();

		_slider_Health = GameObject.Find("[Slider] Health").GetComponent<Slider>();
		_slider_Shield = GameObject.Find("[Slider] Shield").GetComponent<Slider>();
		_slider_Boost  = GameObject.Find("[Slider] Boost").GetComponent<Slider>();

		_healthImage = _slider_Health.gameObject.GetComponentInChildren<Image>();
		_shieldImage = _slider_Shield.gameObject.GetComponentInChildren<Image>();
		_boostImage  = _slider_Boost.gameObject.GetComponentInChildren<Image>();

		_shipHealth = GetComponent<HealthAndShields>();
	}

	private void Update()
	{
		UpdateBoostGauge();

		UpdateHealthAndShields();
	}

	public void UpdateBoostGauge()
	{
		// Boost Gauge
		if (_shipController.BoostGauge < _shipController.Properties.maxBoostGauge && _shipController.Boosting == false)
		{
			float rechargeRate = 1.5f;
			
			_shipController.BoostGauge += rechargeRate * Time.deltaTime;
			
			if (_shipController.BoostGauge > _shipController.Properties.maxBoostGauge) {
				_shipController.BoostGauge = _shipController.Properties.maxBoostGauge;
			}

			// Boost Slider
			_slider_Boost.value = (1 / _shipController.Properties.maxBoostGauge) * _shipController.BoostGauge;

			// Set the color of the boost slider
			_boostImage.color = Color.Lerp(Color.red, Color.yellow, (1 / _shipController.Properties.maxBoostGauge) * _shipController.BoostGauge);
		}
	}

	public void UpdateHealthAndShields()
	{
		_slider_Health.value = (1 / _shipHealth.MaxLife) * _shipHealth.currentLife;
		_slider_Shield.value = (1 / _shipHealth.MaxShield) * _shipHealth.currentShield;
	}

    public void BlinkRedicle(SNSSTypes.PlayerRole role)
    {
        StopAllCoroutines();
        pilotRedicle.rectTransform.localScale = Vector3.one;
        pilotRedicle.color = new Color(1, 1, 1, pilotRedicle.color.a);

        gunnerRedicle.rectTransform.localScale = Vector3.one;
        gunnerRedicle.color = new Color(1, 1, 1, gunnerRedicle.color.a);

		if (!gameObject.activeSelf) return;

        if (role == SNSSTypes.PlayerRole.Pilot)
        {
            StartCoroutine(Blink(pilotRedicle));
        }
        else if (role == SNSSTypes.PlayerRole.Gunner)
        {
            StartCoroutine(Blink(gunnerRedicle));
        }
    }

    private IEnumerator Blink(Image redicle)
    {
        bool decrease = false;
        float scale = 1;

        float max = 1.01f;

        redicle.rectTransform.localScale = Vector3.one * scale;

        redicle.color = new Color(1, 0, 0, redicle.color.a);

        while (true)
        {
            if (decrease)
            {
                scale = Mathf.Clamp(scale - 0.1f, 1, max);
            }
            else
            {
                scale = Mathf.Clamp(scale + 0.1f, 1, max);

                if (scale == max)
                    decrease = true;
            }

            redicle.rectTransform.localScale = Vector3.one * scale;

            if (decrease && scale == 1)
                break;

            yield return new WaitForEndOfFrame();
        }

        redicle.color = new Color(1, 1, 1, redicle.color.a);
    }
}
