using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPCToPlayerDialogueManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;

    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;

    [Header("Choices")]
    public Transform choiceContainer;
    public Button choiceButtonPrefab;

    [Header("Dialogue")]
    public List<DialogueNode> dialogueNodes;

    [Header("Typing")]
    public float typeSpeed = 0.03f;

    private int currentNodeIndex = 0;
    private Coroutine typingCoroutine;

    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        ShowNode(0);
    }

    void ShowNode(int nodeIndex)
    {
        currentNodeIndex = nodeIndex;

        DialogueNode node = dialogueNodes[currentNodeIndex];

        ClearChoices();

        speakerNameText.text = node.speakerName;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeLine(node.dialogueText));
    }

    IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        CreateChoices();
    }

    void CreateChoices()
    {
        DialogueNode node = dialogueNodes[currentNodeIndex];

        if (node.endsDialogue)
        {
            Button endButton = Instantiate(choiceButtonPrefab, choiceContainer);

            endButton.GetComponentInChildren<TextMeshProUGUI>().text = "Alright";

            endButton.onClick.AddListener(EndDialogue);

            return;
        }

        foreach (DialogueChoice choice in node.choices)
        {
            Button button = Instantiate(choiceButtonPrefab, choiceContainer);

            button.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;

            int nextIndex = choice.nextNodeIndex;

            button.onClick.AddListener(() => ShowNode(nextIndex));
        }
    }

    void ClearChoices()
    {
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void EndDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        ClearChoices();

        dialogueText.text = "";
        speakerNameText.text = "";

        dialoguePanel.SetActive(false);
    }
}
