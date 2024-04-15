using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelect : MonoBehaviour
{
    [SerializeField] GameObject _stageSelect;
    public void DisableSelect()
    {
        _stageSelect.SetActive(false);
    }
}
