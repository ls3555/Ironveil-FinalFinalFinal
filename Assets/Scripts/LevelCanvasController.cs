using UnityEngine;

public class LevelCanvasController : MonoBehaviour
{
    [Header("Canvases")]
    public GameObject level1Canvas;   // already active at start
    public GameObject level2Canvas;   // hidden until L1 maxed
    public GameObject level3Canvas;   // hidden until L2 maxed

    void Start()
    {
        // Level 1 stays on, 2 and 3 start hidden
        if (level1Canvas) level1Canvas.SetActive(true);
        if (level2Canvas) level2Canvas.SetActive(false);
        if (level3Canvas) level3Canvas.SetActive(false);

        SkillManager.Instance.OnGateUnlocked += OnGateUnlocked;
    }

    void OnDestroy()
    {
        if (SkillManager.Instance == null) return;
        SkillManager.Instance.OnGateUnlocked -= OnGateUnlocked;
    }

    void OnGateUnlocked()
    {
        // Check what level all skills are now on AFTER the gate fired
        Skill s1 = SkillManager.Instance.GetSkill("S1");

        if (s1.currentLevel == 1)
        {
            // Gate just moved everyone from L0 → L1, open Level 2 Canvas
            if (level2Canvas) level2Canvas.SetActive(true);
        }
        else if (s1.currentLevel == 2)
        {
            // Gate just moved everyone from L1 → L2, open Level 3 Canvas
            if (level3Canvas) level3Canvas.SetActive(true);
        }
    }
}