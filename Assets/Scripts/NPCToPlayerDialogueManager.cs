using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPCToPlayerDialogueManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;
    public GameObject dialogueButtonPanel;
    public GameObject QuestPanel;
    public TextMeshProUGUI questTextBox;

    [Header("Quest")]
    public string questDescription;
    public TextMeshProUGUI clickToContinueText;

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
    private bool createChoices = false;

    void Start()
    {
        dialoguePanel.SetActive(false);
        dialogueButtonPanel.SetActive(false);
        QuestPanel.SetActive(false);
    }

    public void StartDialogue()
    {
        dialogueButtonPanel.SetActive(false);
        QuestPanel.SetActive(false);

        dialoguePanel.SetActive(true);
        ShowNode(0);
    }

public void ClickDialogueButton(string buttonText)
    {
        dialogueButtonPanel.SetActive(true);
        string dialogueButtonText = "Click " + buttonText + " to continue";
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        createChoices = false;
        typingCoroutine = StartCoroutine(TypeLine(clickToContinueText, dialogueButtonText));
    
    }
public void StartQuest()
    {
        dialogueButtonPanel.SetActive(false);
        dialoguePanel.SetActive(false);
        QuestPanel.SetActive(true);
        questTextBox.text = "Quest: " + questDescription;
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
        createChoices = true;
        typingCoroutine = StartCoroutine(TypeLine(dialogueText, node.dialogueText));
        
    }

    IEnumerator TypeLine(TextMeshProUGUI textComponent, string line)
    {
        textComponent.text = "";

        foreach (char c in line)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        if (createChoices)CreateChoices();
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
