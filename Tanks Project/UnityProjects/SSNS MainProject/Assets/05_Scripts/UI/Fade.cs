using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Allows easing fading in and out for the CanvasGroup component
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class Fade : MonoBehaviour
{
    // If this is not 0 then when shown it only shows for the amount of time given
    [SerializeField] private float TIME_TO_SHOW;

    private CanvasGroup group;

    private bool show;

    private float timeShown;

    /// <summary>
    /// Enables or disables the fading
    /// </summary>
    /// <param name="active">Whether to fade or not</param>
    public void FadeActive(bool active)
    {
        this.show = active;

        gameObject.SetActive(true);
    }

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 0;

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (show) 
        {
            if (group.alpha < 1) // Shows the group
            {
                group.alpha += Time.deltaTime;

                if (group.alpha >= 1)
                {
                    group.alpha = 1;
                }
            }
            else if (TIME_TO_SHOW > 0) // Only shows group for limited time before hiding again
            {
                timeShown += Time.deltaTime;

                if (timeShown >= TIME_TO_SHOW)
                {
                    timeShown = 0;
                    show = false;
                }
            }
        }
        else if (!show && group.alpha > 0) // Hides the group
        {
            group.alpha -= Time.deltaTime;

            if (group.alpha <= 0)
            {
                group.alpha = 0;
                gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Returns whether this is shown
    /// </summary>
    public bool Shown { get { return gameObject.activeSelf; } }
}
