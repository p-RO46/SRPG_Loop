using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
public class GameData : ScriptableObject
{
    [Header("Player Data")]
    public PlayerData playerData;

    [Header("Enemy Data List")]
    public List<EnemyData> enemyData;
}
