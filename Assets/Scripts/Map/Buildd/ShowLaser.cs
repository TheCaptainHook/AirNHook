using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShowLaser : MonoBehaviour
{
    [ReadOnly]
    public LaserObject laserObject;
    public void Setting(){
        laserObject = GetComponent<LaserObject>();
        laserObject.Editor_Awake();
    }

}
