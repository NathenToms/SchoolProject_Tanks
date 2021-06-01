using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    public static VideoManager Instance = null;

    [SerializeField] private VideoClip victoryVideo;
    [SerializeField] private VideoClip gameoverVideo;
    [SerializeField] private RectTransform videoPanel;
    [SerializeField] private float duration = 10f;

    private VideoPlayer videoPlayer;
    private Camera camera;

    private bool playing;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;

        TryGetComponent(out videoPlayer);

        TryGetComponent(out camera);

        camera.enabled = false;
        videoPanel.gameObject.SetActive(false);
        videoPlayer.loopPointReached += OnVideoEnded;
    }

    private void OnVideoEnded(VideoPlayer source)
    {
        videoPlayer.Stop();
        playing = false;
        videoPanel.gameObject.SetActive(false);
        videoPlayer.clip = null;
        camera.enabled = false;
    }

    private IEnumerator StopVideo()
    {
        yield return new WaitForSeconds(duration);

        OnVideoEnded(null);
    }

    public void PlayVideo(SNSSTypes.VideoType type)
    {
        switch (type)
        {
            case SNSSTypes.VideoType.VICTORY:
                videoPlayer.clip = victoryVideo;
                break;
            case SNSSTypes.VideoType.GAME_OVER:
                videoPlayer.clip = gameoverVideo;
                break;
        }

        videoPlayer.Play();
        videoPanel.gameObject.SetActive(true);
        camera.enabled = true;
        playing = true;

        StartCoroutine(StopVideo());
    }

    public bool IsPlaying { get { return playing; } }
}
