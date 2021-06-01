using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private float timeToRespawn;
    [SerializeField] private GameObject deathCameraPrefab;
    [SerializeField] private RectTransform hud;
    [SerializeField] private GameObject lifePrefab;

    private Transform lifeHUD;

    private const int NUMBER_OF_LIFES = 5;

    private int currentLifes;

    private ShipController player;

    private GameObject camera;

    public void Respawn()
    {
        if (currentLifes == 0)
        {
            GameManager.Instance.SwitchState(SNSSTypes.GameState.GAME_OVER);
        }
        else
        {
            Transform playerSpawn = AreaManager.Instance.CurrentArea.FindSafeSpawn("PlayerSpawn");

            player.transform.position = playerSpawn.position;
            player.transform.rotation = playerSpawn.rotation;

            if (player.TryGetComponent(out HealthAndShields health))
            {
                health.ResetValues(true);
            }

            AreaManager.Instance.CurrentArea.RespawnArea();

            player.gameObject.SetActive(true);

            Destroy(camera);

            hud.gameObject.SetActive(true);

            GameManager.Instance.SwitchState(SNSSTypes.GameState.BATTLE);
        }
    }

    private void OnDeath()
    {
        camera = Instantiate(deathCameraPrefab);
        camera.transform.position = transform.position + Quaternion.LookRotation(transform.forward) * player.Behaviour.deathPos;
        camera.transform.LookAt(transform.position);

        camera.transform.rotation = Quaternion.Euler(camera.transform.eulerAngles.x, camera.transform.eulerAngles.y, transform.eulerAngles.z);

        hud.gameObject.SetActive(false);

        currentLifes--;

        Destroy(lifeHUD.transform.GetChild(lifeHUD.transform.childCount - 1).gameObject);
    }

    private void Start()
    {
        TryGetComponent(out player);

        if (TryGetComponent(out HealthAndShields health))
        {
            health.onDeath += OnDeath;
        }

        lifeHUD = hud.Find("Player Lives");

        for (int i = 0; i < NUMBER_OF_LIFES; i++)
        {
            Instantiate(lifePrefab, lifeHUD);
        }

        currentLifes = NUMBER_OF_LIFES;
    }
}
