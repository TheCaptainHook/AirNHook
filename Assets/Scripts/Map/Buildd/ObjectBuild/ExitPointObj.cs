using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ExitPointObj : BuildBase
{
    [Header("State")]
    [SerializeField] bool stageClear;

    [Header("Info")]
    [SerializeField] int condition_KeyAmount;
    private int current_KeyAmount;
    public int Current_KeyAmount {
        get { return current_KeyAmount; }
        set { current_KeyAmount++;
            if (current_KeyAmount >= condition_KeyAmount) Debug.Log("Clear"); } }
    event Action OnCheckKey;
    bool isClear;

    //private void FixedUpdate()
    //{
    //    BuildCheck();
    //}


    public void Init(int condition_keyAmount)
    {
        this.condition_KeyAmount = condition_keyAmount;
    }

    void GetKey(GameObject gameObject)
    {
        Destroy(gameObject);
        Current_KeyAmount = 1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Key"))
        {
            GetKey(collision.gameObject);
            Debug.Log(current_KeyAmount);
        }
    }
}
