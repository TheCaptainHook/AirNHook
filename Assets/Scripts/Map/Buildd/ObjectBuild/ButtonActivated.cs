using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

//ButtonActivatedDoor가 무조건 있어야함
public class ButtonActivated : MonoBehaviour
{
    public int linkId;


    public LayerMask mask;

    public bool onActive;
    public bool isPressed = false;
    bool linked;
    public float time = 2;
    public Vector2 curPosition;

    public ButtonActivatedDoor linkDoor;
    Color orgColor;

    [Header("Components")]
    SpriteRenderer spriteRenderer;

    Coroutine coroutine;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();  
        orgColor = spriteRenderer.material.color;
    }

    private void Start()
    {
        coroutine = StartCoroutine(Co_ReLinkDoor());
        LinkDoor();
    }
    private void Update()
    {
        if (!linked)
        {
            LinkDoor();
        }

        if(curPosition != new Vector2(Mathf.Round(transform.position.x * 10f) / 10f, Mathf.Round(transform.position.y * 10f) / 10f))
        {
            time = 2;
            StartCoroutine(Co_ReLinkDoor());
        }

        if (isPressed && !onActive)
        {
            Activation();
        }
    }

    

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position+new Vector3(0,.5f,0), Vector2.up*.5f, 1, mask);
        if(hit.collider != null)
        {
            isPressed = true;
        }
        else if(isPressed && onActive)
        {
          Deactivated();
        }
    }
    //<summary>맵 에디터에서 생성시, 데모맵에서 ButtonActivatedDoor오브젝트를 가져오는 코드를 수정해야함
    //mapEditor 상호작용오브젝트 전용 Transform 만들어서 여기다가 모아놓기
    //ButtonActivateBtn 생성하려면 무조건 ButtonActivateDoor가 하나이상 있어야함.
    //맵안에 ButtonActivateDoor 와 일치하는 id 가 없으면 오류, 설치 불가
    //</summary>
    void LinkDoor()
    {
        Vector2 pot = new Vector2(Mathf.Round(transform.position.x*10f)/10f, Mathf.Round(transform.position.y * 10f) / 10f);

        foreach (Transform transform in MapEditor.Instance.interactionObjectTransform)
        {
            if (transform.GetComponent<ButtonActivatedDoor>().linkId == linkId)
            {
                linked = true;
                linkDoor = transform.GetComponent<ButtonActivatedDoor>();
                if (linkDoor.buttonActivatedBtnList.Contains(curPosition))
                {
                    linkDoor.buttonActivatedBtnList.Remove(curPosition);
                    linkDoor.curLinkBtn--;
                }
                linkDoor.curLinkBtn++;
                linkDoor.buttonActivatedBtnList.Add(pot);
                curPosition = pot;
            }
        }
    }

  IEnumerator Co_ReLinkDoor()
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        LinkDoor();
    }
 
    void Activation()
    {
        onActive = true;
        spriteRenderer.material.color = Color.green;
        linkDoor.CurActiveBtn = 1;
    }

    void Deactivated()
    {
        isPressed = false;
        onActive = false;
        spriteRenderer.material.color = orgColor;
        linkDoor.CurActiveBtn = -1;
    }

}
