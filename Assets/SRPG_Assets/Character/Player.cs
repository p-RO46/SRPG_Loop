using UnityEngine;
public enum E_PlayerClass
{
    WARRIOR,
    MAGE,
    ARCHER,
    HEALER,
    TELEPORTER,
    GUNNER,
}

public class Player : Character
{
    // 생성자로부터 건너온다
    public MapManager mapManager { get; private set; } // 얘가 너무 하위 클래스까지 영향을 미칠까봐 걱정되긴 함
    public GameData gameData { get; private set; }
    public PlayerSkill playerSkill { get; private set; }
    public PlayerData playerData { get; private set; }

    private int _loopCount;

    private E_PlayerClass E_playerClass;

    private PlayerClass warrior;
    private PlayerClass mage;

    public PlayerClass playerClass { get; private set; }
    public GameObject playerObject;

    // 공격 종류,
    // 공격 범위,
    // 공격 설명,
    // 공격마다 사용하는 이펙트, 애니메이션
    // 얘네를 묶은 공격 세트 객체가 필요

    public Player(int _locX, int _locY, E_PlayerClass _playerClass, MapManager _mapManager) : base(E_CharacterType.PLAYER, _locX, _locY)
    {
        mapManager = _mapManager;

        // MapManager에서 SO 넘겨받기
        gameData = mapManager.gameData; 
        playerSkill = mapManager.playerSkill;
        playerData = gameData.playerData;

        _speed = playerData.speed;
        _movement = playerData.movement;
        _attack = playerData.attack;
        _defense = playerData.defense;
        _HP = playerData.HP;
        _loopCount = playerData.loopCount;

        // this로 GameData, SkillData 참조 가능하니 괜찮다?
        // Map과의 연관성이 문제
        // 이 또한 정보를 넘겨주어야 하는 것 아닌가
        warrior = new Warrior(this); 
        mage = new Mage(this);

        E_playerClass = _playerClass;

        playerObject = mapManager.playerObject;


        playerClass = warrior;
        // 여기도 찝찝
        // 팩토리 패턴으로 대체?
        switch (E_playerClass)
        {
            case E_PlayerClass.WARRIOR:
                playerClass = warrior;
                break;
            case E_PlayerClass.MAGE:
                playerClass = mage;
                break;
            default:
                break;
        }
    }

    public override void Attacked(int damage)
    {
        Debug.Log($"\"Player\"가 공격당함! 데미지 : {damage}");
        _HP -= damage;
    }
}
