using UnityEngine;
public enum E_TileType
{
    EMPTY = 0,
    MOVABLE = 1,
    OBSTACLE = 2,
}

public class Tile
{
    public E_TileType tileType;
    public Tile(E_TileType _tileType)
    {
        tileType = _tileType;
    }
}
