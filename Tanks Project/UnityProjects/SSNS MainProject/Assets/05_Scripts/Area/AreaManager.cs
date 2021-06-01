using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;
using UnityEngine.UI;

/// <summary>
/// Manager that handles each area (node)
/// Allows loading for new areas and destroys the old area
/// </summary>
public class AreaManager : MonoBehaviour
{
    public static AreaManager Instance;

    public delegate void AreaLoadEventHandler();

    public event AreaLoadEventHandler AreaLoaded;

    private const int MIN_TRAVEL_TIME = 10;
    private const int MAX_OUTSIDE_TIME = 10;
    private const int WARP_HELD_TIME = 5;

    [SerializeField] private GameObject portalPrefab;
    private GameObject portal;

    [SerializeField] private Image outsideOverlay;
    [SerializeField] private Text boostNotification;

    [SerializeField] private int areaIndex;
    [SerializeField] private Area[] areas;

    private Area lastArea;
    private Area currentArea;

    private GameObject areaEffect;

    private bool nextAreaLoaded;
    private bool lastAreaDestroyed;
    private bool areaEnded;
    private bool portalAhead;

    private float startTravelTime;

    private float outsideTime;

    private float boostHeldTime;

    private Color outsideStartColor;
    private Color outsideTargetColor;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(Instance.gameObject);
		}

		Instance = this;

		boostNotification.gameObject.SetActive(false);

		outsideStartColor = outsideOverlay.color;
		outsideStartColor.a = 0;

		outsideTargetColor = outsideOverlay.color;

		outsideOverlay.color = outsideStartColor;

		currentArea = areas[areaIndex];
	}

	// Update is called once per frame
	void Update()
	{
		// Waits for reward UI to hide before switching to node selection
		if (areaEnded)
		{
			if (GameManager.Instance.Player.Boosting || currentArea.IsPlayerOutside)
			{
				boostHeldTime += Time.deltaTime;

				if (boostHeldTime >= WARP_HELD_TIME)
				{
					areaEnded = false;
					Boosting = false;
					boostHeldTime = 0;
					areaIndex++;

					boostNotification.gameObject.SetActive(false);

					GameManager.Instance.SwitchState(GameState.WARPING);
				}

				GameManager.Instance.Player.Warping = true;
			}
			else
			{
				boostHeldTime = Mathf.Max(boostHeldTime - Time.deltaTime, 0);

				GameManager.Instance.Player.Warping = false;
			}

			boostNotification.text = "Boost for " + (int)(WARP_HELD_TIME - boostHeldTime) + "s";
		}
		// Transition to next node
		else if (nextAreaLoaded && lastAreaDestroyed)
		{
			StartCoroutine(TransitionAreas());
		}

		if (GameManager.Instance.GameState == GameState.BATTLE)
		{
            if (!outsideOverlay.IsActive())
                outsideOverlay.gameObject.SetActive(true);

            if (currentArea.IsPlayerOutside)
			{
				outsideTime += Time.deltaTime;

                if (!boostNotification.gameObject.activeSelf)
                {
                    boostNotification.gameObject.SetActive(true);

                    DialogueSystem.Instance.AddDialogue(19);
                }

                boostNotification.text = "Termination in " + Mathf.RoundToInt(MAX_OUTSIDE_TIME - outsideTime) + "s";

                if (outsideTime >= MAX_OUTSIDE_TIME)
				{
					outsideTime = 0;
                    boostNotification.gameObject.SetActive(false);

                    if (GameManager.Instance.Player.TryGetComponent(out HealthAndShields health))
					{
						health.TakeDamage(Mathf.Infinity, Mathf.Infinity);
					}
                }
            }
			else
			{
				float range = currentArea.Size - (currentArea.Size - 150);
				float input = Mathf.Abs(Vector3.Distance(GameManager.Instance.Player.transform.position, currentArea.transform.position));
				float t = Mathf.Clamp((input - (currentArea.Size - 150)) / range, 0, 1);
				outsideOverlay.color = Color.Lerp(outsideStartColor, outsideTargetColor, t);
                boostNotification.gameObject.SetActive(false);
			}
		}
        else if (GameManager.Instance.GameState == GameState.RESPAWN && outsideOverlay.IsActive())
        {
            outsideOverlay.gameObject.SetActive(false);
        }
	}

	private void CheckForPortal()
    {
        portalAhead = Physics.SphereCast(GameManager.Instance.Player.transform.position, 50, GameManager.Instance.Player.transform.forward, out RaycastHit raycastHit, currentArea.Size * 2, 1 << 15);
    }

    /// <summary>
    /// Ends the area giving the ship the node's reward
    /// and waits for the UI to be hidden again before allowing next node selection
    /// </summary>
    public void EndArea()
    {
        if (!areaEnded)
        {
            areaEnded = true;

            //portal = Instantiate(portalPrefab, currentArea.transform);
            //portal.transform.position = GameManager.Instance.Player.transform.position + GameManager.Instance.Player.transform.forward * 200;
            boostNotification.gameObject.SetActive(true);

            GameManager.Instance.SwitchState(GameState.BATTLE_END);
        }
    }

    /// <summary>
    /// Loads the area given. Uses the NodeEvent script to spawn
    /// prefabs that should handle spawning objects in the area.
    /// </summary>
    /// <param name="nodeInfo">The node to load</param>
    public void LoadNewArea()
    {

        outsideTime = 0;

        outsideOverlay.color = outsideStartColor;

        // Moves player to a spot away from any areas
        //GameManager.Instance.Player.gameObject.SetActive(false);
        GameManager.Instance.Player.transform.position = Vector3.zero;
        GameManager.Instance.Player.transform.rotation = Quaternion.identity;
        //GameManager.Instance.Player.gameObject.SetActive(true);

        if (areaIndex >= areas.Length)
        {
            GameManager.Instance.SwitchState(GameState.VICTORY);
            return;
        }

        // Creates new area storing the old one
        lastArea = currentArea;

        // Disables the last area
        if (lastArea != null)
            lastArea.gameObject.SetActive(false);

        currentArea = areas[areaIndex];

        startTravelTime = Time.time;

        if (areaIndex == 0)
            DialogueSystem.Instance.AddDialogue(10);
        else if (currentArea.AreaType == AreaType.BossShield)
            DialogueSystem.Instance.AddDialogue(23);
        else if (currentArea.AreaType == AreaType.BossAttack)
            DialogueSystem.Instance.AddDialogue(6);
        else
            DialogueSystem.Instance.AddDialogue(15);

        StartCoroutine(UnloadLastArea());
        StartCoroutine(LoadNextArea());
    }

    private IEnumerator LoadNextArea()
    {
        yield return null;

        if (currentArea != null)
            currentArea.gameObject.SetActive(true);

        nextAreaLoaded = true;
    }

    /// <summary>
    /// Destroys the last area while loading the next one
    /// </summary>
    /// <returns></returns>
    private IEnumerator UnloadLastArea()
    {
        yield return null;

        // Destroys the parent of the area
        if (lastArea != null)
        {
            lastArea.gameObject.SetActive(false);

            foreach(Transform obj in lastArea.objects)
            {
                Destroy(obj.gameObject);
            }
        }

        lastAreaDestroyed = true;
    }

    /// <summary>
    /// Transitions between areas
    /// </summary>
    /// <returns></returns>
    private IEnumerator TransitionAreas()
    {
		nextAreaLoaded = false;
		lastAreaDestroyed = false;

		float currentTime = Time.time;

		float time = MIN_TRAVEL_TIME + (areaIndex == 0 ? 5 : 0) - Mathf.Min(Time.time - startTravelTime, 0);

		yield return new WaitForSeconds(time);

		AreaLoaded?.Invoke();

        currentArea.LoadArea(true);

        yield return null;


        Transform playerSpawn = currentArea.FindSafeSpawn("PlayerSpawn");

        //GameManager.Instance.Player.gameObject.SetActive(false);
        GameManager.Instance.Player.transform.position = playerSpawn.position;
		GameManager.Instance.Player.transform.rotation = playerSpawn.rotation;
        //GameManager.Instance.Player.gameObject.SetActive(true);

        SkyboxManager.Instance.SwitchToSkybox(currentArea.Skybox);
		GameManager.Instance.SwitchState(GameState.BATTLE);
    }

    /// <summary>
    /// Adds object to the area
    /// </summary>
    /// <param name="gObject">The object that is needed to be added to the area</param>
    /// <param name="enemy">Whether it's an enemy or not</param>
    public void OnObjectAdd(GameObject gObject, bool enemy = false)
    {
        if (enemy)
        {
            gObject.transform.parent = currentArea.enemies;

            if (gObject.TryGetComponent(out EnemyController enemyController))
            {
                enemyController.Spawn = gObject.transform.position;
                enemyController.SpawnDestination = gObject.transform.position;
                enemyController.waypoints = currentArea.Waypoints;
            }
            else if (gObject.TryGetComponent(out Flock flock))
            {
                flock.SetSpawnpoint = gObject.transform.position;
                flock.SetSpawnDestination = gObject.transform.position;
                flock.WayPoints = currentArea.Waypoints;
            }
        }
        else
        {
            gObject.transform.parent = currentArea.objects;
        }
    }

    /// <summary>
    /// Kills all enemeis in the area
    /// </summary>
    public void KillEnemies()
    {
        foreach(Transform enemy in currentArea.enemies)
        {
            HealthAndShields healthAndShields = enemy.GetComponentInChildren<HealthAndShields>();

            if (healthAndShields)
                healthAndShields.TakeDamage(Mathf.Infinity, Mathf.Infinity);
        }
    }

    public Area CurrentArea { get { return currentArea; } }
	  public int AreaSize { get { return currentArea.Size; } }
    public bool EnemiesDead { get { return currentArea.enemies.childCount == 0; } }
    public int EnemyCount { get { return currentArea.enemies.childCount; } }
    public Vector3 FindRandomPosition { get { return currentArea.transform.position + Random.insideUnitSphere * currentArea.Size; } }
    public Vector3 AreaLocation { get { return currentArea.transform.position; } }
    public bool Boosting { private get; set; }
}
