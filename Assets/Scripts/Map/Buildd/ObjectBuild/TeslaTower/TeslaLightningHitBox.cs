using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaLightningHitBox : MonoBehaviour
{
    [SerializeField] TeslaTower teslaTower;
    public float curLightningRate;

    Collider2D lightningRodCollider;

    Coroutine _Coroutine;



    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<LightningRod>())
        {
            lightningRodCollider = collision;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (lightningRodCollider)
            {
                _Coroutine = StartCoroutine(Lightning(lightningRodCollider.gameObject));
                return;
            }

            _Coroutine = StartCoroutine(Lightning(collision.gameObject));
        }

    }

    public void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.GetComponent<LightningRod>())
        {
            lightningRodCollider = null;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            StopCoroutine(_Coroutine);
        }
       
    }


    IEnumerator Lightning(GameObject target)
    {
        while (teslaTower.onCharge)
        {
            curLightningRate -= Time.deltaTime;
            if(curLightningRate <= 0)
            {   
                teslaTower.Lightning(target);
                curLightningRate = teslaTower.lightningRate;
            }
   
            yield return null;
        }
        

    }


    

}
