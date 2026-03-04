using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class SkillData : ScriptableObject
{
    [Header("Skill Description")]
    public string description;

    [Header("Skill Type")]
    public E_AttackElement attackElement;
    public E_AttackType attackType;
    public E_Debuff debuff;

    [Header("Damage Scale")]
    public int damage; // 데미지 배율

    [Header("Skill Range")]
    public TextAsset skillRangeCSV = null; // csv 파일로 저장
    public int[,] skillRange;

    // 인스펙터에서 값이 수정될 때마다 호출됨
    private void OnValidate()
    {
        if (skillRangeCSV != null)
        {
            SetRange();
        }
    }

    private void SetRange()
    {
        List<List<int>> _skillRange = new List<List<int>>();
        string[] lines = skillRangeCSV.text.Split("\n");
        for (int i = 0; i < lines.Length; i++)
        {
            _skillRange.Add(new List<int>());
            string[] words = lines[i].Split(",");
            for (int j = 0; j < words.Length; j++)
            {
                _skillRange[i].Add(int.Parse(words[j]));
            }
        }

        int len = _skillRange.Count;
        skillRange = new int[len, len];
        for (int i = 0; i < len; i++)
        {
            for (int j = 0; j < len; j++)
            {
                skillRange[i, j] = _skillRange[i][j];
            }
        }
    }
}
