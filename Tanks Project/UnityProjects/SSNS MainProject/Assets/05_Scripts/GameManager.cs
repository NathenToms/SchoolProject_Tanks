using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles switching from states
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool debug;
    public bool paused = false;

    [SerializeField] private GameState gameState = GameState.WARPING;

    [SerializeField] private int seed;

    private System.Random random;

    private ShipController shipController;

    private const int MAX_RESPAWN_TIME = 5;

    private float respawnTime;

    private bool startup;
    private bool exiting;

    /// <summary>
    /// Pause the game and open the pause menu
    /// This is called from the players OnPause
    /// </summary>
    public void PauseGame()
	{
		Player[] players = FindObjectsOfType<Player>();

		foreach (Player player in players)
		{
			string newActions = !paused ? "MenuNavigation" : "Ship";
			player.PlayerInput.SwitchCurrentActionMap(newActions);
		}

		paused = !paused;

		GameObject pauseMenu = GameObject.Find("PauseMenu");
		if (pauseMenu && pauseMenu.TryGetComponent<Animator>(out Animator animator))
		{
			bool open = animator.GetBool("Open");
			animator.SetBool("Open", !open);
		}
	}

    /// <summary>
    /// Switch to provided state
    /// </summary>
    /// <param name="gameState">The game state to switch to</param>
    public void SwitchState(GameState gameState)
    {
        this.gameState = gameState;

        UpdateState();
    }

    /// <summary>
    /// Transition to new state
    /// </summary>
    private void UpdateState()
    {
        if (!shipController)
            shipController = GameObject.FindGameObjectWithTag("Player").GetComponent<ShipController>();

        HealthAndShields healthAndShields = shipController.GetComponent<HealthAndShields>();

        switch (gameState)
        {
            case GameState.BATTLE:
                MusicManager.instance.RandomTrack(MusicTrackType.COMBAT);
                healthAndShields.Invincible = false;
                shipController.Warping = false;
                //WarpEffectBehaviour.instance.WarpTransitionSpeed = 40;
                WarpEffectBehaviour.instance.EndWarp();
                startup = false;
                break;
            case GameState.BATTLE_END:
                MusicManager.instance.RandomTrack(MusicTrackType.NON_COMBAT);
				healthAndShields.ResetValues();
                healthAndShields.Invincible = true;
                AreaManager.Instance.EndArea();
                break;
            case GameState.PAUSE:
                break;
            case GameState.WARPING:
                shipController.Warping = true;
                shipController.Freeze = false;
                shipController.StopThrust = false;
                shipController.AlignGunnerWithPilot();
                if (!startup)
                    WarpEffectBehaviour.instance.StartWarp();
                //AreaManager.Instance.LoadNewArea();
                StartCoroutine(DelayedAreaLoad());
                break;
            case GameState.GAME_OVER:
            case GameState.VICTORY:
                StartCoroutine(DelayedExit());
                break;
        }
    }

    IEnumerator DelayedAreaLoad()
    {
        yield return new WaitForSeconds(6f);
        AreaManager.Instance.LoadNewArea();
        shipController.AlignGunnerWithPilot();
    }

    IEnumerator DelayedExit()
    {
        switch (gameState)
        {
            case GameState.GAME_OVER:
                VideoManager.Instance.PlayVideo(VideoType.GAME_OVER);
                break;
            case GameState.VICTORY:
                yield return new WaitForSeconds(10f);
                VideoManager.Instance.PlayVideo(VideoType.VICTORY);
                break;
        }

        DisableBattle();

        exiting = true;
    }

    private void CheckForRespawn()
    {
        
        if (shipController && !shipController.gameObject.activeSelf)
        {
            SwitchState(GameState.RESPAWN);

            // TODO: Some effect for respawning
        }
    }

    private void RespawnPlayer()
    {
        respawnTime += Time.deltaTime;

        if (respawnTime >= MAX_RESPAWN_TIME)
        {
            respawnTime = 0;

            if (shipController && shipController.TryGetComponent(out PlayerRespawn playerRespawn))
            {
                playerRespawn.Respawn();
            }
        }
    }

    public void DisableBattle()
    {
        AreaManager.Instance.CurrentArea.gameObject.SetActive(false);
        shipController.gameObject.SetActive(false);

        DialogueSystem.Instance.gameObject.SetActive(false);
        MusicManager.instance.gameObject.SetActive(false);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;

        random = new System.Random(seed);

        shipController = GameObject.FindGameObjectWithTag("Player").GetComponent<ShipController>();

        Physics.IgnoreLayerCollision(12, 12);
        Physics.IgnoreLayerCollision(11, 12);
        Physics.IgnoreLayerCollision(10, 12);

        startup = true;
    }

    private void Start()
    {
        UpdateState();
    }

    private void LateUpdate()
    {
        switch (gameState)
        {
            case GameState.WARPING:
                break;
            case GameState.BATTLE:
                CheckForRespawn();
                break;
            case GameState.RESPAWN:
                RespawnPlayer();
                break;
            case GameState.BATTLE_END:
                break;
            case GameState.PAUSE:
                break;
            case GameState.VICTORY:
            case GameState.GAME_OVER:

                if (exiting && !VideoManager.Instance.IsPlaying)
                {

                    foreach (Player player in FindObjectsOfType<Player>())
                    {
                        player.SetPlayerActionMap("MenuNavigation");
                    }

                    SceneManager.LoadScene("MainMenu");
                }

                break;
        }
    }

    public GameState GameState { get { return gameState; } }
    public System.Random Random { get { return random; } }
    public ShipController Player { get { return shipController; } }
}
