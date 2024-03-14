using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private float _horizontal;
    private float _speed = 4f;
    private float _jumpingPower = 20f;

    void Update()
    {

    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(_horizontal * _speed, rb.velocity.y);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, _jumpingPower);
        }

        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.6f);
        }
    }

    private bool IsGrounded()
    {
        // OverlapCircle <- 매개변수로 전달할 위치를 기준으로 반지름만큼 원 생성
        //그 영역 내에 충돌체를 가진 게임오브젝트가 있는지 검사
        return Physics2D.OverlapCircle(groundCheck.position, 0.7f, groundLayer);
    }

    public void Move(InputAction.CallbackContext context)
    {
        _horizontal = context.ReadValue<Vector2>().x;
    }
}