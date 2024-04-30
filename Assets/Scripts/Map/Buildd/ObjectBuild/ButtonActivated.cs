using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Rendering;

//ButtonActivatedDoor가 무조건 있어야함
public class ButtonActivated : BuildObj
{
    public int linkId;


    public LayerMask mask;
    //bool linked;
    public bool onActive;
    public bool isPressed = false;
    public float time = 2;
    public Vector2 curPosition;

    public Transform buttonTransform;
    
    //public ButtonActivatedDoor linkDoor;
    public List<ButtonActivatedDoor> linkDoorList;

    Color orgColor;

    bool isRunningCoroutine;

    [Header("Components")]
    private SpriteRenderer spriteRenderer;
    private Animator _animator;
    
    #region StringCache
    private static readonly int IsActivated = Animator.StringToHash("IsActivated");
    #endregion
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        orgColor = spriteRenderer.material.color;
    }

    //private void Start()
    //{
    //    LinkDoor();
    //}
    private void Update()
    {
        if (!NetworkServer.active || !NetworkClient.isConnected) return;

        if(Managers.Game.CurrentState == GameState.Editor)
        {
            if (curPosition != new Vector2(Mathf.Round(transform.position.x * 10f) / 10f, Mathf.Round(transform.position.y * 10f) / 10f))
            {
                if (isRunningCoroutine) { StopCoroutine(Co_ReLinkDoor()); isRunningCoroutine = false; }
                time = 2;
                Debug.Log("CocoCO");
                StartCoroutine(Co_ReLinkDoor());
            }
        }
        

        if (isPressed && !onActive)
        {
            onActive = true;
            Debug.Log("Activation Updata");
            Activation();
        }
    }

    

    private void FixedUpdate()
    {
        if (!NetworkServer.active || !NetworkClient.isConnected) return;
        
        RaycastHit2D hit = Physics2D.Raycast(buttonTransform.position, Vector2.up, 1, mask);
        if(hit.collider is not null && !turnOff)
        {
            isPressed = true;
        }
        else if (isPressed && onActive && hit.collider is null)
        {
            Debug.Log("Deactive");
            Deactivated();
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(buttonTransform.position, Vector2.up * .5f);
    }
   
    public void LinkDoor()
    {
        Debug.Log("LInk");
        Vector2 pot = new Vector2(Mathf.Round(transform.position.x * 10f) / 10f, Mathf.Round(transform.position.y * 10f) / 10f);
        transform.position = pot;
        if (linkDoorList.Count > 0)
        {
            foreach (ButtonActivatedDoor linkDoor in linkDoorList)
            {
                if (linkDoor.buttonActivatedBtnList.Contains(curPosition))
                {
                    linkDoor.buttonActivatedBtnList.Remove(curPosition);
                }
                linkDoor.buttonActivatedBtnList.Add(pot);
                curPosition = pot;
            }

        }
        else
        {
            foreach (Transform transform in MapEditor.Instance.interactionObjectTransform)
            {
                if (transform.GetComponent<ButtonActivatedDoor>().linkId == linkId)
                {
                    //linked = true;
                    ButtonActivatedDoor linkDoor = transform.GetComponent<ButtonActivatedDoor>();
                    Debug.Log(linkDoor.ButtonActivatedDoorStruct.position);
                    if(linkDoor != null)
                    {
                        if (linkDoor.buttonActivatedBtnList.Contains(curPosition))
                        {
                            linkDoor.buttonActivatedBtnList.Remove(curPosition);
                        }

                        linkDoor.buttonActivatedBtnList.Add(pot);
                        linkDoorList.Add(linkDoor);
                        curPosition = pot;
                    }

                  
                }
            }
        }

    }

    public override void EditorMode_Destroy()
    {
        foreach (ButtonActivatedDoor linkDoor in linkDoorList)
        {
            if (linkDoor.buttonActivatedBtnList.Contains(curPosition))
            {
                linkDoor.buttonActivatedBtnList.Remove(curPosition);
                linkDoor.curLinkBtn--;
            }
        }
            base.EditorMode_Destroy();
    }


    //public void SetLinkDoor(Vector2 pot, ButtonActivatedDoor door)
    //{
    //    linkDoor = door;
    //    curPosition = pot;
    //    linkId = door.linkId;
    //    transform.position = curPosition;

    //}
    public void SetLinkDoor(Vector2 pot, ButtonActivatedDoor door) // in game Load
    {
        linkDoorList.Add(door);
        curPosition = pot;
        linkId = door.linkId;
        transform.position = curPosition;

    }
    public void SetLinkDoor(Vector2 pot,int linkId) // in game Load
    {
        curPosition = pot;
        this.linkId = linkId;
        transform.position = curPosition;

    }

    public void SetLinkDoor(Vector2 pot,int linkId,Transform interactionDoorTransform) // Editro_Editor
    {
        curPosition = pot;
        this.linkId = linkId;
        transform.position = curPosition;

        foreach (Transform tr in interactionDoorTransform)
        {
            ButtonActivatedDoor bd = tr.GetComponent<ButtonActivatedDoor>();
            if (bd.linkId == linkId)
            {
                linkDoorList.Add(bd);
            }
        }
    }

    IEnumerator Co_ReLinkDoor()
    {
        isRunningCoroutine = true;
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        isRunningCoroutine = false;
        LinkDoor();
    }

    void Activation()
    {
        if (linkDoorList.Count == 0)
        {
            LinkDoor();
        }


        // spriteRenderer.material.color = Color.green;
        foreach(ButtonActivatedDoor linkDoor in linkDoorList)
        {
            
            linkDoor.CurActiveBtn = 1;
        }

        _animator.SetBool(IsActivated, true);
    }

    void Deactivated()
    {
        if (!onActive) return;
        isPressed = false;
        onActive = false;
        // spriteRenderer.material.color = orgColor;
        foreach (ButtonActivatedDoor linkDoor in linkDoorList)
        {
            linkDoor.CurActiveBtn = -1;
        }
        _animator.SetBool(IsActivated, false);
    }

    public override void TurnOff()
    {
        base.TurnOff();
        turnOff = true;

        if (isPressed && onActive)
        {
            Deactivated();
        }




    }

    public override void TurnOn()
    {
        base.TurnOn();
        turnOff = false;
    }

}
