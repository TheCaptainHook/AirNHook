using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class LeverBody : BuildObj,IInteractable
{
    [Header("Info")]
    ObjectTypeEnum objectTypeEnum = ObjectTypeEnum.Interaction;
    public int linkId;
    public List<ButtonActivatedDoor> linkDoorList;
    public LeverHead leverHead;
    public Transform attachedLeverHead;
    [SerializeField] Transform leverHeadTransform;
    public Vector2 curPosition;
    private LeverBodyNet _leverBodyNet;

    public float time = 2;
    bool isRunningCoroutine;

    [Header("State")]
    public bool onCompletionParts;
    bool onAcitve;
    bool onOperation;

    [Header("Components")]
    Animator animator;


    private static readonly int OnActive = Animator.StringToHash("OnActive");
    private static readonly int OnCompletion = Animator.StringToHash("OnCompletion");

    private void Awake()
    {
        _leverBodyNet = GetComponent<LeverBodyNet>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        //if (!linked)
        //{
        //    LinkDoor();
        //}

        if (curPosition != new Vector2(Mathf.Round(transform.position.x * 10f) / 10f, Mathf.Round(transform.position.y * 10f) / 10f))
        {
            if (isRunningCoroutine) { StopCoroutine(Co_ReLinkDoor()); isRunningCoroutine = false; }
            time = 2;
            StartCoroutine(Co_ReLinkDoor());
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
    //public void DataSaveLinkDoor()//Editor_Editor only
    //{
    //    if (linkDoorList.Count > 0)
    //    {
    //        foreach (ButtonActivatedDoor linkDoor in linkDoorList)
    //        {
    //            if (linkDoor.leverBodyPotiionList.Contains(transform.position))
    //            {
    //                linkDoor.leverBodyPotiionList.Remove(transform.position);
    //            }
    //            linkDoor.leverBodyPotiionList.Add(transform.position);
    //        }
    //    }
    //}

    public void LinkDoor()
    {
        Vector2 pot = new Vector2(Mathf.Round(transform.position.x * 10f) / 10f, Mathf.Round(transform.position.y * 10f) / 10f);
        transform.position = pot;

        if (linkDoorList.Count > 0)
        {
            foreach (ButtonActivatedDoor linkDoor in linkDoorList)
            {
                if (linkDoor.leverBodyPotiionList.Contains(curPosition))
                {
                    linkDoor.leverBodyPotiionList.Remove(curPosition);
                    linkDoor.curLinkBtn--;
                }
                linkDoor.leverBodyPotiionList.Add(pot);
                linkDoor.curLinkBtn++;
                curPosition = pot;
            }

        }
        else
        {
            foreach (Transform transform in MapEditor.Instance.interactionObjectTransform)
            {
                if (transform.GetComponent<ButtonActivatedDoor>().linkId == linkId)
                {
                    ButtonActivatedDoor linkDoor = transform.GetComponent<ButtonActivatedDoor>();

                    if (linkDoor.leverBodyPotiionList.Contains(curPosition))
                    {
                        linkDoor.leverBodyPotiionList.Remove(curPosition);
                        linkDoor.curLinkBtn--;
                    }

                    linkDoor.leverBodyPotiionList.Add(pot);
                    linkDoor.curLinkBtn++;
                    linkDoorList.Add(linkDoor);
                    curPosition = pot;
                }
            }
        }


    }

    public void UnLinkDoor()
    {
        linkDoorList.Clear();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<LeverHead>())
        {
            if (!onCompletionParts)
            {
                LeverHead leverHead = collision.gameObject.GetComponent<LeverHead>();              
                leverHead.AttachToLevelBody();
                this.leverHead = leverHead;

                if(Managers.Game.CurrentState != GameState.Editor)
                {
                    collision.transform.GetChild(0).gameObject.SetActive(false);
                    Destroy(collision.gameObject, 1f);
                }
       
                attachedLeverHead.gameObject.SetActive(true);
                onCompletionParts = true;
                animator.SetTrigger(OnCompletion);
            }
           
        }
    }

    public void SetLinkDoor(Vector2 pot, int linkId, Transform interactionDoorTransform)
    {

        curPosition = pot;
        transform.position = pot;
        this.linkId = linkId;

        foreach (Transform tr in interactionDoorTransform)
        {
            ButtonActivatedDoor bd = tr.GetComponent<ButtonActivatedDoor>();
            if (bd.linkId == linkId)
            {
                linkDoorList.Add(bd);
            }
        }
    }
    
    public void Activation()
    {
        foreach (ButtonActivatedDoor door in linkDoorList)
        {
            onAcitve = !onAcitve;
            StartCoroutine(Co_Operation(door));
        }
    }

    public override void TurnOn()
    {
        base.TurnOn();
        
    }
    public override void TurnOff()
    {
        if (onCompletionParts)
        {
            leverHead.DetachToLevelBody();
            onCompletionParts = false;
        }
        base.TurnOff();

    }


    IEnumerator Co_Operation(ButtonActivatedDoor door)
    {
        onOperation = true;
        if (onAcitve)
        {
            animator.SetBool(OnActive, true);
            AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(0);
            Debug.Log(animationState.length);
            door.CurActiveBtn = 1;
            yield return new WaitForSeconds(animationState.length+0.5f);
        }
        else
        {
            animator.SetBool(OnActive, false);
            AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(0);
            door.CurActiveBtn = -1;
            yield return new WaitForSeconds(animationState.length+0.5f);
            
        }
        onOperation = false;
        

    }


    public void Interaction(Transform accessor = null)
    {
        if (onCompletionParts && !onOperation)
        {
            _leverBodyNet.CmdLeverActivate();
        }
        
    }

    public ObjectTypeEnum GetObjectType()
    {
        return objectTypeEnum;
    }
}
