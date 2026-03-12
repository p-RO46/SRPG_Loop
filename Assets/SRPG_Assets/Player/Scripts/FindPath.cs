using System.Collections.Generic;
using UnityEngine;

public class FindPath // 몬스터들에게도 적용되는 코드
{
    private MapManager mapManager;

    private int _width, _height;
    private Node[,] _nodes;

    public FindPath(MapManager _mapManager)
    {
        mapManager = _mapManager;
        _width = mapManager.TileMap.Count;
        _height = mapManager.TileMap[0].Count;
        _nodes = new Node[_width, _height];

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _nodes[x, y] = new Node(x, y, mapManager.TileMap[x][y].tileType == E_TileType.MOVABLE);
                if(mapManager.EnemyExist(x, y) != null)
                {
                    _nodes[x, y].isWalkable = false;
                }
            }
        }
    }

    // BFS 방식 사용
    public SkillLocation[,] CanReach(Vector2Int startPos, int movement)
    {
        int max = movement * 2 + 1;
        SkillLocation[,] result = new SkillLocation[max, max];

        // 1. 시작점 데이터 미리 설정 (필요한 경우)
        result[movement, movement].location = startPos;
        result[movement, movement].skill = 0;

        // 방문 여부와 거리를 체크하기 위한 큐 (BFS)
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        // Dictionary 대신 단순 배열을 쓰면 더 빠르지만, 맵이 너무 크다면 Dictionary가 낫습니다.
        Dictionary<Vector2Int, int> visited = new Dictionary<Vector2Int, int>();

        queue.Enqueue(startPos);
        visited[startPos] = 0;

        while (queue.Count > 0)
        {
            Vector2Int curr = queue.Dequeue();
            int dist = visited[curr];

            if (dist >= movement) continue;

            // 상하좌우 탐색
            foreach (Vector2Int dir in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int next = curr + dir;

                // 맵 범위 체크 및 이동 가능 여부 체크
                if (next.x >= 0 && next.x < _width && next.y >= 0 && next.y < _height)
                {
                    if (_nodes[next.x, next.y].isWalkable && !visited.ContainsKey(next))
                    {
                        visited[next] = dist + 1;
                        queue.Enqueue(next);

                        // 결과 배열에 저장 (중심점 기준 상대 좌표 계산)
                        int resX = next.x - startPos.x + movement;
                        int resY = next.y - startPos.y + movement;

                        if (resX >= 0 && resX < max && resY >= 0 && resY < max)
                        {
                            result[resX, resY].location = next;
                            result[resX, resY].skill = 1;
                        }
                    }
                }
            }
        }
        return result;
    }

    public List<Node> FindShortestPath(Vector2Int start, Vector2Int end)
    {
        foreach(Node node in _nodes)
        {
            node.parent = null;
        }

        Node startNode = _nodes[start.x, start.y];
        Node targetNode = _nodes[end.x, end.y];

        // BFS는 일반 Queue를 사용합니다. (PriorityQueue 필요 없음!)
        Queue<Node> queue = new Queue<Node>();
        // 방문 여부를 체크할 집합
        HashSet<Node> visited = new HashSet<Node>();

        queue.Enqueue(startNode);
        visited.Add(startNode);

        while (queue.Count > 0)
        {
            Node currentNode = queue.Dequeue();

            // 목적지에 도착했다면 경로 역추적
            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                // 이동 가능하고, 아직 방문하지 않은 타일만 탐색
                if (neighbor.isWalkable && !visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    neighbor.parent = currentNode; // 경로를 기억하기 위해 부모 설정
                    queue.Enqueue(neighbor);
                }
            }
        }

        return null; // 경로 없음
    }

    private List<Node> RetracePath(Node startNode, Node targetNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = targetNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    private List<Node> GetNeighbors(Node node)
    {
        // 인접한 노드 리스트 반환
        List<Node> neighbors = new List<Node>();

        // 상하좌우
        int[] dx = { 0, 0, -1, 1 };
        int[] dy = { -1, 1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int checkX = node.pos.x + dx[i];
            int checkY = node.pos.y + dy[i];

            if(checkX >= 0 && checkX < _width && checkY >= 0 && checkY < _height)
            {
                neighbors.Add(_nodes[checkX, checkY]);
            }
        }

        return neighbors;
    }
}
