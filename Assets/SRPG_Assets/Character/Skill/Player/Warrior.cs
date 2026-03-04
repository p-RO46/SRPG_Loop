public class Warrior : PlayerClass
{
    public Warrior(Player _player) : base(_player)
    {
        skillData = player.playerSkill.WarriorSkillData;
        skillCount = skillData.Count;
        skillRange = skillData[0].skillRange;
    }
}
