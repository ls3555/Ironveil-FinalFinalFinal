using System;
using UnityEngine;

[System.Serializable]
public class Skill
{
    public string skillId;
    public string displayName;
    public int currentLevel;      // 0, 1, 2, 3
    public int pointsInLevel;     // accumulated points at this level
    public int maxLevel = 3;

    public bool IsMaxed => currentLevel >= maxLevel;

    // Points required to fill each level before the gate fires
    // Index = currentLevel: L0 needs 5, L1 needs 10, L2 needs 15
    public static readonly int[] PointCapPerLevel = { 5, 10, 15 };

    public bool IsLevelFull =>
        !IsMaxed && pointsInLevel >= PointCapPerLevel[currentLevel];

    public int PointCapAtCurrentLevel =>
        IsMaxed ? 0 : PointCapPerLevel[currentLevel];
}

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    [Header("Skills")]
    public Skill[] skills = new Skill[]
    {
        new Skill { skillId = "S1", displayName = "Grit" },
        new Skill { skillId = "S2", displayName = "Reason"  },
        new Skill { skillId = "S3", displayName = "Empathy"    },
    };

    [Header("Skill Points")]
    public int skillPoints = 0;

    // Events
    public event Action<Skill>  OnSkillUpdated;       // any point or level change
    public event Action<int>    OnSkillPointsChanged;
    public event Action<int>    OnCharacterLevelUp;   // passes new character level
    public event Action         OnGateUnlocked;       // all skills just cleared a gate

    int characterLevel = 1;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddSkillPoints(int amount)
    {
        skillPoints += amount;
        OnSkillPointsChanged?.Invoke(skillPoints);
    }

    // Can the player spend 1 SP on this skill right now?
    public bool CanAddPoint(string skillId)
    {
        Skill s = GetSkill(skillId);
        if (s == null || s.IsMaxed) return false;
        if (skillPoints < 1)        return false;

        // Gate: only accept points if this skill's level is open
        // (i.e. the current level was just unlocked for this skill)
        return !s.IsLevelFull;
    }

    public void AddPointToSkill(string skillId)
    {
        if (!CanAddPoint(skillId)) return;

        Skill s = GetSkill(skillId);
        skillPoints-=5;
        s.pointsInLevel+=5;

        OnSkillPointsChanged?.Invoke(skillPoints);
        OnSkillUpdated?.Invoke(s);

        // Check if all skills are now full at this level -> open the gate
        CheckGate(s.currentLevel);
    }

    void CheckGate(int level)
    {
        // All skills must be full (or already past this level) to fire the gate
        foreach (var s in skills)
        {
            if (s.currentLevel < level) return;
            if (s.currentLevel == level && !s.IsLevelFull) return;
        }

        // Gate passed - advance all skills that are sitting at this level
        bool anyAdvanced = false;
        foreach (var s in skills)
        {
            if (s.currentLevel == level && !s.IsMaxed)
            {
                s.currentLevel++;
                anyAdvanced = true;
                OnSkillUpdated?.Invoke(s);
            }
        }

        if (anyAdvanced)
        {
            OnGateUnlocked?.Invoke();

            // Character level-up fires when all skills reach L3
            if (AreAllSkillsMaxed())
            {
                characterLevel++;
                OnCharacterLevelUp?.Invoke(characterLevel);
            }
        }
    }

    bool AreAllSkillsMaxed()
    {
        foreach (var s in skills)
            if (!s.IsMaxed) return false;
        return true;
    }

    public Skill GetSkill(string skillId)
    {
        foreach (var s in skills)
            if (s.skillId == skillId) return s;
        return null;
    }
}
