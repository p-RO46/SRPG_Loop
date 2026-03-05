using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform playerObject;
    [SerializeField]
    private float posY;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(playerObject.position.x, posY, playerObject.position.z);
    }
}
