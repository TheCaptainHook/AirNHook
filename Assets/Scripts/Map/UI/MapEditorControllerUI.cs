using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class MapEditorControllerUI : MonoBehaviour
{
    [Header("Map Size")]
    int maxWidth = 50;
    int maxHeight= 50;
    [SerializeField] TMP_InputField widthInputField;
    [SerializeField] TMP_InputField heightInputField;
    [SerializeField] Button initBtn;
    [SerializeField] TextMeshProUGUI messageText;

    private void Awake()
    {
        initBtn.onClick.AddListener(MapSizeInit);
    }


    #region Map Size UI
    void MapSizeInit()
    {
        MapEditor.Instance.SetMapSize(int.Parse(widthInputField.text), int.Parse(heightInputField.text));
    }

   
  
    #endregion



}
