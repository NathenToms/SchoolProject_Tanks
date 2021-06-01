using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Menu
{
	public override void InitializeMenu() { }
	public override void PlayTransition() { }
	public override void UpdateMenu() { }

	bool playedIntro = false;

	/// <summary>
	///  When the Quit Button is pressed
	/// </summary>
	public void QuitGame()
	{
		Application.Quit();
	}

	/// <summary> Declaring this makes sure the base method does not get called </summary>
	public override void PlayExitTransition()
	{
	}
}
