using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="MapOutLineSO",menuName ="Create Map SO/Map OutLine SO",order = 0)]
public class MapOutLineSO : ScriptableObject
{
    public GameObject bottom;
    public GameObject top;
    public GameObject left;
    public GameObject right;
    public GameObject curveTL;
    public GameObject curveTR;
    public GameObject curveBL;
    public GameObject curveBR;
}
