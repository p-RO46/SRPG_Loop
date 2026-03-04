using UnityEngine;

// Bush나 Fog를 이렇게 설정해 버리면 문제가 발생하지 않나?
// 바닥과 생명체를 분리해서 Map을 만들어야 하나?

public enum E_ObstacleType
{
    BUSH, // 은신
    BLOCK, // 이동 불가
    FOG, // 시야 차단으로 인한 이동력 감소
}
public class ObstacleTile : Tile
{
    public E_ObstacleType obstacleType;
    public ObstacleTile(E_ObstacleType _obstacleType) : base(E_TileType.OBSTACLE)
    {
        obstacleType = _obstacleType;
    }
}
