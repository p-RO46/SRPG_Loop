using UnityEngine;

public enum E_EnemyType
{
    BOSS,
    GHOST,
    DEMON,
    ZOMBIE,
}
 
public class Enemy : Character
{
    public E_EnemyType enemyType;
    public GameObject enemyObject;

    public Enemy(int _locX, int _locY, E_EnemyType _enemyType, GameObject _enemyObject, MapManager _mapManager) 
        : base(E_CharacterType.ENEMY, _locX, _locY, _mapManager.gameData.enemyData[(int)_enemyType])
    {
        enemyType = _enemyType;
        enemyObject = _enemyObject;
    }

    public override void Attacked(int damage)
    {
        Debug.Log($"\"{enemyType}\"이(가) 공격당함! 데미지 : {damage}");
        HP -= damage;
    }
}
