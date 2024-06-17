using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaTowerHitBox : MonoBehaviour
{
    [SerializeField] TeslaTower teslaTower;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            teslaTower.EnterBoundaryPlayerAmount = 1;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            teslaTower.EnterBoundaryPlayerAmount = -1;
        }
    }
}
