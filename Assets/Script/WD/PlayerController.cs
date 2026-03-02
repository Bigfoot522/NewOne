using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    
    [Header("Ground Check")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D _rb;
    private Vector2 _moveInput;
    private bool _isGrounded;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        // 캐릭터가 넘어지지 않게 회전 고정
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // Input System Message: OnMove (SendMessage 방식 기준)
    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    // Input System Message: OnJump
    public void OnJump(InputValue value)
    {
        if (value.isPressed && _isGrounded)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
        }
    }

    private void FixedUpdate()
    {
        // 1. 바닥 체크
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // 2. 수평 이동 적용
        _rb.linearVelocity = new Vector2(_moveInput.x * moveSpeed, _rb.linearVelocity.y);
    }

    // 에디터에서 바닥 체크 범위를 시각적으로 확인
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}