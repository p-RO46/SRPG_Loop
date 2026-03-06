using System;
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
            }
        }
    }

    /*
    public SkillLocation[,] CanReach(Vector2Int cur, int movement)
    {
    // 필수: 모든 노드의 데이터 초기화
    for (int x = 0; x < _width; x++) {
        for (int y = 0; y < _height; y++) {
            _nodes[x, y].G = 999999;
            _nodes[x, y].H = 0;
            _nodes[x, y].parent = null;
        }
    }
        // 현재 위치를 중심으로 했을 때, 도달 가능한 범위를 표시한 배열을 반환한다.
        int max = movement * 2 + 1;
        Node center = new Node(cur.x, cur.y, true); // 시작(중심)위치
        Node[,] temp = new Node[max, max];
        SkillLocation[,] result = new SkillLocation[max, max];

        for (int i = 0; i < max; i++)
        {
            int x = cur.x - movement + i;
            if (x < 0 || x >= _width)
            {
                for (int j = 0; j < max; j++)
                {
                    temp[i, j] = new Node(x, cur.y - movement + j, false);
                }
                continue;
            }

            for (int j = 0; j < max; j++)
            {
                int y = cur.y - movement + j;
                if (y < 0 || y >= _height)
                {
                    temp[i, j] = new Node(x, y, false);
                    continue;
                }

                temp[i, j] = new Node(x, y, mapManager.TileMap[x][y].tileType == E_TileType.MOVABLE);
            }
        }

        for (int i = 0; i < max; i++)
        {
            for(int j = 0; j < max; j++)
            {
                if (!temp[i, j].isWalkable) continue;
                List<Node> path = FindShortestPath(center.pos, temp[i, j].pos);
                if (path != null && path.Count <= movement)
                {
                    result[i, j].location = temp[i, j].pos;
                    result[i, j].skill = 1; // 이동 가능하면 1, 아니면 0
                }
            }
        }
        result[movement, movement].skill = 0; // 현재 서 있는 위치는 제외
        return result;
    }
    */
    // BFS 방식 사용
    public SkillLocation[,] CanReach(Vector2Int startPos, int movement)
    {
        int max = movement * 2 + 1;
        SkillLocation[,] result = new SkillLocation[max, max];

        // 방문 여부와 거리를 체크하기 위한 큐 (BFS)
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
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
        Node startNode = _nodes[start.x, start.y];
        Node targetNode = _nodes[end.x, end.y];

        // 아직 탐색하지 않은 노드들을 저장 (F값이 낮은 순으로 정렬)
        var openList = new PriorityQueue<Node, int>();
        var openSet = new HashSet<Node>();
        // 이미 탐색 완료된 노드들
        var closedList = new HashSet<Node>();

        // (1) 열린 리스트에 시작 노드를 넣는다.
        openList.Enqueue(startNode, startNode.F);
        openSet.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList.Dequeue();

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            // 탐색 완료
            closedList.Add(currentNode);
            // (2) 현재 노드에 인접한, 지나갈 수 있는 노드를 열린 리스트에 넣어준다.
            //     이미 탐색한 노드도 제외?
            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.isWalkable || closedList.Contains(neighbor))
                    continue;

                // 이동 비용 계산 (상하좌우 1, 대각선 제외)
                int newMovementCostToNeighbor = currentNode.G + GetDistance(currentNode, neighbor);

                if(newMovementCostToNeighbor < neighbor.G || !Contains(openSet, neighbor))
                {
                    neighbor.G = newMovementCostToNeighbor;
                    neighbor.H = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    openList.Enqueue(neighbor, neighbor.F);
                }
            }
        }

        return null;
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

    private int GetDistance(Node nodeA, Node nodeB)
    {
        // 맨해튼 거리 공식
        return Math.Abs(nodeA.pos.x - nodeB.pos.x) + Math.Abs(nodeA.pos.y - nodeB.pos.y);
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

    private bool Contains(HashSet<Node> hs, Node node)
    {
        // 간단한 구현을 위해 UnorderedItems 사용 (성능 최적화 필요 시 별도 체크 로직 구현 가능)
        foreach (var item in hs)
        {
            if (item == node) return true;
        }
        return false;
    }
}
