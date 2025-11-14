
using System.Collections;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(InteractableObject_Puzzle_1_Item))]
public class Puzzle_1_Item : BuildObj,IDamageable,IRemoveSocketEffect
{

    [Header("Puzzle")]
    public int socketNumber; 
    public bool possibleInsertSocket;

    #region Components
    // private Rigidbody2D rb;
    private Collider2D col;

    private InteractableObject_Puzzle_1_Item Net_Item => GetComponent<InteractableObject_Puzzle_1_Item>();

    #endregion

    #region Network Field
    //private bool Parts => Net_Item.parts ? true : false;
    #endregion

    // private void Awake()
    // {
    //     rb = GetComponent<Rigidbody2D>();
    //     // DissolveInitSetting();
    // }

    #region Socket
    public Puzzle_1_Parts parts;
    public void ContectParts(Puzzle_1_Parts parts)
    {
        this.parts = parts;
    }

    public override void Clean()
    {
        DissolveClean();
    }
    public void Set_Item()
    {
        canRespawn = true;
        DissolveInitSetting();
    }
    // public void Server_Dissolve()
    // {
    //     if(!NetworkServer.active)
    //     {
    //         StartCoroutine(WaitUntilReadyThenExecute());
    //     }
    //     else
    //     {
    //         Net_Item.Server_Dissolve();
    //     }
    // }

    // IEnumerator WaitUntilReadyThenExecute()
    // {
    //     yield return new WaitUntil(() => NetworkClient.ready);
    //     Net_Item.Server_Dissolve();
    // }

    #endregion

    #region Effect
    private float forceStrength = 7f;
    private float forceDefault = 3f;
    private float horizontalVariation = 1f;


    public void RemoveSocketEffect(bool power = false)
    {
        float xForce = Random.Range(-horizontalVariation, horizontalVariation);
        if (power)
        {
            _rb.AddForce(new Vector2(xForce, forceStrength), ForceMode2D.Impulse);
        }
        else
        {
            _rb.AddForce(new Vector2(xForce, forceDefault), ForceMode2D.Impulse);
        }

    }
    #endregion

    public void Server_SetOrgPosition(Vector3 pos) //Server
    {

        position = pos;
    }

}
