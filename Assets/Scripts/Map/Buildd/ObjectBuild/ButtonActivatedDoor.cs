using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ButtonActivated 가 1개 이상 존재해야함
public class ButtonActivatedDoor : BuildBase
{
    public int id;
    public int curLinkBtn;
    public int curActiveBtn;
    public int CurActiveBtn {set { curActiveBtn += value; if (curActiveBtn == curLinkBtn) Debug.Log("Open");} }

    
    public List<ButtonActivatedBtn> buttonActivatedBtnList = new List<ButtonActivatedBtn>();

}
 
//id, path, position

//ButtonActivatedDoor 생성 -> ButtonActivateBtn 생성 -> 맵안에 ButtonActivatedDoor탐색, id 일치하면
//ButtonActivatedBtn 과 door link, door에 btn정보 리스트 Add
//door의 list만 데이터로 뽑으면됨