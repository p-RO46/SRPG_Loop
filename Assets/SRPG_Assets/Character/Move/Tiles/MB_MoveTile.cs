using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class MB_MoveTile : MonoBehaviour, IPointerClickHandler
{
    private MB_PlayerManager playerManager; // Observer
    private Vector2Int moveTilesIndex;

    // 한 번에 하나만 클릭가능
    // Observer 하는게 낫나?
    public bool isClicked = false;
    private MeshRenderer meshRenderer;

    [SerializeField]
    private Material movableMaterial;
    [SerializeField]
    private Material hoverMaterial;
    [SerializeField]
    private Material endMaterial;
    [SerializeField]
    private Material pathMaterial;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (AnybodyClicked()) return;
        SetHover();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (AnybodyClicked()) return;
        SetMovable();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 리스트 순회하며 false로 만들어주기,
            MakeAllDeselect();
            // 어차피 경로 표시하려면 전체 타일 관리해야 할 듯
            isClicked = true;
            playerManager.SetDestination(moveTilesIndex);
        }
    }

    public void SetMovable()
    {
        meshRenderer.material = movableMaterial;
    }
    public void SetHover()
    {
        meshRenderer.material = hoverMaterial;
    }
    public void SetEnd()
    {
        meshRenderer.material = endMaterial;
    }
    public void SetPath()
    {
        meshRenderer.material = pathMaterial;
    }

    public void SubscribePlayerManager(MB_PlayerManager pm, Vector2Int index)
    {
        playerManager = pm;
    }

    public void UnsubscribePlayerManager()
    {
        playerManager = null;
        SetMovable();
    }

    #region Private
    private void MakeAllDeselect()
    {
        int x = playerManager.moveTiles.GetLength(0);
        int y = playerManager.moveTiles.GetLength(1);

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                playerManager.moveTiles[i, j].isClicked = false;
                playerManager.moveTiles[i, j].SetHover();
            }
        }
    }

    private bool AnybodyClicked()
    {
        int x = playerManager.moveTiles.GetLength(0);
        int y = playerManager.moveTiles.GetLength(1);

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                if (playerManager.moveTiles[i, j].isClicked)
                {
                    return true;
                }
            }
        }

        return false;
    }
    #endregion
}
