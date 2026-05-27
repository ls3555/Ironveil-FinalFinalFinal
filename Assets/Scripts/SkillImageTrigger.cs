using UnityEngine;

public class SkillImageController : MonoBehaviour
{
    [Header("Which level does this controller watch for?")]
    public int pointThreshold = 5;   // L0=5, L1=10, L2=15

    [Header("Skill Images")]
    public GameObject s1Image;
    public GameObject s2Image;
    public GameObject s3Image;

    [Header("Level Full Image — shows when all 3 hit the threshold")]
    public GameObject levelFullImage;

    bool s1Maxed = false;
    bool s2Maxed = false;
    bool s3Maxed = false;

    void Start()
    {
        SetAllInactive();
        SkillManager.Instance.OnSkillUpdated += OnSkillUpdated;
    }

    void OnDestroy()
    {
        if (SkillManager.Instance == null) return;
        SkillManager.Instance.OnSkillUpdated -= OnSkillUpdated;
    }

    void OnSkillUpdated(Skill s)
    {
        if (s.pointsInLevel >= pointThreshold)
        {
            if (s.skillId == "S1" && !s1Maxed)
            {
                s1Maxed = true;
                if (s1Image) s1Image.SetActive(true);
            }
            else if (s.skillId == "S2" && !s2Maxed)
            {
                s2Maxed = true;
                if (s2Image) s2Image.SetActive(true);
            }
            else if (s.skillId == "S3" && !s3Maxed)
            {
                s3Maxed = true;
                if (s3Image) s3Image.SetActive(true);
            }
        }

        if (s1Maxed && s2Maxed && s3Maxed)
            if (levelFullImage) levelFullImage.SetActive(true);
    }

    void SetAllInactive()
    {
        if (s1Image)        s1Image.SetActive(false);
        if (s2Image)        s2Image.SetActive(false);
        if (s3Image)        s3Image.SetActive(false);
        if (levelFullImage) levelFullImage.SetActive(false);
    }

    public void Reset()
    {
        s1Maxed = false;
        s2Maxed = false;
        s3Maxed = false;
        SetAllInactive();
    }
}