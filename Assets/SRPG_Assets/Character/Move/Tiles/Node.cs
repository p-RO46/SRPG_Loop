using UnityEngine;

public class Node
{
    public Vector2Int pos;
    public bool isWalkable;
    public Node parent;

    public int G;
    public int H;
    public int F => G + H;

    public Node(int x, int y, bool isWalkable)
    {
        pos = new Vector2Int(x, y);
        this.isWalkable = isWalkable;
    }
}