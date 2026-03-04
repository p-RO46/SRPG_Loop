using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MB_PlayerManager : MonoBehaviour
{
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

    [Header("Prefab Pool")]
    [SerializeField]
    private GameObject possibleTilePrefab;
    [SerializeField]
    private GameObject impossibleTilePrefab;

    [Header("Debug")]
    public bool startTest = false;

    // 현재 사용하려고 하는 스킬의 index;
    private int currentSkillIndex = 1;
    // 생성한 스킬 타일 프리팹 모음
    private List<GameObject> tilePrefabs = new List<GameObject>();

    private Coroutine readySkillRoutine = null;
    private Coroutine useTurnRoutine = null;

    void Start()
    {
        #region Input Action
        SRPG = PlayerInput.FindActionMap("SRPG");
        rotate_L = SRPG.FindAction("SRPG/PlayerRotate/Left");
        rotate_R = SRPG.FindAction("SRPG/PlayerRotate/Right");
        swapSkill = SRPG.FindAction("SRPG/PlayerSkill/Swap");

        rotate_L.Enable();
        rotate_R.Enable();
        swapSkill.Enable();
        rotate_L.performed += PlayerRotate_L;
        rotate_R.performed += PlayerRotate_R;
        swapSkill.performed += SwapSkill;
        #endregion
    }

    public void SetStart()
    {
        player = mapManager.player;
        UpdateTurns();
    }

    // Update is called once per frame
    void Update()
    {
        if (isTurn)
        {
            if(useTurnRoutine == null)
            {
                useTurnRoutine = StartCoroutine(UseTurn());
            }
            isTurn = false;
        }
    }

    private IEnumerator UseTurn()
    {
        Debug.Log("UseTurn 시작!");
        // 테스트용
        ReadySkill();
        while (totalTurn > 0)
        {
            yield return null;
        }
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
    private void ReadySkill()
    {
        Debug.Log("ReadySkill 시작!");
        if (attackTurn <= 0 && freeTurn <= 0)
        {
            Debug.Log("Error! ReadySkill : 스킬 사용 가능 턴이 없습니다.");
            return;
        }
        SetSkillTiles();

        #region Input Action
        rotate_L.Enable();
        rotate_R.Enable();
        swapSkill.Enable();
        rotate_L.performed += PlayerRotate_L;
        rotate_R.performed += PlayerRotate_R;
        swapSkill.performed += SwapSkill;
        #endregion

        if(readySkillRoutine == null)
        {
            readySkillRoutine = StartCoroutine(ReadySkillCycle());
        }
    }
    private IEnumerator ReadySkillCycle()
    {
        while (!startTest)
        {
            yield return null;
        }

        UseSkill();
    }
    private void UseSkill()
    {
        #region Input Action
        rotate_L.Disable();
        rotate_R.Disable();
        swapSkill.Disable();
        rotate_L.performed -= PlayerRotate_L;
        rotate_R.performed -= PlayerRotate_R;
        swapSkill.performed -= SwapSkill;
        #endregion

        ReturnAllTiles();
        player.playerClass.UseSkill();
        if(attackTurn > 0)
        {
            attackTurn--;
        }
        else
        {
            freeTurn--;
        }
        currentSkillIndex = 1; // Reset
        // PlayerClass에 회전 정보 남아있는것도 좀 아니꼬움..
        UpdateTurns();
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

    private void SetSkillTiles()
    {
        ReturnAllTiles();
        player.playerClass.CalculateRange(currentSkillIndex);

        int x = player.playerClass.realRangeX;
        int y = player.playerClass.realRangeY;

        for(int i = 0; i < x; i++)
        {
            for(int j = 0; j < y; j++)
            {
                SkillLocation sl = player.playerClass.GetRealRange(i, j);
                if (sl.skill == 1)
                {
                    GameObject tile = PoolManager.Instance.Get(possibleTilePrefab);

                    Vector3 pos = new Vector3(sl.location.x * 2, 0, sl.location.y * 2);
                    tile.transform.position = pos;
                    tile.transform.parent = transform;
                    tilePrefabs.Add(tile);
                }
                else if (sl.skill == -1)
                {
                    GameObject tile = PoolManager.Instance.Get(impossibleTilePrefab);

                    Vector3 pos = new Vector3(sl.location.x * 2, 0, sl.location.y * 2);
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

    private void UpdateTurns()
    {
        totalTurn = attackTurn + moveTurn + freeTurn;
    }
}
