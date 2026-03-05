using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MB_MoveTileManager : MonoBehaviour
{
    private GameObject currentSelectedObject;
    [SerializeField]
    private Material endMaterial;
    [SerializeField]
    private Material pathMaterial;

    private MeshRenderer lastRenderer;
    private MB_PlayerManager playerManager;

    private void Start()
    {
        playerManager = GetComponent<MB_PlayerManager>();
    }

    private void Update()
    {
        // 1. 마우스 왼쪽 버튼 클릭 감지 씨 발 이거 무야?
        if (Input.GetMouseButtonDown(0))
        {
            // 2. UI를 클릭 중이라면 3D 오브젝트 선택 로직은 건뛰
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                SelectObject(hit.transform.gameObject);
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
            
        }
    }

    private void DeselectObject()
    {
        if(currentSelectedObject != null)
        {

        }
    }
}
