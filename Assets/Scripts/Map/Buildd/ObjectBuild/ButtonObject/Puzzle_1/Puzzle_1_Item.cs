
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(InteractableObject_Puzzle_1_Item))]
public class Puzzle_1_Item : MonoBehaviour
{

    [Header("Puzzle")]
    public int socketNumber; //1,2,3
    public bool possibleInsertSocket;
    //public Puzzle_1_Parts parts;

    #region Components
    private Rigidbody2D rb;
    private Collider2D col;

    private InteractableObject_Puzzle_1_Item Net_Item => GetComponent<InteractableObject_Puzzle_1_Item>();

    #endregion

    #region Network Field
    private bool Parts => Net_Item.parts ? true : false;
    #endregion

    private void Awake(){
        rb = GetComponent<Rigidbody2D>();
        //col = GetComponent<Collider2D>();
    }

   #region Socket
   public void InsertSocket(){
    if(Parts){
         if(Net_Item.parts.TryGetComponent(out Puzzle_1_Parts component))
            {
                component.InsertSocket(gameObject);
            }
        //parts.InsertSocket(this);
        //possibleInsertSocket = false;
        //col.enabled = false;
    }
   }
    public void RemoveSocket(bool onEffect = false)
    {
        //col.enabled = true;
        //rb.gravityScale = 1;
        Net_Item.Cmd_SetOnInsert(false);
        //RemoveSocketEffect(onEffect);
    }

    public void Net_HandleSetParts(Puzzle_1_Parts parts){
        if(parts == null){
            Net_Item.HandleSetParts(null);
        }else
        Net_Item.HandleSetParts(parts.gameObject);
    }

   //public void PossibleInsertSocket(Puzzle_1_Parts parts){
   // this.parts = parts;
   // possibleInsertSocket = true;
   //}
   //public void UnPossibleInsertSocket(){
   // parts = null;
   // possibleInsertSocket = false;
   //}


    #endregion

    #region Effect
    private float forceStrength = 7f;
    private float forceDefault = 3f;
    private float horizontalVariation = 1f;
    public void RemoveSocketEffect(bool Power = false)
    {
        float xForce = Random.Range(-horizontalVariation, horizontalVariation);

        if (Power)
        {
            rb.AddForce(new Vector2(xForce, forceStrength), ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(new Vector2(xForce, forceDefault), ForceMode2D.Impulse);
        }

       
    }
    #endregion

    #region Util
    //public Vector2 GetPartsPosition()
    //{
    //    //return (parts == null) ? Vector2.zero : parts.transform.position;
    //    return (Net_Item.parts == null) ? Vector2.zero : Net_Item.parts.transform.position;
    //}

    //public bool GetPossibleInsertSocket()
    //{
    //    //return possibleInsertSocket;
    //    return Net_Item.possibleInsertSocket;
    //}
    #endregion




}
