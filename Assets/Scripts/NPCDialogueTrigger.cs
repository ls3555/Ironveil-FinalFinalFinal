using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    public NPCToPlayerDialogueManager dialogueManager;
    private bool hasVisited = false;
    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!hasVisited)
            {
                dialogueManager.StartDialogue();
                hasVisited = true;
            }
            else
            {
                dialogueManager.StartQuest();
            }
            
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            dialogueManager.ClickDialogueButton("E");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            dialogueManager.dialogueButtonPanel.SetActive(false);
            dialogueManager.dialoguePanel.SetActive(false);
            dialogueManager.QuestPanel.SetActive(false);
        }
    }
}
