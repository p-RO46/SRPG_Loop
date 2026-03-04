using System.Collections.Generic;
using UnityEngine;
public enum E_AttackElement
{
    HOLY, // (신성)
    CHAOS, // (혼돈)
    BLOOD, // (적혈)
    SPIRIT, // (영혼)
    COSMIC, // (초월)
}

public enum E_AttackType
{
    PHYSICAL, // (물리)
    MAGICAL, // (마법)
    PSYCHIC, // (정신)
}

public enum E_Debuff
{
    NONE,
    DARKNESS, // (어둠)이동 범위 감소
    BURN, // (화상)도트뎀
    FEAR, // (공포)공격력 감소
    INSANITY, // (광기)방어력 감소, 공격력 증가
    CONFUSION, // (혼란)자신 공격
}

public class Skill
{
    public SkillData skillData;

    public E_AttackElement AE;
    public E_AttackType AT;
    public E_Debuff DF;

    public int damage = 1; // 데미지 배율

    public int[,] attackRange; // 정사각

    public Skill(SkillData _skillData)
    {
        skillData = _skillData;

        AE = skillData.attackElement;
        AT = skillData.attackType;
        DF = skillData.debuff;
        damage = skillData.damage;
        SetRange(skillData.skillRangeCSV);
    }

    public int[,] Rotate_L() // 반시계 방향으로 회전
    {
        int len0 = attackRange.GetLength(0);
        int len1 = attackRange.GetLength(1);

        int[,] _range = new int[len0, len1];
        int[,] temp = (int[,])attackRange.Clone();

        for (int i = 0; i < len0; i++)
        {
            for (int j = 0; j < len1; j++)
            {
                _range[i, j] = temp[j, len0 - 1 - i];
            }
        }

        attackRange = _range;
        return _range;
    }

    public int[,] Rotate_R() // 시계 방향으로 회전
    {
        int len0 = attackRange.GetLength(0);
        int len1 = attackRange.GetLength(1);

        int[,] _range = new int[len0, len1];
        int[,] temp = (int[,])attackRange.Clone();

        for (int i = 0; i < len0; i++)
        {
            for (int j = 0; j < len1; j++)
            {
                _range[i, j] = temp[len1 - 1 - j, i];
            }
        }

        attackRange = _range;
        return _range;
    }

    private void SetRange(TextAsset skillRangeCSV)
    {
        List<List<int>> _attackRange = new List<List<int>>();
        string[] lines = skillRangeCSV.text.Split("\n");
        for (int i = 0; i < lines.Length; i++)
        {
            _attackRange.Add(new List<int>());
            string[] words = lines[i].Split(",");
            for (int j = 0; j < words.Length; j++)
            {
                _attackRange[i].Add(int.Parse(words[j]));
            }
        }

        int len = _attackRange.Count;
        attackRange = new int[len, len];
        for (int i = 0; i < len; i++)
        {
            for (int j = 0; j < len; j++)
            {
                attackRange[i, j] = _attackRange[i][j];
            }
        }
    }
}
