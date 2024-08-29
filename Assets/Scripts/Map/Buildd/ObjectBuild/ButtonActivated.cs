using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

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
    
    Color orgColor;

    bool isRunningCoroutine;

    [Header("Components")]
    private SpriteRenderer spriteRenderer;
    private Animator _animator;
    
    #region StringCache
    private static readonly int IsActivated = Animator.StringToHash("IsActivated");
    #endregion

    bool onPrograss;


    private List<Vector2> targetPosition; //TODO 0829
    [SerializeField] List<GameObject> targetObjects;//TODO 0829

    [Header("Data Setting")]
    private ButtonObjectStruct buttonObjectData;
    public ButtonObjectStruct ButtonObjectData {
        get { return buttonObjectData; }
        set { buttonObjectData = value;
            ObjectData = new ObjectData(value.id, value.position, value.scale);
            transform.position = value.position;
            transform.localScale = value.scale;
            targetPosition = value.targetPositions;
        } }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        orgColor = spriteRenderer.material.color;
    }

    private void Update()
    {
        //if (!NetworkServer.active || !NetworkClient.isConnected) return; //24.05.20        

        if (isPressed && !onActive)
        {
            onActive = true;
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

    public override void EditorMode_Destroy()
    {   
            base.EditorMode_Destroy();
    }

#region  GET,SET

    public override void SetData<T>(T data)
    {
        try{
            if (typeof(T) == typeof(ButtonObjectStruct))
        {
         ButtonObjectStruct buttonData = (ButtonObjectStruct)(object)data;
         ButtonObjectData = buttonData;

        //Set Target

        FindTargetObject(ButtonObjectData.targetPositions);

        }
        }catch(Exception ex){
                Debug.Log($"{ex}\n{typeof(T)}");
        }
        

        
    }
    public override T GetData<T>()
    {
        if(typeof(T) == typeof(ButtonObjectStruct)){
            return (T)(object)new ButtonObjectStruct(id,GetTargetPositions(),transform.position,transform.localScale);
        }

        return default(T);
    }
#endregion

    #region Util
    private List<Vector2> GetTargetPositions(){
        List<Vector2> list = new();

        foreach(GameObject obj in targetObjects){
            list.Add(obj.transform.position);
        }

        return list;
    }

    private void FindTargetObject(List<Vector2> list){

        List<GameObject> objList = new();

        foreach(Vector2 vec in list){
            foreach(Transform tr in MapEditor.Instance.buttonActivatedObjectTransform){
             BuildObj buildObj = tr.GetComponent<BuildObj>();
              if(buildObj != null){
                if(buildObj.position == vec){
                    objList.Add(tr.gameObject);
                }
             }
        }

        targetObjects = objList;
        }

       
    }

    #endregion


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

        PrograssButtonActivatedObject(true);

        yield return new WaitForSeconds(0.5f);
        onPrograss = false;
    }
    IEnumerator Co_Deactivated()
    {
        onPrograss = true;

        isPressed = false;
        onActive = false;
        
        _animator.SetBool(IsActivated, false);

        PrograssButtonActivatedObject(false);

        yield return new WaitForSeconds(0.5f);
        onPrograss = false;
    }


private void PrograssButtonActivatedObject(bool onActivate){

}



    // void FindLinkDoorAndActivated(bool onActivate)
    // {
    //     foreach(Transform tr in MapEditor.Instance.interactionObjectTransform)
    //     {
    //         ButtonActivatedDoor bd = tr.GetComponent<ButtonActivatedDoor>();
    //         if(bd != null && bd.linkId == linkId)
    //         {
    //             if (onActivate)
    //             {
    //                 bd.CurActiveBtn = 1;
    //             }
    //             else
    //             {
    //                 bd.CurActiveBtn = -1;
    //             }
    //         }
    //     }
    // }



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
