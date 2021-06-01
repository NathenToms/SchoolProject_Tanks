using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Menu : MonoBehaviour
{
	/// <summary> The Animation that will play when transitioning to this Menu </summary>
	[SerializeField] protected Animator transition;
	[SerializeField] protected CinematicController cinematicController;
	
	/// <summary> The Menu that directed us to this Menu </summary>
	Menu parentMenu;

	public abstract void InitializeMenu();
	public abstract void PlayTransition();
	public abstract void UpdateMenu();

	public float wait = 1.5f;

	public void EndTransition()
	{
		cinematicController.inTransition = false;
	}

	// Start is called before the first frame update
	void Start()
	{
		cinematicController = GameObject.FindObjectOfType<CinematicController>();
		InitializeMenu();
	}
	// Update is called once per frame
	void Update()
	{
		UpdateMenu();
	}

	/// <summary> Transition to a different menu from this menu </summary>
	public virtual void OpenMenu(Menu newMenu)
	{
		//if (cinematicController && cinematicController.inTransition == false)
		//{
		//}

		StartCoroutine(ShowMenu(newMenu, newMenu.wait));
		newMenu.parentMenu = this;
		newMenu.PlayTransition();

	}
	public virtual void OpenMenu(Menu newMenu, bool updateParent = true)
	{
		//if (cinematicController && cinematicController.inTransition == false)
		//{
		//}

		StartCoroutine(ShowMenu(newMenu, newMenu.wait));
		newMenu.PlayTransition();

		if (updateParent == true) {
			newMenu.parentMenu = this;
		}
	}

	IEnumerator ShowMenu(Menu menu, float wait)
	{
		cinematicController.inTransition = true;

		CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
		canvasGroup.interactable = false;
		canvasGroup.alpha = 0;

		yield return new WaitForSeconds(wait);

		canvasGroup.interactable = true;
		canvasGroup.alpha = 1;

		menu.gameObject.SetActive(true);
		CloseMenu();
	}

	/// <summary> Handle closing the current Menu </summary> 
	public virtual void CloseMenu()
	{
		PlayExitTransition();
		gameObject.SetActive(false);
	}

	/// <summary> Return to the parent Menu </summary>
	public virtual void ReturnToLastMenu()
	{
		OpenMenu(parentMenu, false);
	}

	/// <summary> Play the exit transition for this Menu </summary>
	public virtual void PlayExitTransition()
	{
		if (transition)
		{
			transition.SetTrigger("Exit");
		}
	}
}
