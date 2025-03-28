
using Mirror;
using UnityEngine;

public class CrumblingHitBox : MonoBehaviour
{
    //[SerializeField]CrumblingBox parent;
    [SerializeField] CrumblingBox_Net net;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
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
}
