using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    Pooling pool;

    RaycastHit2D hit;

    public LayerMask layerMask;
    private bool isShot;
    public float cooltime;
    private float curtime;
    private void Awake()
    {
        pool = GetComponent<Pooling>();
        pool.CreatePoolItem(transform);
    }

    private void Update()
    {
        if (isShot)
        {
            curtime -= Time.deltaTime;
            if(curtime <= 0)
            {
                isShot = false;
                
            }
        }
    }


    private void FixedUpdate()
    {
        hit = Physics2D.Raycast(transform.position, transform.right, 100f, layerMask);

        if (hit)
        {
            if (!isShot)
            {
                isShot = true;
                curtime = cooltime;


                float z = Mathf.Atan2(transform.right.y, transform.right.x) + Random.Range(-5,5);
                GameObject obj = pool.GetPoolItem("Arrow");
                obj.transform.right = transform.right;
                obj.transform.Rotate(transform.forward * z);
                obj.GetComponent<Projectile_Arrow>().Reset();
                obj.transform.position = transform.position;
                obj.SetActive(true);
                pool.Destroy(obj, 10f);
            }
          
        }
    }



}
