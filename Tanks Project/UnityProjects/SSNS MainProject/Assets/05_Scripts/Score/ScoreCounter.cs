using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour
{
	[SerializeField] private ScoreDisplay scoreDisplay;

	[SerializeField] static int Score;
    [SerializeField] int thisScore;

    enum TypeOfHit { ShieldHit, DestroyHit };
    [SerializeField] TypeOfHit typeOfHit;

    [Header("Only Include if Neccessary")]
    [Tooltip("The UI element for the player text, only is used if tag is player")]
    [SerializeField] Text playerText;
    bool isPlayer = false;
    bool hitThisFrame = false;

    // Start is called before the first frame update
    void Start()
    {
		scoreDisplay = GameObject.FindObjectOfType<ScoreDisplay>();

		Score = 0;
    
        if (gameObject.tag == "Player")
        {
            isPlayer = true;
        }
    }

    private void Update()
    {
        //if (isPlayer)
        //{
        //    playerText.text = "Score: " + Score;
        //}
    }


    /// <summary>
    /// Only call with null go if it's the laser shooting
    /// </summary>
    /// <param name="thingShot"></param>
    public void Hit(GameObject thingShot = null)
    {
        if (thingShot == null)
        {
            if (typeOfHit == TypeOfHit.ShieldHit)
			{
				//if (scoreDisplay) scoreDisplay.AddScore(thisScore);

				Score += thisScore;
                scoreDisplay.SetScore(Score);

            }
        }
        else if (thingShot.TryGetComponent(out ShotThing st))
        {
            if (st.whoSent == ShotThing.shotFrom.Player && typeOfHit == TypeOfHit.ShieldHit)
            {
                //if (scoreDisplay)
                //scoreDisplay.AddScore(thisScore);
           
                Score += thisScore;
                scoreDisplay.SetScore(Score);
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        Hit(collision.gameObject);
    }

    private void OnDestroy()
    {
        if (typeOfHit == TypeOfHit.DestroyHit)
        {
            Score += thisScore;
            scoreDisplay.SetScore(Score);
        }
    }
}
