// using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem; // ⭐ 필수: 이게 있어야 새로운 시스템을 씁니다.

public class Player : MonoBehaviour
{
    [Header("입력 설정 (인스펙터에서 키 지정)")]
    public InputAction moveAction; // 이동 키 (A, D / 화살표)
    public InputAction jumpAction; // 점프 키 (Space)

    [Header("설정값")]
    public float moveSpeed = 8f;
    public float jumpForce = 15f;

    [Header("땅 체크")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.5f);
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveInput;
    private bool isFacingRight = true;

    // ⭐ 중요: 입력 액션은 켜고 꺼줘야 작동합니다.
    private void OnEnable() 
    {
        moveAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable() 
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1. 이동 입력 받기 (New System 방식)
        // ReadValue<Vector2>()는 (x, y) 값을 줍니다. 우리는 좌우(x)만 필요합니다.
        Vector2 inputVector = moveAction.ReadValue<Vector2>();
        moveInput = inputVector.x;

        // 2. 점프 입력 받기 (New System 방식)
        // triggered는 "방금 눌렸다"는 뜻입니다. (GetButtonDown과 동일)
        if (jumpAction.triggered && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // 3. 방향 뒤집기
        if (moveInput > 0 && !isFacingRight) Flip();
        else if (moveInput < 0 && isFacingRight) Flip();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
}