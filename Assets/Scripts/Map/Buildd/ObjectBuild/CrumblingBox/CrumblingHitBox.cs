
using Mirror;
using UnityEngine;

public class CrumblingHitBox : MonoBehaviour
{
    //[SerializeField]CrumblingBox parent;
    [SerializeField] CrumblingBox_Net net;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && GetFallingPlayer(collision))
        {
            if (NetworkServer.active) net.Server_SetCrumbringIndex();
            Debug.Log("Hit");
        }
       
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Exit");
        }
    }



    float cosA = 45;

    private bool GetFallingPlayer(Collider2D other)
    {
        Vector2 toPlayer = (Vector2)other.bounds.center - (Vector2)transform.position;

        var dir = toPlayer.normalized;

        float cos = Mathf.Cos(cosA * Mathf.Deg2Rad);

        bool angleOk = Vector2.Dot(dir, transform.up) >= cos;

        return angleOk;

    }
}
