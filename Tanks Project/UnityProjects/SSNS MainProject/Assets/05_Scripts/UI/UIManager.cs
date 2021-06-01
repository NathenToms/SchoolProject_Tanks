using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static Helper;

// NOTE -- Use the Tag "Menu" on all the parent items [Canvass] per menu

// The UI Manager controlles the games menus
// This is mainly controlled through the public ChangeMenu() function, allowing you to transition menus

/// <summary>
/// The Singleton to the UIManager Script
/// </summary>
public class UIManager : MonoBehaviour
{
	//// The Singleton reference
	public static UIManager Instance;

	// Set the singleton
	private void Awake()
	{
		Instance = this;
	}

	/// <summary>
	/// Change the menu that is being displayed
	/// </summary>
	/// <param name="newMenu"> The menu to change to </param>
	/// <param name="currentMenu"> The menu we are currently on </param>
	public void ChangeMenu(GameObject newMenu, GameObject currentMenu)
	{
		newMenu.SetActive(true);
		currentMenu.SetActive(false);
	}
}
