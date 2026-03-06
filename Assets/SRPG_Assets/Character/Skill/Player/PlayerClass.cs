using System.Collections.Generic;
using UnityEngine;

public struct SkillLocation
{
    public Vector2Int location;
    public int skill; // 1:possible / 0:empty / -1:impossible
}

public class PlayerClass // 상태 객체
{
    protected Player player;
    protected List<SkillData> skillData; // 생성 이후에는 내부에서도 readonly로 써야함
    protected int skillCount;

    // local 기준 스킬 범위
    public int[,] skillRange { get; protected set; }

    #region 실제 스킬 적용 위치
    // World(TileMap) 기준 스킬 범위 좌표
    private SkillLocation[,] realRange = new SkillLocation[3, 3];

    // "가져오기" 기능을 명확히 표현
    public SkillLocation GetRealRange(int x, int y)
    {
        // 안전 장치 추가가 용이함
        if (IsOutOfBounds(x, y)) return default;
        return realRange[x, y];
    }
    private bool IsOutOfBounds(int x, int y) =>
        x < 0 || x >= realRangeX || y < 0 || y >= realRangeY;

    // 배열의 크기를 알 수 있게 속성 제공
    public int realRangeX => realRange.GetLength(0);
    public int realRangeY => realRange.GetLength(1);
    #endregion

    public int currentSkillIndex = 1; // 하

    public PlayerClass(Player _player)
    {
        player = _player;
    }

    public void Rotate_L() // 반시계 방향으로 회전
    {
        int len = skillRange.GetLength(0);
        int[,] temp = (int[,])skillRange.Clone();

        for (int i = 0; i < len; i++)
        {
            for (int j = 0; j < len; j++)
            {
                skillRange[i, j] = temp[j, len - 1 - i];
            }
        }
    }
    public void Rotate_R() // 시계 방향으로 회전
    {
        int len = skillRange.GetLength(0);
        int[,] temp = (int[,])skillRange.Clone();

        for (int i = 0; i < len; i++)
        {
            for (int j = 0; j < len; j++)
            {
                skillRange[i, j] = temp[len - 1 - j, i];
            }
        }
    }

    public void CalculateRange(int index) // index는 1 ~ n
    {
        if (skillCount >= index)
        {
            if(currentSkillIndex != index)
            {
                // Deep Copy
                skillRange = (int[,])skillData[index - 1].skillRange.Clone();
                currentSkillIndex = index;
            }
            int len = skillRange.GetLength(0);
            realRange = new SkillLocation[len, len];

            int len2 = len / 2;
            int startX = player.locX - len2, endX = player.locX + len2;
            int startY = player.locY - len2, endY = player.locY + len2;

            for (int i = startX; i <= endX; i++)
            {
                if (i < 0 || i >= player.mapManager.TileMap.Count)
                {
                    for (int k = 0; k < len; k++)
                    {
                        realRange[i - startX, k].skill = 0;
                        realRange[i - startX, k].location = new Vector2Int(-1, -1);
                    }
                    continue;
                }

                for (int j = startY; j <= endY; j++)
                {
                    if (j < 0 || j >= player.mapManager.TileMap[i].Count)
                    {
                        realRange[i - startX, j - startY].skill = 0;
                        realRange[i - startX, j - startY].location = new Vector2Int(-1, -1);
                        continue;
                    }

                    Debug.Log((i - startX) + ", " + (j - startY));
                    if (skillRange[i - startX, j - startY] == 1)
                    {
                        // 공격 가능한 타일이라면 1
                        if (player.mapManager.TileMap[i][j].tileType == E_TileType.MOVABLE)
                        {
                            realRange[i - startX, j - startY].skill = 1;
                        }
                        else // 비었거나, 장애물이라면 -1
                        {
                            realRange[i - startX, j - startY].skill = -1;
                        }
                    }
                    realRange[i - startX, j - startY].location = new Vector2Int(i, j);
                }
            }
        }
        else
        {
            Debug.Log("Error from CalculateRange: index Out Of Range");
        }
    }
    public void UseSkill()
    {
        CalculateRange(currentSkillIndex); // 범위 계산 완료
        // 0. realLocation인 곳에 이펙트 발생 (이건 고민필요)
        // 1. 타일 위에 몬스터가 존재하는지 검사 => 이건 MapManager 선에서 해결
        // 2. 있을 경우, 데미지를 입게 함
        foreach (SkillLocation loc in realRange)
        {
            Enemy target = player.mapManager.EnemyExist((int)loc.location.x, (int)loc.location.y);
            if (target != null)
            {
                target.Attacked(CurrentSkillData().damage);
            }
        }
        player.playerClass.currentSkillIndex = 1;
    }
    public string SkillAnim()
    {
        string trigger = CurrentSkillData().animTrigger;
        return trigger;
    }

    public SkillData CurrentSkillData()
    {
        return skillData[currentSkillIndex - 1];
    }
}
