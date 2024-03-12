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




    #region Map Size UI
    void MapSizeInit()
    {
        widthInputField.text = Mathf.Clamp(int.Parse(widthInputField.text),10,maxWidth).ToString();
    }

    void ValueChanged()
    {
        widthInputField.text = Mathf.Clamp(int.Parse(widthInputField.text), 10, maxWidth).ToString();
        heightInputField.text = Mathf.Clamp(int.Parse(heightInputField.text), 10, maxHeight).ToString();
    }
    #endregion



}
