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
    [SerializeField] private Vector2 minPos = new Vector2(-50f, -50f); // X, Z 최소
    [SerializeField] private Vector2 maxPos = new Vector2(50f, 50f);   // X, Z 최대
    [SerializeField] private float minHeight = 0f;
    [SerializeField] private float maxHeight = 10f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minFOV = 30f;
    [SerializeField] private float maxFOV = 60f;
    private Camera cam;

    private InputAction IA_CameraRotate;
    private InputAction IA_CameraMove;
    private InputAction IA_CameraVerticalMove;
    private InputAction IA_CameraZoom;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        InputActionMap srpg = playerInput.FindActionMap("SRPG");
        IA_CameraRotate = srpg.FindAction("SRPG/Camera/Rotate");
        IA_CameraMove = srpg.FindAction("SRPG/Camera/Move");
        IA_CameraVerticalMove = srpg.FindAction("SRPG/Camera/VerticalMove");
        IA_CameraZoom = srpg.FindAction("SRPG/Camera/Zoom");

        // 전체 활성화
        srpg.Enable();
    }

    private void OnDisable()
    {
        IA_CameraRotate.Disable();
        IA_CameraMove.Disable();
        IA_CameraVerticalMove.Disable();
    }

    void Update()
    {
        HandleRotation();
        HandleMovement();
        HandleZoom();
        ApplyBounds();

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
            xRotation -= mouseInput.y * sensitivity;

            xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

            // 쿼터니언 적용
            transform.rotation = Quaternion.Euler(xRotation + rotX, yRotation, 0f);
            playerObject.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }

    private void HandleMovement()
    {
        Vector2 moveInput = IA_CameraMove.ReadValue<Vector2>();
        float verticalInput = IA_CameraVerticalMove.ReadValue<float>();
        Vector3 direction = Vector3.zero;

        if (moveInput.sqrMagnitude > 0 || Mathf.Abs(verticalInput) > 0)
        {
            isFollowing = false; // 마우스/키보드로 직접 움직이면 팔로우 중단

            // 수평 방향 계산 (카메라 기준)
            Vector3 horizontalDir = transform.right * moveInput.x + transform.forward * moveInput.y;
            // 수직 방향 계산 (월드 기준)
            Vector3 verticalDir = Vector3.up * verticalInput;

            direction = horizontalDir + verticalDir;

            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }
    private void HandleZoom()
    {
        float zoomInput = IA_CameraZoom.ReadValue<float>();

        if (Mathf.Abs(zoomInput) > 0)
        {
            // FOV를 조절하여 줌 구현 (가장 일반적인 방식)
            cam.fieldOfView -= zoomInput * zoomSpeed * Time.deltaTime;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
        }
    }

    private void ApplyBounds()
    {
        // 현재 위치를 가져와서 제한 범위 내로 고정(Clamp)
        float clampedX = Mathf.Clamp(transform.position.x, minPos.x, maxPos.x);
        float clampedY = Mathf.Clamp(transform.position.y, minHeight, maxHeight);
        float clampedZ = Mathf.Clamp(transform.position.z, minPos.y, maxPos.y);

        transform.position = new Vector3(clampedX, clampedY, clampedZ);
    }

    private void FollowPlayer()
    {
        // 플레이어 뒤쪽 일정 거리를 유지하도록 계산
        Vector3 targetPos = playerObject.position + (playerObject.forward * posZ);
        targetPos.y = posY;

        // 부드러운 추적 효과 (Lerp)
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 5f);
    }
}
