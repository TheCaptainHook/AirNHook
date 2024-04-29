using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassTheMapTest : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    //[SerializeField] private BoxCollider2D _bc;

    [SerializeField] private Vector2 _nowPosition;
    [SerializeField] private Vector2 _pastPosition;
    private float _time;
    private bool _isWallCheck;

    private void FixedUpdate()
    {
        //플레이어가 벽안에 끼이는순간
        //if () { _isWallCheck = true; } 
    
        if (!_isWallCheck)
        {
            _time += Time.deltaTime;
            _nowPosition = _rb.transform.position;
    
            //후크는 에어에게 빨려들어가있는상태일때 예전포지션갱신을 멈춰야할듯?
            //에어 공기총때문에 맵밖으로 나가게되면 위치를 맵밖으로 기억해버리기때문에
            if (_time >= 0.3f)
            {
                _pastPosition = _rb.transform.position;
                _time = 0;
            }
        }
        else
        {
            //벡터방향 구하기
            var directionVector = (_pastPosition - _nowPosition).normalized;
            //캐릭터의 움직임이 멈춤 , 키네마틱 체크 , 벨로시티 0으로하고 움직이고있던 반대방향으로 캐릭터 이동
            
            
            _isWallCheck = false;
        }
    }
}
