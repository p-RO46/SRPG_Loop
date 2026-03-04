using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSkill", menuName = "Scriptable Objects/PlayerSkill")]
public class PlayerSkill : ScriptableObject
{
    [Header("Warrior Skill")]
    public List<SkillData> WarriorSkillData;

    [Header("Mage Skill")]
    public List<SkillData> MageSkillData;
}
