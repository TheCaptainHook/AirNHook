using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StageSelect : MonoBehaviour
{
    [SerializeField] GameObject _stageSelect;
    private StageButton _stageButton;


    public void DisableSelect()
    {
        _stageSelect.SetActive(false);
    }
}
