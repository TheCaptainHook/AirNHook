
using UnityEngine;

public class CrumblingHitBox : MonoBehaviour
{
    [SerializeField]CrumblingBox parent;




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            parent.Crumbling();
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
