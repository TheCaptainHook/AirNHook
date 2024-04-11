using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _stageText;
    public bool stageClear;
    public void StageSelect(PlayerData playerData)
    {
        _stageText.text = playerData.StageID.ToString();
        stageClear = playerData.StageClear;
    }

    //1~5의 버튼을 누르면 누른번호에 해당되는 StageClear값이 true로 바뀌고 메인화면의 save를 누르면
    //바뀐값이 저장되도록
    public void Click()
    {

    }
}
