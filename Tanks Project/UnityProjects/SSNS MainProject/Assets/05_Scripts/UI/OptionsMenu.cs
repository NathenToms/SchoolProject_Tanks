using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : Menu
{
	public override void InitializeMenu() { }
	public override void UpdateMenu() { }

	public override void PlayTransition()
	{
		if (transition == null)
		{
			// Do nothing
			return;
		}
		else
		{
			transition.SetTrigger("TransitionToOptions");
		}
	}
}
