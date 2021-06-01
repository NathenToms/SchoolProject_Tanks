using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeMusic : MonoBehaviour
{
    [SerializeField] float changeDelay = 15f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Randomize());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Randomize()
    {
        while (true)
        {
            yield return new WaitForSeconds(changeDelay);

            int rand = Random.Range(0, 3);

            switch (rand)
            {
                case 0:
                    MusicManager.instance.RandomTrack(SNSSTypes.MusicTrackType.MENU);
                    break;
                case 1:
                    MusicManager.instance.RandomTrack(SNSSTypes.MusicTrackType.NON_COMBAT);
                    break;
                case 2:
                    MusicManager.instance.RandomTrack(SNSSTypes.MusicTrackType.COMBAT);
                    break;
            }
        }
    }
}