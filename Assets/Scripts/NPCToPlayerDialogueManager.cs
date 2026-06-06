using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPCToPlayerDialogueManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;

    [SerializeField] private GameObject dialogueButtonPanel;
    [SerializeField] private GameObject QuestPanel;
    [SerializeField] private TextMeshProUGUI questTextBox;

    [Header("Quest")]
    [SerializeField] private string questDescription;
    [SerializeField] private TextMeshProUGUI clickToContinueText;

    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Choices")]
    [SerializeField] private Transform choiceContainer;
    [SerializeField] private Button choiceButtonPrefab;

    [Header("Dialogue")]
    [SerializeField] private List<DialogueNode> dialogueNodes;

    [Header("Typing")]
    [SerializeField] private float typeSpeed = 0.03f;

    private int currentNodeIndex = 0;
    private Coroutine typingCoroutine;
    ///private bool createChoices = false;
    
    void Start()
    {
        SetPanels(false, dialoguePanel, dialogueButtonPanel, QuestPanel);
    }

    private void SetPanels(bool active, params GameObject[] panels)
    { 
        foreach(GameObject panel in panels)
        {
            if (panel != null) panel.SetActive(active);
        }
    }
    public void CloseAllPanels()
    {
        SetPanels(false, dialoguePanel, dialogueButtonPanel, QuestPanel);
    }   

    public void StartDialogue()
    {
        Debug.Log("Dialogue triggered by ");
        SetPanels(false, dialogueButtonPanel, QuestPanel);
        dialoguePanel.SetActive(true);
        ShowNode(0);
    }

public void ClickDialogueButton(string buttonText)
    {
        SetPanels(true, dialogueButtonPanel);
        string dialogueButtonText = "Click " + buttonText + " to continue";
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeLine(clickToContinueText, dialogueButtonText));
    
    }
public void StartQuest()
    {
        SetPanels(false, dialogueButtonPanel, dialoguePanel);
        SetPanels(true, QuestPanel);
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
        yield return new WaitForSeconds(typeSpeed+2f);
        
        CreateChoices();
    }

    void CreateChoices()
    {
        DialogueNode node = dialogueNodes[currentNodeIndex];

        if (node.endsDialogue)
        {
            if(node.createChoices)
            {
            Button endButton = Instantiate(choiceButtonPrefab, choiceContainer);
            endButton.GetComponentInChildren<TextMeshProUGUI>().text = "Alright";
            endButton.onClick.AddListener(EndDialogue);
            }
            else
            {
                EndDialogue();
            }

            return;
        }
        if(node.choices.Count == 0)
        {
                ShowNode(currentNodeIndex + 1);
        }
        else{

            foreach (DialogueChoice choice in node.choices)
            {
                Button button = Instantiate(choiceButtonPrefab, choiceContainer);
                button.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;
                int nextIndex = choice.nextNodeIndex;
                button.onClick.AddListener(() => ShowNode(nextIndex));
            }
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
