public class Mage : PlayerClass
{
    public Mage(Player _player) : base(_player)
    {
        skillData = player.playerSkill.MageSkillData;
        skillCount = skillData.Count;
        //skillRange = skillData[0].skillRange;
    }
}