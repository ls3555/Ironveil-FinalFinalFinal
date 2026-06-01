using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [Header("Dialogue")]
    [TextArea(2, 5)]
    public string[] lines;

    [Header("Typing Settings")]
    public float typeSpeed = 0.04f;

    [Header("Timing")]
    public float minimumReadTime = 1.5f;
    public float delayBetweenLines = 0.5f;

    // Average reading speed
    private const float WORDS_PER_MINUTE = 200f;

    private Coroutine dialogueCoroutine;
    private bool isRunning = false;

    void Start()
    {
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    public void StartDialogue()
    {
        if (isRunning)
            return;

        isRunning = true;

        dialoguePanel.SetActive(true);
        dialogueText.text = "";

        dialogueCoroutine = StartCoroutine(RunDialogue());
    }

    public void EndDialogue()
    {
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
            dialogueCoroutine = null;
        }

        isRunning = false;

        dialogueText.text = "";

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    IEnumerator RunDialogue()
    {
        foreach (string line in lines)
        {
            // Clear old text
            dialogueText.text = "";

            // Type current line
            yield return StartCoroutine(TypeLine(line));

            // Calculate reading time
            int wordCount = line.Split(' ').Length;

            float readTime =
                Mathf.Max(
                    (wordCount / WORDS_PER_MINUTE) * 60f,
                    minimumReadTime
                );

            // Wait for player to read
            yield return new WaitForSeconds(readTime);

            // Clear text before next line
            dialogueText.text = "";

            yield return new WaitForSeconds(delayBetweenLines);
        }

        EndDialogue();
    }

    IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }
}