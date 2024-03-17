using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform floorCheck;
    public LayerMask floorLayer;

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
        if (context.performed && IsFloor())
        {
            rb.velocity = new Vector2(rb.velocity.x, _jumpingPower);
        }

        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.6f);
        }
    }

    private bool IsFloor()
    {
        // OverlapCircle <- �Ű������� ������ ��ġ�� �������� ��������ŭ �� ����
        //�� ���� ���� �浹ü�� ���� ���ӿ�����Ʈ�� �ִ��� �˻�
        return Physics2D.OverlapCircle(floorCheck.position, 0.7f, floorLayer);
    }

    public void Move(InputAction.CallbackContext context)
    {
        _horizontal = context.ReadValue<Vector2>().x;
    }
}