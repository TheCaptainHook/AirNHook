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
            if (current_KeyAmount >= condition_KeyAmount) MoveNextStage(); } }

    public GameObject nextPosition;
    public Vector2 nextPot;


    //todo 0320
    public string nextMapId;
    //todo

    event Action OnCheckKey;
    bool isClear;

    //private void FixedUpdate()
    //{
    //    BuildCheck();
    //}

    public ExitObjStruct GetExitObjectStruct()
    {
        Vector2 pot;
        if (nextPosition == null) pot = new Vector2(999, 999);
        else pot = nextPosition.transform.position;

        return new ExitObjStruct(id,transform.position, condition_KeyAmount, pot);
    }
    
    
    public void SetData(ExitObjStruct data,Transform transform)
    {
        condition_KeyAmount = data.condition_KeyAmount;
        nextPot = data.nextPosition;
        if(nextPot != new Vector2(999, 999))
        {
            GameObject spawnDoor = Instantiate(Resources.Load<GameObject>("Prefabs/Map/Object/SpawnPoint"));
            spawnDoor.transform.position = data.nextPosition;
            spawnDoor.transform.SetParent(transform);
            nextPosition = spawnDoor;
        }
       
    }

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

    public void MoveNextStage()
    {
        if(nextPot == new Vector2(999f, 999f))
        {
            Debug.Log("Stage All Clear");
        }
        else
        {
            Debug.Log("Next Stage");
        }

    }
}
