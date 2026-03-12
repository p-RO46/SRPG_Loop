using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // 필수!
using UnityEngine.InputSystem;

public class MB_MoveTileManager : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private LayerMask UILayer;
    [SerializeField] private LayerMask interactableLayer; // 클릭을 감지할 레이어 선택
    [SerializeField] private float maxDistance = 100f;   // 레이캐스트 최대 거리

    private GameObject currentSelectedObject;
    private GameObject lastHoveredObject;

    [SerializeField]
    private Material movableMaterial;
    [SerializeField]
    private Material hoverMaterial;

    private MeshRenderer lastRenderer;
    private MB_PlayerManager playerManager;

    private void Start()
    {
        playerManager = GetComponent<MB_PlayerManager>();
    }

    private void Update()
    {
        // 마우스 위치 가져오기
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // 1. 마우스 왼쪽 버튼 클릭 감지
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // 마우스가 UI 위에 있다면 아래 로직(게임 오브젝트 클릭)을 무시합니다.
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, maxDistance, interactableLayer))
            {
                SelectObject(hit.transform.gameObject);
            }
            else
            {
                // 빈 공간을 가리키고 있다면 이전 상태 해제
                DeselectObject();
            }
        }
        else if(currentSelectedObject == null)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance, interactableLayer))
            {
                GameObject currentObject = hit.transform.gameObject;

                // 마우스가 가리키는 대상이 바뀌었을 때만 실행
                if (currentObject != lastHoveredObject)
                {
                    // 1. 이전 오브젝트 원상복구
                    ClearLastHover();

                    // 2. 새 오브젝트 하이라이트
                    lastHoveredObject = currentObject;
                    Renderer renderer = lastHoveredObject.GetComponent<Renderer>();

                    if (renderer != null)
                    {
                        renderer.material = hoverMaterial;
                    }
                }
            }
            else
            {
                // 빈 공간을 가리키고 있다면 이전 상태 해제
                ClearLastHover();
            }
        }
    }

    private void SelectObject(GameObject obj)
    {
        // 이전에 선택된 게 있다면 먼저 해제
        if (currentSelectedObject != null) DeselectObject();

        currentSelectedObject = obj;
        lastRenderer = obj.GetComponent<MeshRenderer>();

        if(lastRenderer != null)
        {
            Vector2Int destination = obj.GetComponent<MB_MoveTile>().moveTilesIndex;
            playerManager.SetDestination(destination);
        }
    }

    private void DeselectObject()
    {
        if (currentSelectedObject != null)
        {
            if (lastRenderer != null)
            {
                playerManager.CancelPath();
            }
            // Debug.Log(currentSelectedObject.name + " 해제됨");
            currentSelectedObject = null;
            lastRenderer = null;
        }
    }

    // 하이라이트를 해제하는 공통 함수
    void ClearLastHover()
    {
        if (lastHoveredObject != null)
        {
            MeshRenderer meshRenderer = lastHoveredObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null && movableMaterial != null)
            {
                meshRenderer.material = movableMaterial;
            }
            lastHoveredObject = null;
        }
    }
}
