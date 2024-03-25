using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Arrow : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float speed;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.AddForce(transform.right*speed, ForceMode2D.Force);
    }
}
