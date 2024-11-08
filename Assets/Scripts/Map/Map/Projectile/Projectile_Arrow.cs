using System.Collections;
using UnityEngine;

public class Projectile_Arrow : ProjectileEntity
{

    public override void Reset()
    {
        rb.gravityScale = 0;
        _collider.enabled = true;
        onHit = false;
    }
   

    private void OnDrawGizmosSelected()
    {
        // 스피어 캐스트를 그리기 위해 씬 상에 범위를 표시
        Gizmos.color = Color.red;
        float hitDistance =rb.velocity.magnitude * Time.fixedDeltaTime * 1.5f;
        Gizmos.DrawRay(transform.position+(transform.right*0.5f), transform.right * hitDistance);
    }
}
