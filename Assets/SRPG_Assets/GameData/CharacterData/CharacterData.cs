using UnityEngine;

public class CharacterData : ScriptableObject
{
    public int speed; // 속도. 행동 순서 결정 (높을수록 빠르다)
    public int movement; // 이동력. 이동 가능 범위 결정
    public int attack; // 공격력
    public int defense; // 방어력
    public int HP; // 체력
}
