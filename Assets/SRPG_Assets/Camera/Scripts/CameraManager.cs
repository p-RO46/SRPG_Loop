using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager: MonoBehaviour
{
    [Header("Player")]
    public Transform playerObject;
    public InputActionAsset playerInput;

    [Header("Follow Player")]
    [SerializeField] private float posY;
    [SerializeField] private float posZ;
    [SerializeField] private bool isFollowing = true; // 이동 시 팔로우를 끌 수 있도록 추가

    [Header("Camera Rotation")]
    [SerializeField] private float rotX;
    [SerializeField] private float sensitivity = 0.5f;
    [SerializeField] private float topClamp = -10f;
    [SerializeField] private float bottomClamp = 10f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    [Header("Camera Move")]
    [SerializeField] private float moveSpeed = 10f;

    private InputAction IA_CameraRotate;
    private InputAction IA_CameraMove;

    private void Awake()
    {
        InputActionMap srpg = playerInput.FindActionMap("SRPG");
        IA_CameraRotate = srpg.FindAction("SRPG/Camera/Rotate");
        IA_CameraMove = srpg.FindAction("SRPG/Camera/Move");

        IA_CameraRotate.Enable();
        IA_CameraMove.Enable();
    }

    private void OnDisable()
    {
        IA_CameraRotate.performed -= OnLook;
        IA_CameraMove.performed -= OnCameraMove;
        IA_CameraRotate.Disable();
        IA_CameraMove.Disable();
    }

    void Update()
    {
        HandleRotation();
        HandleMovement();

        if (isFollowing)
        {
            FollowPlayer();
        }
    }

    private void HandleRotation()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            // Update에서 매 프레임 값을 읽어옵니다.
            Vector2 mouseInput = IA_CameraRotate.ReadValue<Vector2>();

            yRotation += mouseInput.x * sensitivity;
            xRotation += mouseInput.y * sensitivity;

            xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

            // 쿼터니언 적용
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
            playerObject.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }

    private void HandleMovement()
    {
        Vector2 moveInput = IA_CameraMove.ReadValue<Vector2>();

        if (moveInput.sqrMagnitude > 0)
        {
            isFollowing = false; // 마우스/키보드로 직접 움직이면 팔로우 중단
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            move.y = 0; // 카메라가 위아래로 박히지 않게 고정
            transform.position += move.normalized * moveSpeed * Time.deltaTime;
        }
    }

    private void FollowPlayer()
    {
        // 플레이어 뒤쪽 일정 거리를 유지하도록 계산
        Vector3 targetPos = playerObject.position + (playerObject.forward * posZ);
        targetPos.y = posY;

        // 부드러운 추적 효과 (Lerp)
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 5f);

        // 카메라 효과같은거 있으면 좋을듯
        // transform.position = new Vector3(playerObject.position.x, posY, playerObject.position.z + posZ);
    }
    private void OnLook(InputAction.CallbackContext context)
    {
        // 마우스 오른쪽 버튼 클릭 여부 체크
        if (Mouse.current.rightButton.isPressed)
        {
            Vector2 mouseInput = context.ReadValue<Vector2>();

            yRotation += mouseInput.x * sensitivity;
            xRotation -= mouseInput.y * sensitivity;
            xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

            // 카메라/플레이어 회전 적용
            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
            playerObject.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }

    private void OnCameraMove(InputAction.CallbackContext context)
    {
        // 카메라가 바라보는 방향 기준으로 이동 방향 계산
        Vector2 moveInput = context.ReadValue<Vector2>();

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        transform.position += move * moveSpeed * Time.deltaTime;
    }
}
