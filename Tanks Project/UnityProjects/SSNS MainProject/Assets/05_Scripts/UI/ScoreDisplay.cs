using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Text text;

    public void AddScore(int score)
    {
        int currentScore = int.Parse(text.text);
        int newScore = currentScore + score;

        text.text = newScore.ToString();

        animator.SetTrigger("AddScore");
    }

    public void SetScore(int score)
    {
        if (text != null)
        {
            text.text = score.ToString();

            animator.SetTrigger("AddScore");
        }
    }
}
