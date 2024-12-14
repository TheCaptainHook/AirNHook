using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Puzzle_1 : MonoBehaviour
{
   private void Update(){
    if(Input.GetKeyDown(KeyCode.P)){
         GameObject obj = Managers.Stage.CmdBatchObject("Puzzle_1_Item (1)");
        obj.transform.position = Vector2.zero;
    }
   
   }
}
