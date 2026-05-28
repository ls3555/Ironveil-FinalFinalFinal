using UnityEngine;

public class DebugAddPoints : MonoBehaviour
{


    public void AddPointsToS1()
    {
        SkillManager.Instance.AddSkillPoints(5);
        SkillManager.Instance.AddPointToSkill("S1");
    }
        public void AddPointsToS2()
    {
        SkillManager.Instance.AddSkillPoints(5);
        SkillManager.Instance.AddPointToSkill("S2");
    }
    public void AddPointsToS3()
    {
        SkillManager.Instance.AddSkillPoints(5);
        SkillManager.Instance.AddPointToSkill("S3");
    }
}