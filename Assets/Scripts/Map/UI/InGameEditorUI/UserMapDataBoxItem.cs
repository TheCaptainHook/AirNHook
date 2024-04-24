using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class UserMapDataBoxItem : MonoBehaviour
{
    [SerializeField] Image mapImage;
    [SerializeField] TextMeshProUGUI mapIdText;
    [SerializeField] TextMeshProUGUI createDate;
    public Map map;

    public event Action OnLoadUserMap;


    [SerializeField] Button loadBtn;

    private void Awake()
    {
        loadBtn.onClick.AddListener(() => { CallOnLoadUserMap(); MapEditor.Instance.editorUIController.LoadUserMapEditorInit(map); });
    }


    private void CallOnLoadUserMap()
    {
        OnLoadUserMap?.Invoke();
    }


    public void SetData(UserMapData userMapData)
    {
        map = userMapData.LoadMap();
        mapIdText.text = map.mapID;
        StartCoroutine(CreateSprtie(userMapData.mapImage));

        DateTimeData date = userMapData.LoadDateTimeData();

        createDate.text = $"{date.year}년 {date.month}월 {date.day}일\n{date.hour}시 {date.minute}분 {date.second}초";
    }




    IEnumerator CreateSprtie(byte[] bytes)
    {

        Texture2D texture = new Texture2D(1920, 1080, TextureFormat.ARGB32, false);
        texture.LoadImage(bytes);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 1920, 1080), new Vector2(0.5f, 0.5f), 100f);

        yield return new WaitForSeconds(0.5f);

        while (sprite == null)
        {
            Debug.Log("Loading");
            yield return null;
        }

        mapImage.sprite = sprite;
    }

}
