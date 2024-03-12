using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointObj : MonoBehaviour
{
    [SerializeField] bool isActive;
    [SerializeField] bool placeable;
    [SerializeField] bool isCollision;
    [SerializeField]LayerMask mask;

    [SerializeField] SpriteRenderer mainSprite;
    Color orgColor;

    RaycastHit2D hit;
            
     // asdasd

    private void Awake()
    {
        mainSprite = GetComponent<SpriteRenderer>();
        orgColor = mainSprite.material.color;
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, mask);

            if (hit.collider != null)
            {
                placeable = true;
                mainSprite.color = orgColor;
                Debug.Log(hit.collider.gameObject.GetComponent<BuildObj>().tileType);
            }
            else
            {
                placeable = false;
                mainSprite.color = Color.red;
            }
        }

    }


    bool CheckArea()
    {
        float x = mainSprite.transform.localScale.x/2;

        if (transform.position.x - x >= 0 && transform.position.x+x < MapEditor.Instance.width)
        {
            return true;
        }
        else { return false; }
        
    }

    // 설치하는거,,,
    //상호작용, 버튼을누르면 문이열라는거면,, 하느 클래스에 오브젝트[2] 크기에다가 차례로 넣어서 클릭했을때 설치후 바로 다음 설치할 수있게..흠
    //0311 스폰위치와 탈출위치 만들고 설치하는 것 까지
    //에디터 마우스 컨트롤
    //에디터 유아이

}
