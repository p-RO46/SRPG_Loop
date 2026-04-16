using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public GameObject child;
    Animator childAnim;
    Rigidbody rigidBody;
    Vector3 direction;

    public float moveSpeed = 20f;
    public float dodgeSpeed = 50f;

    public float cooldownTime = 1.0f; // 쿨타임 (1초)
    private float lastInputTime;
    private int attackCount = 0;

    // ... 위 코드의 변수들에 추가
    public float dashDuration = 0.2f;
    private bool isDashing = false;

    public float attackDuration = 1f;
    private bool isAttacking = false;
    public float attackDash = 1f;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        childAnim = child.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if(!isDashing && !isAttacking)
        {
            PlayerRun();
        }
    }

    public void PlayerRun()
    {
        rigidBody.linearVelocity = direction * moveSpeed + Vector3.up * rigidBody.linearVelocity.y;
    }

    public void OnRunInput(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        if (input.x > 0) // right
        {
            LookAt(PlayerDir.RIGHT);
        }
        else if (input.x < 0)
        {
            LookAt(PlayerDir.LEFT);
        }
        Quaternion correction = Quaternion.Euler(new Vector3(0, 45, 0));
        direction = new Vector3(input.x, 0f, input.y);
        direction = correction * direction;
    }

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        // 스페이스바
        // 버튼을 눌렀을 때(Started)만 체크
        if (context.performed)
        {
            if (Time.time - lastInputTime >= cooldownTime)
            {
                if (!isDashing && !isAttacking) StartCoroutine(AttackRoutine());
            }
            else
            {
                Debug.Log("아직 쿨타임 중입니다!");
            }
        }
    }

    public void OnDodgeInput(InputAction.CallbackContext context)
    {
        // 스페이스바
        // 버튼을 눌렀을 때(Started)만 체크
        if (context.performed)
        {
            if (Time.time - lastInputTime >= cooldownTime)
            {
                if(!isDashing && !isAttacking) StartCoroutine(DashRoutine());
            }
            else
            {
                Debug.Log("아직 쿨타임 중입니다!");
            }
        }
    }

    IEnumerator DashRoutine()
    {
        isDashing = true;

        Vector3 dashDir = direction;
        if (dashDir == Vector3.zero) dashDir = transform.forward;
        if (dashDir.x > 0) // right
        {
            LookAt(PlayerDir.RIGHT);
        }
        else if (dashDir.x < 0)
        {
            LookAt(PlayerDir.LEFT);
        }

        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            //rigidBody.AddForce(direction * dodgeSpeed, ForceMode.VelocityChange);
            //yield return null; // 다음 프레임까지 대기

            rigidBody.linearVelocity = dashDir * dodgeSpeed;

            yield return null;
        }

        rigidBody.linearVelocity = Vector3.zero;
        isDashing = false;
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        Vector3 attackDir = direction;
        if (attackDir == Vector3.zero) attackDir = transform.forward;
        if (attackDir.x > 0) // right
        {
            LookAt(PlayerDir.RIGHT);
        }
        else if (attackDir.x < 0)
        {
            LookAt(PlayerDir.LEFT);
        }

        float startTime = Time.time;

        childAnim.SetTrigger("Attack");

        while (Time.time < startTime + attackDuration)
        {
            //if (direction == Vector3.zero) direction = transform.forward;
            //rigidBody.AddForce(direction * attackDash, ForceMode.VelocityChange);
            //yield return null; // 다음 프레임까지 대기

            rigidBody.linearVelocity = attackDir * attackDash;

            yield return null;
        }

        isAttacking = false;
    }

    void LookAt(PlayerDir dir)
    {
        // 8 directions, sprite-level state
        // 일단은 오/왼만
        switch (dir)
        {
            case PlayerDir.LEFT:
                child.transform.localScale = new Vector3(4, 4, 4);
                break;
            case PlayerDir.RIGHT:
                child.transform.localScale = new Vector3(-4, 4, 4);
                break;
        }
    }
}
