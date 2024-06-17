using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaLightningHitBox : MonoBehaviour
{
    [SerializeField] TeslaTower teslaTower;
    private bool onLightningRod;
    private float curLightningRate;

    private Collider2D lightningRodCollider;
    private Collider2D playerColider;

    private LightningRod lightningRod;

    private int coroutineCount;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<LightningRod>())
        {
            lightningRodCollider = collision.GetComponent<Collider2D>();

            if (teslaTower.onCharge && playerColider)
            {
                if (coroutineCount > 2) return;

                onLightningRod = true;
                lightningRod = collision.GetComponent<LightningRod>();
                StartCoroutine(Lightning(lightningRod));
            }
            
        }else if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
           playerColider = collision.GetComponent<Collider2D>();
           if(lightningRodCollider != null)
            {
                OnTriggerEnter2D(lightningRodCollider);
                return;
            }

            teslaTower.Lightning(collision.transform);
        }

    }



    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<LightningRod>())
        {
            onLightningRod = false;
            lightningRodCollider = null;
            lightningRod = null;
            if(playerColider != null)
            {
                OnTriggerEnter2D(playerColider);
            }
        }else if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerColider = null;

        }

    }




    IEnumerator Lightning(LightningRod lightningRod)
    {
        coroutineCount++;
        while (onLightningRod && teslaTower.onCharge)
        {
            curLightningRate -= Time.deltaTime;
            if(curLightningRate <= 0)
            {
                teslaTower.Lightning(lightningRod.hitPoint);
                lightningRod.Electric();
                curLightningRate = teslaTower.lightningRate;
            }
   
            yield return null;
        }
        coroutineCount--;

    }


}
