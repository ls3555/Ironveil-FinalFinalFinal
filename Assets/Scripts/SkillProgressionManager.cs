using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class SkillProgressionManager : MonoBehaviour
{
    public GameObject skillMetricPanel;
    public SkillManager skillManager;
    public GameObject skillUIPanel; //
    public GameObject titlePanel;
    //public int totalSkillPoints = 0; // Total skill points accumulated by the player if 15 -> next level
    public string skillType;
    [SerializeField] private FadeOutEffect fadeOutEffect;
    [SerializeField] private CanvasGroup skillMetricCanvasGroup;
    [SerializeField] private CanvasGroup skillUICanvasGroup;
    [SerializeField] private CanvasGroup titleCanvasGroup;
    private List<CanvasGroup> canvasGroups;
    [SerializeField] public EnemyController enemy; // assign in Inspector

    [SerializeField] private Transform player;
    [SerializeField] private Vector3 uiOffset = new Vector3(0, 2f, 0);


private void Start()
    {
        canvasGroups = new List<CanvasGroup>() { skillMetricCanvasGroup, skillUICanvasGroup, titleCanvasGroup };
        
        if (enemy != null){
        enemy.OnEnemyDied += HandleEnemyDeath;
        }
    }  
     private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SkillTriggerObject"))
        {
            ResetCanvasGroups(); // Reset alpha values before starting the fade-in effect
            switch (skillType)
            {
                case "S1":
                    AddPointsToS1();// Points for the "Grit" skill
                    break;
                case "S2":
                    AddPointsToS2();// Points for the "Reasoning" skill
                    break;
                case "S3":
                    AddPointsToS3();// Points for the "Empathy" skill
                    break;
            }
            other.isTrigger = false; // Disable the trigger to prevent multiple activations
            StartCoroutine(DismissAfterDelay());
        }
    }
    private IEnumerator DismissAfterDelay( )
    { 
        yield return new WaitForSeconds(2f); // Wait for 2 seconds
        fadeOutEffect.FadeOutBackground(canvasGroups);
        yield return new WaitForSeconds(2f); // fade duration
        skillMetricPanel.SetActive(false);
        skillUIPanel.SetActive(false);
        titlePanel.SetActive(false);

    }
    public void AddPointsToS1()
    {
        skillManager.AddSkillPoints(5);
        skillManager.AddPointToSkill("S1");
    }
        public void AddPointsToS2()
    {
        skillManager.AddSkillPoints(5);
        skillManager.AddPointToSkill("S2");
    }
    public void AddPointsToS3()
    {
        skillManager.AddSkillPoints(5);
        skillManager.AddPointToSkill("S3");
 
    }
private void ResetCanvasGroups()
    {
        foreach (CanvasGroup canvasGroup in canvasGroups)
            {  
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
            skillUIPanel.SetActive(true);
            titlePanel.SetActive(true);
            skillMetricPanel.SetActive(true);
    }

    private void HandleEnemyDeath()
    {
        if (skillType != "S1")
            return;

        ResetCanvasGroups();
        
        AddPointsToS1();
        
        StartCoroutine(DismissAfterDelay());
    }
}