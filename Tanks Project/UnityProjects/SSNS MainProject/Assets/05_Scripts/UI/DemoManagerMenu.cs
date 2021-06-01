using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoManagerMenu : MonoBehaviour
{
	[SerializeField] private DemoManager demoManager;
	[SerializeField] private HealthAndShields playerHp;
	[SerializeField] private Image playerGodmode;

	// This menus Rect
	RectTransform menuRect;

	// Menu positions
	Vector3 hiddenPos = Vector3.zero;
	Vector3 initialPos = Vector3.zero;
	Vector3 targetPos = Vector3.zero;

	// Is this menu open ?
	public bool visible = false;

	// Start is called before the first frame update
	void Start()
    {
		if (gameObject.TryGetComponent<RectTransform>(out RectTransform rectTransform)) {

			initialPos = rectTransform.position;
			hiddenPos = initialPos + new Vector3(rectTransform.rect.width * 1.25f, 0, 0);

			targetPos = hiddenPos;

			menuRect = rectTransform;
			menuRect.position = hiddenPos;

			menuRect.gameObject.SetActive(true);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (Vector3.Distance(menuRect.transform.position, targetPos) > 15f)
		{
			// If we're close to our target stop moving
			menuRect.transform.position = Vector3.Lerp(menuRect.transform.position, targetPos, 0.25f);
		}
		else
		{
			// Else we turn off the menu if its hidden
			if (visible == false)
			{
				menuRect.gameObject.SetActive(false);
			}
		}

		playerGodmode.color = playerHp.enabled ? Color.green : Color.red;
	}

	/// <summary>
	/// Toggle the visability of the Demo window
	/// </summary>
	public void ToggleMenu(bool state)
	{
		visible = state;
		targetPos = (visible) ? initialPos : hiddenPos;
		menuRect.gameObject.SetActive(true);
	}

	/// <summary>
	/// Change the skybox
	/// </summary>
	public void ToggleSkybox()
	{
		demoManager.ToggleSkybox();
	}

	/// <summary>
	/// Spawn a Charger Enemy
	/// </summary>
	public void SpawnCharger()
	{
		// Spawn enemies using the SpawnEnemy() method in DemoManager
		demoManager.SpawnEnemy( demoManager.chargerPrefab, Vector3.zero, Input.GetKey(KeyCode.LeftShift) ? 10 : 1);
	}

	/// <summary>
	/// Spawn a Fighter Enemy
	/// </summary>
	public void SpawnFighter()
	{
		// Spawn enemies using the SpawnEnemy() method in DemoManager
		demoManager.SpawnEnemy( demoManager.fighterPrefab, Vector3.zero, Input.GetKey(KeyCode.LeftShift) ? 10 : 1);
	}

	/// <summary>
	/// Move the player (0,0,0)
	/// </summary>
	public void TeleportPlayer()
	{
		demoManager.TeleportPlayer();
	}

	/// <summary>
	/// Change the player to godmode
	/// </summary>
	public void SetPlayerGodMode()
	{
		playerHp.enabled = !playerHp.enabled;
	}

	/// <summary>
	/// Change the active camera
	/// </summary>
	public void ChangeCamera(int ID)
	{
		if (demoManager.cameras.Length >= ID) {
			demoManager.UpdateActiveCamera(demoManager.cameras[ID]);
		}
	}
}
