using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Parents")]
    [SerializeField]
    private Transform MapParent;
    [SerializeField]
    private Transform EnemyParent;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject tilePrefab;
    [SerializeField]
    private GameObject obstaclePrefab;
    [SerializeField]
    private List<GameObject> enemyPrefabs;

    [Header("DB")]
    public GameData gameData;
    public PlayerSkill playerSkill;
    [SerializeField]
    private TextAsset tileDatasCSV;
    [SerializeField]
    private TextAsset characterDatasCSV;

    /// <summary>
    /// Enum
    /// 이걸 읽고, 바로 객체를 생성하는 쪽으로?
    /// 하지만 구체적인 데이터는 들어가있지 않을 텐데 어떡하지
    /// csv차원에서 얼마나 구체적으로 데이터를 처리할 것인가
    /// int로 가져가되, 10으로 나눴을 때 몫과 나머지로 처리할까 (되게 고전적이다)
    /// 나머지가 TileType, 몫이 Name 혹은 Class를 나타내는 거
    /// </summary>
    // Instance
    // private List<List<Character>> C_Map = new List<List<Character>>(); // Character Map. ENEMY와 PLAYER를 관리함
    private List<List<Tile>> T_Map = new List<List<Tile>>(); // Tile Map. 일반 타일(MOVABLE)과 특수 타일(OBSTACLE)을 관리함
    
    public Player player { get; private set; }
    private List<Enemy> enemys = new List<Enemy>(); // 정렬 기준 만들어야 함. speed 순

    // 외부용 열람 프로퍼티 (IReadOnlyList로 감싸서 노출)
    public IReadOnlyList<IReadOnlyList<Tile>> TileMap => T_Map;

    // Transform, Animation 등 그래픽적 요소만 처리
    public List<GameObject> EnemyObjects = new List<GameObject>();
    public GameObject playerObject;
    private int enemyIndex = 0;

    private void Start()
    {
        ReadTMapDatas();
        ReadCMapDatas();

        SetTiles();
    }

    private void ReadTMapDatas()
    {
        string fs = tileDatasCSV.text;
        string[] fLines = fs.Split("\n");
        for (int i = 0; i < fLines.Length; i++)
        {
            T_Map.Add(new List<Tile>());
            string[] fTypes = fLines[i].Split(",");

            for (int j = 0; j < fTypes.Length; j++)
            {
                int temp = int.Parse(fTypes[j]);
                E_TileType tempTile = (E_TileType)(temp % 10);

                if (tempTile == E_TileType.OBSTACLE)
                {
                    T_Map[i].Add(new ObstacleTile((E_ObstacleType)(temp / 10)));
                }
                else
                {
                    T_Map[i].Add(new Tile(tempTile));
                }
            }
        }
    }

    /// <summary>
    /// 캐릭터의 경우, 전체 맵에 싹 다 깔아놓을 게 아니라 캐릭터 오브젝트 하나당 현재 좌표 정보를 저장하는 게 낫지 않나?
    /// 이렇게 되면 몬스터와 캐릭터의 겹침 문제는 파악하기 더 어려워질까?
    /// 오히려 더 쉬울 것 같기도 하고...
    /// 일단 Empty가 있는게 겁나 낭비같다 왜있냐고.
    /// </summary>
    private void ReadCMapDatas()
    {
        string fs = characterDatasCSV.text;
        string[] fLines = fs.Split("\n");
        for (int i = 0; i < fLines.Length; i++)
        {
            //C_Map.Add(new List<Character>());
            string[] fTypes = fLines[i].Split(",");

            for (int j = 0; j < fTypes.Length; j++)
            {
                int temp = int.Parse(fTypes[j]);
                E_CharacterType tempChar = (E_CharacterType)(temp % 10);

                if (tempChar == E_CharacterType.PLAYER)
                {
                    playerObject.transform.position = new Vector3(i * 2, 0, j * 2);
                    player = new Player(i, j, (E_PlayerClass)(temp / 10), this);
                    playerObject.GetComponent<MB_PlayerManager>().SetStart();
                }
                else if (tempChar == E_CharacterType.ENEMY)
                {
                    GameObject enemy = PoolManager.Instance.Get(enemyPrefabs[temp / 10]);
                    enemy.transform.position = new Vector3(i * 2, 0, j * 2);
                    enemy.transform.parent = EnemyParent;
                    EnemyObjects.Add(enemy);

                    enemys.Add(new Enemy(i, j, (E_EnemyType)(temp / 10), EnemyObjects[enemyIndex++], this));
                }
                else 
                {
                    // 아무것도 없음
                }
            }
        }
    }

    private void SetTiles()
    {
        for(int i = 0; i < T_Map.Count; i++)
        {
            for(int j = 0; j < T_Map[i].Count; j++)
            {
                switch (T_Map[i][j].tileType)
                {
                    case E_TileType.MOVABLE:
                        Instantiate(tilePrefab, new Vector3(i * 2, 0, j * 2), Quaternion.identity, MapParent);
                        break;
                    case E_TileType.OBSTACLE:
                        Instantiate(obstaclePrefab, new Vector3(i * 2, 0, j * 2), Quaternion.identity, MapParent);
                        break;
                    case E_TileType.EMPTY:
                    default:
                        break;
                }
            }
        }
    }

    // 이걸 여기서 처리하는 게 적절한지?
    // Map위 특정 좌표에 Enemy가 존재하는지 검사하는 함수
    public Enemy EnemyExist(int x, int y)
    {
        foreach(Enemy e in enemys)
        {
            if(e.isExist(x, y))
            {
                return e;
            }
        }
        return null;
    }
}