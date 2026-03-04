using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/CharacterData/EnemyData")]
public class EnemyData : CharacterData
{
    [Header("Enemy Stat")]
    public int level;

    // 이외에 활동 내역 등등. 다양한 세이브 정보들
}
