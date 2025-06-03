using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncapsulationField : MonoBehaviour
{
    private BuildObj obj;
    private BuildObj Main { get { obj ??= GetComponent<BuildObj>(); return obj; } }

    private Transform parent => transform.parent;


    #region  Main
    [Header("Save Data Field")]
    public bool onEncapsulationItem;

    [ReadOnly]
    public bool isCapsuling;


    #endregion

    public void Capsuling()
    {
        
    }
    public void UnCapsuling()
    {

    }


    ///Capsuling
    /// 1. CapsulateField capsulateField = Instantiate(CapsulateField)
    /// 2. capsulateField.Setting()
    /// 3. ConstraintParent, Main : capsulateField, parts : Main.gameObject

    ///
}
