using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(AudioSource))]
public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance;

    [SerializeField] AudioSource audioSource;
    [SerializeField] Text dialogueText;

    Queue<DialogueClass> dialogueQueue = new Queue<DialogueClass>();

    public List<DialogueClass> Dialogue;

    // Editor
    public DialogueClass NewDialogue;
    public Texture2D SoundIcon;
    public bool ShowDefaultEditor;

    private int textPos = 0;

    private bool dialoguePaused;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;
    }

    [ContextMenu("Play Index Zero")]
    public void PlayIndexZero()
    {
        AddDialogue(0);
    }

    // Start is called before the first frame update
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(TrackTextDisplay());
    }

    private void Update()
    {
        if (audioSource && dialogueQueue.Count > 0)
        {
            if (dialogueText) dialogueText.text = dialogueQueue.Peek().Text[textPos];

            if (!audioSource.isPlaying && textPos == 0)
            {
                audioSource.clip = dialogueQueue.Peek().SoundClip;
                audioSource.Play();
                //Debug.Log("play clip");
            }

            if (dialogueQueue.Peek().Text.Length == 1) dialogueQueue.Dequeue();
            else if (textPos == dialogueQueue.Peek().Text.Length - 1)
            {
                //Debug.Log("Reset");
                textPos = 0;
                dialogueQueue.Dequeue();
            }
        }
        else if (dialogueText.text != "" && !audioSource.isPlaying && dialogueQueue.Count == 0)
        {
            dialogueText.text = "";
        }

        if (GameManager.Instance.paused)
        {
            audioSource.Pause();

            dialoguePaused = true;
        }
        else if (dialoguePaused)
        {
            audioSource.UnPause();

            dialoguePaused = false;
        }
    }

    IEnumerator TrackTextDisplay()
    {
        while (true)
        {
            if (dialoguePaused)
            {
                yield return null;
                continue;
            }

            if (dialogueQueue.Count > 0)
            {
                DialogueClass diag = dialogueQueue.Peek();

                if (diag.displayTime != null && diag.Text.Length > 1 && textPos < diag.displayTime.Length)
                {
                    yield return new WaitForSecondsRealtime(diag.displayTime[textPos]);
                    textPos++;
                    //Debug.Log(textPos);
                }
                else yield return null;
            }
            else yield return null;
        }
    }

    public void AddDialogue(DialogueClass dialogue)
    {
        Debug.Log("Dialogue added");
        dialogueQueue.Enqueue(dialogue);
    }

    public DialogueClass AddDialogue(int listIndex)
    {
        Debug.Log("Dialogue added");
        if (listIndex < Dialogue.Count)
        {
            foreach (DialogueClass dialogue in Dialogue)
            {
                if (dialogue.Index == listIndex)
                {
                    DialogueClass pickedDialogue = dialogue;
                    if (dialogue.alternatives != null && dialogue.alternatives.Length > 0)
                    {
                        int randomIndex = Random.Range(0, dialogue.alternatives.Length + 1);

                        if (randomIndex > 0)
                            pickedDialogue = Dialogue[dialogue.alternatives[randomIndex - 1]];
                    }

                    dialogueQueue.Enqueue(pickedDialogue);
                    return pickedDialogue;
                }
            }
        }
        else
        {
            Debug.LogError("Index out of range.");
        }

        return null;
    }

    public void PlayQuickClip(AudioClip clip)
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void GenerateNewDialogue()
    {
        NewDialogue.Index = Dialogue.Count;
        Dialogue.Add(NewDialogue);
        NewDialogue = new DialogueClass();
    }

    public void ClearDialogue()
    {
        dialogueQueue.Clear();
        audioSource.Stop();
    }
}
