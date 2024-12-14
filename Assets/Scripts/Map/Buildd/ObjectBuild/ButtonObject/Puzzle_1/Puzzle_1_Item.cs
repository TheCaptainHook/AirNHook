using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(InteractableObject_Puzzle_1_Item))]
public class Puzzle_1_Item : MonoBehaviour
{

    [Header("Puzzle")]
    public int socketNumber; //1,2,3
    public bool possibleInsertSocket;
    public Puzzle_1_Parts parts;
    #region Components
    private Rigidbody2D rb;
    private Collider2D col;
    #endregion

    

    private void Awake(){
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

   #region Socket
   public void InsertSocket(){
    if(parts){
        parts.InsertSocket(this);
        possibleInsertSocket = false;
    }
   }

   public bool GetPossibleInsertSocket(){
    return possibleInsertSocket;
   }

   public void PossibleInsertSocket(Puzzle_1_Parts parts){
    this.parts = parts;
    possibleInsertSocket = true;
   }
   public void UnPossibleInsertSocket(){
    parts = null;
    possibleInsertSocket = false;
   }






   #endregion

   



}
