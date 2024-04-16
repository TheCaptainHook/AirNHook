using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class LeverBody : BuildObj,IInteractable
{
    [Header("Info")]
    ObjectTypeEnum objectTypeEnum = ObjectTypeEnum.Interaction;
    public int linkId;
    public List<ButtonActivatedDoor> linkDoorList;
    public LeverHead leverHead;
    [SerializeField] Transform leverHeadTransform;

    [Header("State")]
    public bool onCompletionParts;
    bool onAcitve;
    bool onOperation;

    [Header("Components")]
    Animator animator;


    private static readonly int OnActive = Animator.StringToHash("OnActive");

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Activation();
        }

    }


    public void DataSaveLinkDoor()//Editor_Editor only
    {
        if (linkDoorList.Count > 0)
        {
            foreach (ButtonActivatedDoor linkDoor in linkDoorList)
            {
                if (linkDoor.leverBodyPotiionList.Contains(transform.position))
                {
                    linkDoor.leverBodyPotiionList.Remove(transform.position);
                }
                linkDoor.leverBodyPotiionList.Add(transform.position);
            }
        }
    }

    public void LinkDoor()
    {
        foreach(Transform tr in MapEditor.Instance.interactionObjectTransform)
        {
            if (tr.GetComponent<ButtonActivatedDoor>().linkId == linkId)
            {
                linkDoorList.Add(tr.GetComponent<ButtonActivatedDoor>());
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
                leverHead.AttachToLevelBody(leverHeadTransform);
                this.leverHead = leverHead;

                onCompletionParts = true;

                LinkDoor();
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
        if (!NetworkServer.active || !NetworkClient.isConnected)
            return;

        if (onCompletionParts && !onOperation)
        {
            Activation();
        }
    }

    public ObjectTypeEnum GetObjectType()
    {
        return objectTypeEnum;
    }
}
