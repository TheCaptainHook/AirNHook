
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
        }
       
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Exit");
        }
    }





    private bool GetFallingPlayer(Collider2D other)
    {
        var dir = (other.transform.position - transform.position).normalized;

        bool x = -0.7f< dir.x && dir.x < 0.7f;
        bool y = dir.y >= 0.74f;
        return x && y;
       

     

    }
}
