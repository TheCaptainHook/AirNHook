using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaLightningHitBox : MonoBehaviour
{
    [SerializeField] TeslaTower teslaTower;
    bool onLightningRod;
    public float curLightningRate;

    public Collider2D lightningRodCollider;
    public Collider2D playerColider;

    private LightningRod lightningRod;

    Coroutine coroutine;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<LightningRod>())
        {
            lightningRodCollider = collision.GetComponent<Collider2D>();

            if (teslaTower.onCharge)
            {
                onLightningRod = true;
                lightningRod = collision.GetComponent<LightningRod>();
                coroutine = StartCoroutine(Lightning(lightningRod));
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
        while (onLightningRod)
        {
            curLightningRate -= Time.deltaTime;
            if(curLightningRate <= 0)
            {
                teslaTower.Lightning(lightningRod.hitPoint);
                curLightningRate = teslaTower.lightningRate;
            }

            yield return null;
        }

    }


}
