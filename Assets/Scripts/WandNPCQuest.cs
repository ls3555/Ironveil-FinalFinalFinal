using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
//On NPC
public class WandNPCQuest : MonoBehaviour
{

    [SerializeField] private GameObject sleepingObject;// wand
    [SerializeField] private Rigidbody2D sleepingRb;
    [SerializeField] private SpriteRenderer sleepingSr;
    [SerializeField] private  GameObject npc;
    [SerializeField] private SpriteRenderer weaponSprite;
    [SerializeField] private GameObject breakableObject;// barrel
    [SerializeField] private Rigidbody2D skillTriggerObject; // statue rigidbody
    //public GameObject wandPanel;
    private bool objectBroken = false;
    [SerializeField] private string requiredTag = "NPCItem";
    [SerializeField] private WandFloat wandFloat;
    [SerializeField] private NPCToPlayerDialogueManager dialogueManager;
    [SerializeField] private NPCMovement npcMovement;

private IEnumerator WakeStatueAfterDelay()
{
    yield return new WaitForSeconds(4f);
    dialogueManager.StartDialogue();
    yield return new WaitForSeconds(4f);
    skillTriggerObject.WakeUp();
    npcMovement.DisableDialogueTrigger();
    Destroy(sleepingObject);
}

    private void Start()
    {
      if (sleepingObject != null) {
            sleepingRb.Sleep();
            sleepingSr.sortingOrder = 1; // Set the sorting layer ID to 1
            sleepingSr.sortingLayerName = "Layer 1"; 
            }
        objectBroken = false;
        weaponSprite.enabled = false; // Hide weapon at the start
        //wandPanel.SetActive(false);
        npcMovement.TurnOff();
        skillTriggerObject.Sleep();  
        if (breakableObject != null)
    {
        var breakable = breakableObject.GetComponent<BreakableObject>();
        if (breakable != null)
        {
            breakable.OnBroken += HandleObjectBroken;
            Debug.Log("Successfully subscribed to OnBroken"); // ← fires?
        }
        else
            Debug.LogError("BreakableObject script not found on " + breakableObject.name);
    }
    else
        Debug.LogError("breakableObject is not assigned in Inspector!");
}

public void HandleObjectBroken()
{
    Debug.Log("HandleObjectBroken called!");
        if(objectBroken) return;
        objectBroken = true;

        sleepingRb.WakeUp();
        sleepingSr.sortingOrder = 2;
        sleepingSr.sortingLayerName = "Layer 2";  // put wand visible on scree
                    //TO-DO : Wand navs to npc location using WandFloat script
        wandFloat.SetTarget(npc.transform);


    }

    //For when wand and npc interact
     private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(requiredTag))
        {
            //Wand and NPC interacted
            sleepingSr.enabled = false;
            weaponSprite.enabled = true; // Show weapon
            npcMovement.TurnOn();
            StartCoroutine(WakeStatueAfterDelay());

            
        }
    }
   
    private void OnDestroy()
    {
        if (breakableObject != null)
        {
            var breakable = breakableObject.GetComponent<BreakableObject>();
            if (breakable != null)
                breakable.OnBroken -= HandleObjectBroken;
        }
    }
}
