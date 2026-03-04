using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/CharacterData/PlayerData")]
public class PlayerData : CharacterData
{
    [Header("Player Stat")]
    public int loopCount; // 루프 횟수에 따라서 클래스 해금
    
    // 이외에 활동 내역 등등. 다양한 세이브 정보들
    // 이건 json으로 해야하는구나...
}
