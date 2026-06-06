using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
//On NPC
public class WandNPCQuest : MonoBehaviour
{

    [SerializeField] private GameObject sleepingObject;// wand
    private Rigidbody2D sleepingRb;
    private SpriteRenderer sleepingSr;
    [SerializeField] private  GameObject npc;
    [SerializeField] private SpriteRenderer weaponSprite;
    [SerializeField] private GameObject breakableObject;// barrel
    [SerializeField] private Rigidbody2D skillTriggerObject; // statue rigidbody
    public GameObject wandPanel;
    private bool objectBroken = false;
    [SerializeField] private string requiredTag = "NPCItem";
    [SerializeField] private WandFloat wandFloat;
    [SerializeField] private NPCMoveToPlayer npcMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created


void Awake()
{
    sleepingRb = sleepingObject.GetComponent<Rigidbody2D>();
    sleepingSr = sleepingObject.GetComponent<SpriteRenderer>();
    skillTriggerObject.Sleep();
}

private IEnumerator WakeStatueAfterDelay()
{
    yield return new WaitForSeconds(4f);
    wandPanel.SetActive(false);
    skillTriggerObject.WakeUp();
    //Destroy(sleepingObject);
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
        wandPanel.SetActive(false);

    }  

    void Update()
    {
        if (breakableObject == null)
        {
            Debug.Log("Target was hit and destroyed!");
            objectBroken = true;
            if(objectBroken)
            {
                sleepingRb.WakeUp();
                sleepingSr.sortingOrder = 2;
                sleepingSr.sortingLayerName = "Layer 3";  // put wand visible on scree
                //TO-DO : Wand navs to npc location using WandFloat script
                wandFloat.SetTarget(npc.transform);

            }
        }
    }

    //For when wand and npc interact
     private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(requiredTag))
        {
            //Wand and NPC interacted
            sleepingSr.enabled = false;
            weaponSprite.enabled = true; // Show weapon
            //npcMovement.StartMoving();


            wandPanel.SetActive(true); // Tell Player what she's doing
            StartCoroutine(WakeStatueAfterDelay());

            
        }
    }
   
}
