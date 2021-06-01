using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CinematicController : MonoBehaviour
{
	public static CinematicController Instance;

	[SerializeField] private GameObject mainMenu;

	private Animator animator;
	private GameObject ship;


	bool canSkip = true;
	public bool inTransition = false;

	public GameObject creditMenu;


	private void Awake()
	{
		Instance = this;
	}
	void Start()
	{
		ship = GameObject.FindGameObjectWithTag("Ship");
		animator = GetComponent<Animator>();
	}

	public void TransitionMainTo_Options()
	{
		animator.SetTrigger("TransitionToOptions");
	}
	public void TransitionMainTo_Setup()
	{
		animator.SetTrigger("TransitionToSetup");
	}

	public void TransitionOptionsTo_Main()
	{
		animator.SetTrigger("Exit");
	}
	public void TransitionSetupTo_Main()
	{
		animator.SetTrigger("Exit");
	}

	public void CreditsMenu()
	{
		animator.SetTrigger("CreditsMenu");
		mainMenu.SetActive(false);
		creditMenu.SetActive(true);
	}

	public void LeaveCreditsMenu()
	{
		animator.SetTrigger("Exit");
		creditMenu.SetActive(false);
		mainMenu.SetActive(true);
	}

	public void PlayGame()
	{
		animator.SetTrigger("PlayGame");

		WarpEffectBehaviour.instance.StartWarp();
	}

	public void LoadGame()
	{
		StartCoroutine(LoadScene());
	}

	private IEnumerator LoadScene()
	{
		AsyncOperation load = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);

		while (!load.isDone)
		{
			yield return null;
		}
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	public void Return()
	{
	}

	public void Select()
	{
	}

	public void StartTransition()
	{
		inTransition = true;
	}



    // Update is called once per frame
    void Update()
    {
		if (Input.GetMouseButtonDown(0)) {
			EndIntroCinematic();
		}
    }
	
	public void EndIntroCinematic()
	{
		Animator animator = GetComponent<Animator>();

		if (animator && canSkip) {
			//TurnOffTrails();  //Triggered in animation already
			animator.Play("Intro_Animation", 0, 0.95f);
			animator.SetTrigger("TransitionToMain");
		}

		canSkip = false;
	}

	public void TurnOnTrails()
	{
		TrailRenderer[] trails = GameObject.FindObjectsOfType<TrailRenderer>();
		foreach (TrailRenderer trail in trails) { trail.emitting = true; }
	}
	public void TurnOffTrails()
	{
		TrailRenderer[] trails = GameObject.FindObjectsOfType<TrailRenderer>();
		foreach (TrailRenderer trail in trails) { trail.emitting = false; }
	}

	public void ShowMainMenu()
	{
		if (!creditMenu.activeSelf)
			mainMenu.SetActive(true);
	}
	public void HideMainMenu()
	{
		mainMenu.SetActive(false);
	}
}
