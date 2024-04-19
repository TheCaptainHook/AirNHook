using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_EditorTItle : MonoBehaviour
{
    [SerializeField] Button newBtn;
    [SerializeField] Button loadBtn;




    private void NewCreate()
    {
        Instantiate(ResourceManager.Instantiate("Prefabs/MapEditor/MapEditor"));
        MapEditor.Instance.EditorMode_Init();
    }
    private void OpenLoadUI()
    {

    }
}
