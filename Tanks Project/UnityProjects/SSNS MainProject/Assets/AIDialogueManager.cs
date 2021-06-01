using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDialogueManager : MonoBehaviour
{
    [SerializeField] NumberRange dialogueRange;
    [SerializeField] NumberRange timeSeconds;
    // Start is called before the first frame update
    void Start()
    {
        if (timeSeconds.min >= timeSeconds.max) timeSeconds.min = timeSeconds.max / 2;

        StartCoroutine(RandomizeDialogue());
    }

    IEnumerator RandomizeDialogue()
    {
        while (true)
        {
            float rand = Random.Range(timeSeconds.min, timeSeconds.max);

            yield return new WaitForSecondsRealtime(rand);

            int index = Random.Range((int)dialogueRange.min, (int)dialogueRange.max + 1);
            DialogueSystem.Instance.AddDialogue(index);
        }
    }
}