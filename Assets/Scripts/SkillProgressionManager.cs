using UnityEngine;
using System.Collections;
public class SkillProgressionManager : MonoBehaviour
{
    public GameObject skillMetricPanel;
    public SkillManager skillManager;
    public GameObject skillUIPanel;
    public GameObject titlePanel;
    //public int totalSkillPoints = 0; // Total skill points accumulated by the player if 15 -> next level
    public string skillType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SkillTriggerObject"))
        {   skillUIPanel.SetActive(true);
            titlePanel.SetActive(true);
            new WaitForSeconds(1.5f); // Wait for 1.5 seconds
            // Update the skill points display
            skillMetricPanel.SetActive(true);
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
        yield return new WaitForSeconds(3f); // Wait for 3 seconds
        skillMetricPanel.SetActive(false);
        yield return new WaitForSeconds(3f);
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
}