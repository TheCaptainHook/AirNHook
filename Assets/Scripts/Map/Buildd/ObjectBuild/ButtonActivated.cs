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
    //public List<ButtonActivatedDoor> linkDoorList;

    Color orgColor;

    bool isRunningCoroutine;

    [Header("Components")]
    private SpriteRenderer spriteRenderer;
    private Animator _animator;
    
    #region StringCache
    private static readonly int IsActivated = Animator.StringToHash("IsActivated");
    #endregion

    bool onPrograss;


    [Header("Data Setting")]
    private ButtonActivatedObject buttonActivatedObject;
    public ButtonActivatedObject ButtonActivatedObject {
        get { return buttonActivatedObject; }
        set { buttonActivatedObject = value;
            ObjectData = new ObjectData(value.id, value.position, value.scale);
            linkId = value.linkId;
            transform.position = value.position;
            transform.localScale = value.scale;
        } }

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
        //if (!NetworkServer.active || !NetworkClient.isConnected) return; //24.05.20
        

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
        if(hit.collider is not null && !turnOff && !onPrograss)
        {
            isPressed = true;
        }
        else if (isPressed && onActive && hit.collider is null && !onPrograss)
        {
            Deactivated();
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(buttonTransform.position, Vector2.up * .5f);
    }
   
    //public void LinkDoor()
    //{
    //    Debug.Log("LInk");
    //    //Vector2 pot = new Vector2(Mathf.Round(transform.position.x * 10f) / 10f, Mathf.Round(transform.position.y * 10f) / 10f);
    //    //transform.position = pot;

    //        foreach (Transform transform in MapEditor.Instance.interactionObjectTransform)
    //        {
    //            if (transform.GetComponent<ButtonActivatedDoor>().linkId == linkId)
    //            {
    //                //linked = true;
    //                ButtonActivatedDoor linkDoor = transform.GetComponent<ButtonActivatedDoor>();
    //                Debug.Log(linkDoor.ButtonActivatedDoorStruct.position);
    //                if(linkDoor != null)
    //                {
    //                    if (linkDoor.buttonActivatedBtnList.Contains(curPosition))
    //                    {
    //                        linkDoor.buttonActivatedBtnList.Remove(curPosition);
    //                    }

    //                    linkDoor.buttonActivatedBtnList.Add(transform.position);
    //                    linkDoorList.Add(linkDoor);
    //                    //curPosition = pot;
    //                }

                  
    //            }
    //        }
        

    //}

    public override void EditorMode_Destroy()
    {

            base.EditorMode_Destroy();
    }


    //public void SetLinkDoor(Vector2 pot, ButtonActivatedDoor door)
    //{
    //    linkDoor = door;
    //    curPosition = pot;
    //    linkId = door.linkId;
    //    transform.position = curPosition;

    //}
    //public void SetLinkDoor(Vector2 pot, ButtonActivatedDoor door) // in game Load
    //{
    //    linkDoorList.Add(door);
    //    curPosition = pot;
    //    linkId = door.linkId;
    //    transform.position = curPosition;

    //}
    //public void SetLinkDoor(Vector2 pot,int linkId) // in game Load
    //{
    //    curPosition = pot;
    //    this.linkId = linkId;
    //    transform.position = curPosition;

    //}

    //public void SetLinkDoor(Vector2 pot,int linkId,Transform interactionDoorTransform) // Editro_Editor
    //{
    //    curPosition = pot;
    //    this.linkId = linkId;
    //    transform.position = curPosition;

    //    foreach (Transform tr in interactionDoorTransform)
    //    {
    //        ButtonActivatedDoor bd = tr.GetComponent<ButtonActivatedDoor>();
    //        if (bd.linkId == linkId)
    //        {
    //            linkDoorList.Add(bd);
    //        }
    //    }
    //}


    public ButtonActivatedObject GetData()
    {
        return new ButtonActivatedObject(id, linkId, transform.position,transform.localScale);
    }



    //IEnumerator Co_ReLinkDoor()
    //{
    //    isRunningCoroutine = true;
    //    while (time > 0)
    //    {
    //        time -= Time.deltaTime;
    //        yield return null;
    //    }
    //    isRunningCoroutine = false;
    //    LinkDoor();
    //}

    void Activation()
    {
        if (onPrograss) return;

        StartCoroutine(Co_Activation());
    }

    void Deactivated()
    {
        if (onPrograss) return;
        if (!onActive) return;
        StartCoroutine(Co_Deactivated());
  
    }


    IEnumerator Co_Activation()
    {
        onPrograss = true;


        _animator.SetBool(IsActivated, true);

        FindLinkDoorAndActivated(true);

        yield return new WaitForSeconds(0.5f);
        onPrograss = false;
    }
    IEnumerator Co_Deactivated()
    {
        onPrograss = true;

        isPressed = false;
        onActive = false;
        
        _animator.SetBool(IsActivated, false);

        FindLinkDoorAndActivated(false);

        yield return new WaitForSeconds(0.5f);
        Debug.Log("Btn Deactivated");
        onPrograss = false;
    }


    void FindLinkDoorAndActivated(bool onActivate)
    {
        foreach(Transform tr in MapEditor.Instance.interactionObjectTransform)
        {
            ButtonActivatedDoor bd = tr.GetComponent<ButtonActivatedDoor>();
            if(bd != null && bd.linkId == linkId)
            {
                if (onActivate)
                {
                    bd.CurActiveBtn = 1;
                }
                else
                {
                    bd.CurActiveBtn = -1;
                }
            }
        }
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
