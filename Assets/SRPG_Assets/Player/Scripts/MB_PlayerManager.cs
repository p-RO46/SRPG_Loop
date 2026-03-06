using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MB_PlayerManager : MonoBehaviour
{
    #region Variables
    [Header("Input Action Asset")]
    [SerializeField]
    private InputActionAsset PlayerInput;
    private InputActionMap SRPG;

    private InputAction swapSkill;
    private InputAction rotate_R;
    private InputAction rotate_L;

    [Header("Map Manager")]
    [SerializeField]
    private MapManager mapManager;
    private Player player;

    // 플레이어와 관련된 이벤트 처리를 하는 곳
    [Header("Does Player Have Turn")]
    [SerializeField]
    private bool isTurn = false;

    [Header("Player Turn Behaviour")]
    [SerializeField]
    private int totalTurn;
    [SerializeField]
    private int attackTurn = 1;
    [SerializeField]
    private int moveTurn = 1;
    [SerializeField]
    private int freeTurn = 1;

    [Header("UI")]
    [SerializeField]
    private GameObject gameUI;
    [SerializeField]
    private TextMeshProUGUI attackTurnText;
    [SerializeField]
    private TextMeshProUGUI moveTurnText;
    [SerializeField]
    private TextMeshProUGUI freeTurnText;

    [Header("Prefab Pool")]
    [SerializeField]
    private GameObject possibleTilePrefab;
    [SerializeField]
    private GameObject impossibleTilePrefab;
    [SerializeField]
    private GameObject movableTilePrefab;

    [Header("State")]
    [SerializeField]
    private bool startSkill = false;
    [SerializeField]
    private bool startMove = false;

    [Header("Effect Parent")]
    [SerializeField]
    private Transform effectParent;

    [Header("Tile Materials(Move)")]
    [SerializeField]
    private Material movableMaterial;
    [SerializeField]
    private Material endMaterial;
    [SerializeField]
    private Material pathMaterial;

    // Animator
    private Animator playerAnim;
    // 현재 사용하려고 하는 스킬의 index;
    private int currentSkillIndex = 1;

    // 생성한 스킬 타일 프리팹 모음
    private List<GameObject> tilePrefabs = new List<GameObject>();
    [HideInInspector]
    public MB_MoveTile[,] moveTiles;
    private List<Node> path = new List<Node>();

    // Coroutine
    private Coroutine useTurnRoutine = null;
    private Coroutine startTurnRoutine = null;
    #endregion
    #region Unity LifeCycle
    void Start()
    {
        SRPG = PlayerInput.FindActionMap("SRPG");
        rotate_L = SRPG.FindAction("SRPG/PlayerRotate/Left");
        rotate_R = SRPG.FindAction("SRPG/PlayerRotate/Right");
        swapSkill = SRPG.FindAction("SRPG/PlayerSkill/Swap");
        EnableActions();

        playerAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isTurn)
        {
            if(startTurnRoutine == null)
            {
                startTurnRoutine = StartCoroutine(StartTurn());
            }
            isTurn = false;
        }
    }
    #endregion

    #region Turn Management
    public void SetStart() // MapManger가 모든 객체를 생성한 뒤 마지막으로 Player를 할당해주기 위함. 생성 전에 할당하면 꼬인다
    {
        player = mapManager.player;
        UpdateTurns();
    }
    private void UpdateTurns()
    {
        totalTurn = attackTurn + moveTurn + freeTurn;
    }
    private IEnumerator StartTurn()
    {
        Debug.Log("UseTurn 시작!");
        gameUI.SetActive(true);
        UpdateUI();
        while (totalTurn > 0)
        {
            yield return null;
        }
        useTurnRoutine = null;
    }
    #endregion

    #region Attack
    public void ReadySkill()
    {
        Debug.Log("ReadySkill 시작!");
        if (attackTurn <= 0 && freeTurn <= 0)
        {
            Debug.Log("Error! ReadySkill : 스킬 사용 가능 턴이 없습니다.");
            return;
        }
        SetSkillTiles();

        EnableActions();
    }

    public void StartSkill()
    {
        if (useTurnRoutine == null)
        {
            useTurnRoutine = StartCoroutine(StartSkillCycle());
            ReturnAllTiles();
            player.playerClass.UseSkill();
            if (attackTurn > 0)
            {
                attackTurn--;
            }
            else
            {
                freeTurn--;
            }
            UpdateUI();
            currentSkillIndex = 1; // Reset
            UpdateTurns();
        }
    }
    private IEnumerator StartSkillCycle()
    {
        DisableActions();

        int x = player.playerClass.realRangeX;
        int y = player.playerClass.realRangeY;

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                SkillLocation sl = player.playerClass.GetRealRange(i, j);
                if(sl.skill == 1)
                {
                    GameObject effect = PoolManager.Instance.Get(player.playerClass.CurrentSkillData().skillEffect);
                    effect.transform.position = new Vector3(sl.location.x * 2, 0, sl.location.y * 2);
                    effect.transform.parent = effectParent;

                    // 꺼내자마자 파티클 재생 (Stop Action이 다시 활성화됨)
                    if (effect.TryGetComponent<ParticleSystem>(out var ps))
                    {
                        ps.Play();
                    }
                }
            }
        }

        playerAnim.SetTrigger(player.playerClass.SkillAnim());
        yield return new WaitForSeconds(3f);
        useTurnRoutine = null;
    }
    private void SwapSkill(InputAction.CallbackContext context)
    {
        string keyName = context.control.name;
        if (int.TryParse(keyName, out int result))
        {
            Debug.Log($"눌린 숫자: {result}");
        }
        currentSkillIndex = result;
        SetSkillTiles();
    }
    private void SetSkillTiles()
    {
        ReturnAllTiles();
        player.playerClass.CalculateRange(currentSkillIndex);

        int x = player.playerClass.realRangeX;
        int y = player.playerClass.realRangeY;

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                SkillLocation sl = player.playerClass.GetRealRange(i, j);
                if (sl.skill == 1)
                {
                    GameObject tile = PoolManager.Instance.Get(possibleTilePrefab);

                    Vector3 pos = new Vector3(sl.location.x * 2, 0.1f, sl.location.y * 2);
                    tile.transform.position = pos;
                    tile.transform.parent = transform;
                    tilePrefabs.Add(tile);
                }
                else if (sl.skill == -1)
                {
                    GameObject tile = PoolManager.Instance.Get(impossibleTilePrefab);

                    Vector3 pos = new Vector3(sl.location.x * 2, 0.1f, sl.location.y * 2);
                    tile.transform.position = pos;
                    tile.transform.parent = transform;
                    tilePrefabs.Add(tile);
                }
            }
        }
    }
    private void ReturnAllTiles()
    {
        // 타일을 모두 풀로 돌려보낸다
        foreach (GameObject tile in tilePrefabs)
        {
            if (tile.TryGetComponent<PooledObject>(out var pooled))
            {
                pooled.ReturnToPool();
            }
        }
        tilePrefabs = new List<GameObject>();
    }
    private void PlayerRotate_L(InputAction.CallbackContext context)
    {
        player.playerClass.Rotate_L();
        SetSkillTiles();
    }
    private void PlayerRotate_R(InputAction.CallbackContext context)
    {
        player.playerClass.Rotate_R();
        SetSkillTiles();
    }
    #endregion
    #region Move
    public void ReadyMove()
    {
        SetMoveTiles();
        // 생성된 타일들에 대해서 상호작용 하도록.
        // 클릭 이벤트는 타일 프리팹 자체에서 처리하는데
        // 경로를 보여줘야 하니까 이게 복잡하네
    }
    public void StartMove()
    {
        // path List 를 기반으로 캐릭터의 이동 구현.

    }
    private IEnumerator StartMoveCycle()
    {
        int count = path.Count;
        while(count > 0)
        {
            
            count--;
        }
        yield return null;
    }

    private void MovePlayer()
    {
        // 1. 현재 보고 있는 방향을 파악할 것
        // 2. 이동해야 할 방향으로 회전할 것
        // 3. 한 칸 이동
        // 4. 도착할 때까지 반복.

    }
    private void RotatePlayer(bool right) // true : right, false : left
    {
        if (right)
        {
            transform.Rotate(new Vector3(0, 90, 0));
        }
        else
        {
            transform.Rotate(new Vector3(0, -90, 0));
        }
    }

    private void SetMoveTiles()
    {
        ReturnAllTiles();

        FindPath findPath = new FindPath(mapManager);
        SkillLocation[,] canMove = findPath.CanReach(new Vector2Int(player.locX, player.locY), player.movement);

        int x = canMove.GetLength(0);
        int y = canMove.GetLength(1);

        // 얘는 여기서 초기화?
        moveTiles = new MB_MoveTile[x, y];

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                GameObject tile = PoolManager.Instance.Get(movableTilePrefab);

                Vector3 pos = new Vector3(canMove[i, j].location.x * 2, 0.1f, canMove[i, j].location.y * 2);
                tile.transform.position = pos;
                tilePrefabs.Add(tile);

                moveTiles[i, j] = tile.GetComponent<MB_MoveTile>();
                moveTiles[i, j].SetIndex(canMove[i, j].location); // 전체 맵 기준 인덱스
            }
        }
    }
    public void SetDestination(Vector2Int dest)
    {
        FindPath findPath = new FindPath(mapManager);
        Vector2Int playerPos = new Vector2Int(player.locX, player.locY);

        int startX = moveTiles[0, 0].moveTilesIndex.x;
        int startY = moveTiles[0, 0].moveTilesIndex.y;

        path = findPath.FindShortestPath(playerPos, dest);
        Debug.Log(path.Count);
        foreach (Node node in path)
        {
            moveTiles[node.pos.x - startX, node.pos.y - startY].ChangeMaterial(pathMaterial);
        }
        moveTiles[dest.x - startX, dest.y - startY].ChangeMaterial(endMaterial);
    }

    public void CancelPath()
    {
        for (int i = 0; i < moveTiles.GetLength(0); i++)
        {
            for (int j = 0; j < moveTiles.GetLength(1); j++)
            {
                moveTiles[i, j].ChangeMaterial(movableMaterial);
            }
        }
    }
    #endregion
    #region Class Swap
    public void ReadySwapClass()
    {

    }
    public void StartSwapClass()
    {

    }
    private IEnumerator StartSwapClassCycle()
    {
        yield return null;
    }
    #endregion

    #region Input Action
    private void EnableActions()
    {
        rotate_L.Enable();
        rotate_R.Enable();
        swapSkill.Enable();
        rotate_L.performed += PlayerRotate_L;
        rotate_R.performed += PlayerRotate_R;
        swapSkill.performed += SwapSkill;
    }
    private void DisableActions()
    {
        rotate_L.Disable();
        rotate_R.Disable();
        swapSkill.Disable();
        rotate_L.performed -= PlayerRotate_L;
        rotate_R.performed -= PlayerRotate_R;
        swapSkill.performed -= SwapSkill;
    }
    #endregion

    #region UI Update
    private void UpdateUI()
    {
        attackTurnText.text = "Attack Turn : " + attackTurn;
        moveTurnText.text = "Move Turn : " + moveTurn;
        freeTurnText.text = "Free Turn : " + freeTurn;
    }
    #endregion
}
