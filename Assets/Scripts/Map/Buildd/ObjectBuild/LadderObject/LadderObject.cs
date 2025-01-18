using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public enum LadderType
{
    Cable,
    Chain
}

public class LadderObject : MonoBehaviour
{
   
    public LadderType ladderType;

  
    [SerializeField] Tilemap tileMap;
    [SerializeField] LayerMask checkGroundlayerMask;



    private bool onProgress;

    private int ladderLength = 5;
    private TileBase tileBase;

    private WaitForSeconds waitForSeconds = new(.1f);

    private int curLadderLength;
    [ReadOnly]
    public bool isRopeUnwound;

    private void Awake()
    {
        //test
        tileBase = GetTileBase(ladderType);
    }

    private void Update()
    {
        if (onProgress) return;
        //test
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Unwound();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Rewind();
        }
    }

    //Interactable, 


    public void Unwound()
    {
        isRopeUnwound = true;
        StartCoroutine(UnwoundLadderCo(CheckGround()));
    }

    public void Rewind()
    {
        StartCoroutine(RewindLadderCo()); 
        isRopeUnwound = false;

    }



    #region Util
    private int CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down);
        if (hit.collider == null) { curLadderLength = ladderLength; return ladderLength; }
        else if (((1 << hit.collider.gameObject.layer) & checkGroundlayerMask) != 0)
        {
            int length = Mathf.FloorToInt(transform.position.y - hit.point.y);
            if (length > ladderLength)
            {
                curLadderLength = ladderLength;
                return ladderLength;
            }
            else
            {
                curLadderLength = length;
                return length;
            }

        }

        else
        {
            curLadderLength = ladderLength;
            return ladderLength;
        }
    }
    private TileBase GetTileBase(LadderType ladderType)
    {
        switch (ladderType)
        {
            case LadderType.Chain:
                return Resources.Load<TileBase>(GlobalText.TILEBASE_CHAIN);
            case LadderType.Cable:
                return Resources.Load<TileBase>(GlobalText.TILEBASE_CABLE);
            default:
                return null;
        }
    }

    IEnumerator UnwoundLadderCo(int length)
    {
        onProgress = true;
        for (int i = 1; i <= length; i++)
        {
            Vector3Int pot = new Vector3Int(0, -i, 0);
            tileMap.SetTile(pot, tileBase);
            yield return waitForSeconds;
        }
        onProgress = false;

    }

    IEnumerator RewindLadderCo()
    {
        Debug.Log(curLadderLength);
        onProgress = true;
        for (int i = curLadderLength; i >= 1; i--)
        {     
            Vector3Int pot = new Vector3Int(0, -i, 0);
            tileMap.SetTile(pot, null);
            yield return waitForSeconds;
        }
        onProgress = false;
    }
    #endregion
}
