using UnityEngine;

public class DevCheatScript : MonoBehaviour
{
    [Header("Keybindings")]
    public KeyCode addAthleticsSkillPointKey = KeyCode.L;

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = PlayerStats.Instance;

        if (playerStats == null)
        {
            Debug.LogError("PlayerStats not found in scene.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(addAthleticsSkillPointKey))
        {
            AddSkillPoint(SkillType.Athletics);
        }
    }

    private void AddSkillPoint(SkillType skill)
    {
        if (playerStats == null) return;

        SkillUsageTracker.RegisterSkillUse(skill, 9999f);

    }
}
