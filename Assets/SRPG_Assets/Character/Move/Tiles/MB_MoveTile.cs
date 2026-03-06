using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class MB_MoveTile : MonoBehaviour
{
    //private MB_PlayerManager playerManager;
    public Vector2Int moveTilesIndex { get; private set; }
    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    public void SetIndex(Vector2Int index)
    {
        moveTilesIndex = index;
    }
    public void ChangeMaterial(Material material)
    {
        meshRenderer.material = material;
    }
}
