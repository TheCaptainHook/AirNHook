using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TEST0410 : MonoBehaviour
{
    [SerializeField] Button testBtn;

    private void Awake()
    {
        testBtn.onClick.AddListener(() => { MapEditor.Instance.EditorMode_Init(); });
    }
}
