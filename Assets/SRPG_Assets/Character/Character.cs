using UnityEngine;
public enum E_CharacterType
{
    EMPTY = 0,
    PLAYER = 1,
    ENEMY = 2,
}

public class Character
{
    public E_CharacterType characterType;

    public int speed { get; protected set; } // 속도. 행동 순서 결정 (높을수록 빠르다)
    public int movement { get; protected set; } // 이동력. 이동 가능 범위 결정
    public int attack { get; protected set; }  // 공격력
    public int defense { get; protected set; } // 방어력
    public int HP { get; protected set; } // 체력
     
    // 배열 기준 좌표. 계속 변화함
    public int locX = 0;
    public int locY = 0;

    public Character(E_CharacterType _characterType, int _locX, int _locY, CharacterData charData)
    {
        characterType = _characterType;
        locX = _locX;
        locY = _locY;

        speed = charData.speed;
        movement = charData.movement;
        attack = charData.attack;
        defense = charData.defense;
        HP = charData.HP;
    }

    public void SetLocation(int x, int y)
    {
        locX = x;
        locY = y;
    }

    public bool isExist(int x, int y)
    {
        if(locX == x && locY == y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual void Attacked(int damage)
    {
        Debug.Log($"공격당함! 데미지 : {damage}");
        HP -= damage;
    }
}
