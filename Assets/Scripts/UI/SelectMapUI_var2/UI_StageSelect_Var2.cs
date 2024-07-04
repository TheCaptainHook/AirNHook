using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class UI_StageSelect_Var2 : UI_Base
{

    public bool onInteractable = true;

    [Header("Current Item")]
    Button currentMainAndUserMapChangeBtn;

    [Header("Main, User Change Button")]
    Color activationColor = new Color(123f/255f,122f/255f,122f/255f,1);
    Color deactivationColor = Color.white;
    [SerializeField] float fadeSpeed;
    [SerializeField] Button mainBtn;
    [SerializeField] Button userBtn;


    [Header("Main Stage Select")]
    [SerializeField] GameObject mainMapSelectContainer;
    [SerializeField] GameObject userMapSelectContainer;


    [Header("Etc Button")]
    [SerializeField] Button exitBtn;
    [SerializeField] Button spawnKeyBtn;

    public Transform container;

    GameObject key;



    //event Action<string> OnSelectMap;

    /// <summary>
    /// UI_StageSelect_Var2 -> MainMapSelectContainer -> MainMapStageSelectItem -> StageSelectScrollView -> MainMapItem
    /// </summary>

    private void Awake()
    {
        //Main, User Change Button
        mainBtn.onClick.AddListener(() => { if (!onInteractable) return;
            //reset
            if (currentMainAndUserMapChangeBtn != null && mainBtn != currentMainAndUserMapChangeBtn)
            {
                StartCoroutine(FadeOutChangeBtn(currentMainAndUserMapChangeBtn));
            }

            currentMainAndUserMapChangeBtn = mainBtn;
            StartCoroutine(FadeInChangeBtn(mainBtn));

            mainMapSelectContainer.SetActive(true);
            userMapSelectContainer.SetActive(false);

        });

        userBtn.onClick.AddListener(() => { if (!onInteractable) return;
            //reset
            if (currentMainAndUserMapChangeBtn != null && userBtn != currentMainAndUserMapChangeBtn)
            {
                StartCoroutine(FadeOutChangeBtn(currentMainAndUserMapChangeBtn));
            }

            currentMainAndUserMapChangeBtn = userBtn;
            StartCoroutine(FadeInChangeBtn(userBtn));


            if (mainMapSelectContainer.activeSelf)
            {
                mainMapSelectContainer.SetActive(false);
                userMapSelectContainer.SetActive(true);
            }

        });

        //Etc Button
        exitBtn.onClick.AddListener(() => { CloseUI(); });
        spawnKeyBtn.onClick.AddListener(() => { SpawnKey(); });

    }


    public override void OnEnable()
    {
        
    }


    protected override void OpenUI()
    {
        base.OpenUI();
    }
    protected override void CloseUI()
    {
        gameObject.SetActive(false);

    }


    #region Button
    // Main And User Change Btn
    //Size, MaxWidth: 140, MinWidth: 100.
  IEnumerator FadeInChangeBtn(Button btn)
  {
        RectTransform rt = btn.gameObject.transform as RectTransform;

        onInteractable = false;
        Vector2 wh = rt.sizeDelta;

        ColorBlock colorBlock = btn.colors;

            colorBlock.normalColor = activationColor;
            colorBlock.selectedColor = activationColor;
            btn.colors = colorBlock;

            while (wh.x > 100)
            {
                wh.x -= Time.deltaTime * fadeSpeed;
                rt.sizeDelta = wh;

                yield return null;
            }
            
            wh.x = 100;
            rt.sizeDelta = wh;
        
        onInteractable = true;
    }

    IEnumerator FadeOutChangeBtn(Button btn)
    {
        RectTransform rt = btn.gameObject.transform as RectTransform;
        ColorBlock colorBlock = btn.colors;

        Vector2 wh = rt.sizeDelta;

        colorBlock.normalColor = deactivationColor;
        btn.colors = colorBlock;

        while (wh.x < 140)
        {
            wh.x += Time.deltaTime * fadeSpeed;
            rt.sizeDelta = wh;

            yield return null;
        }
        wh.x = 140;
        rt.sizeDelta = wh;
    }


    #endregion


    #region Key
    public void SpawnKey()
    {
        ExitPointObj obj = MapEditor.Instance.exitDoorObjectTransform.GetChild(0).gameObject.GetComponent<ExitPointObj>();
        if (obj.nextMapId != string.Empty)
        {
            if(key == null)
            {
                key = Managers.Stage.CmdBatchObject("Key");
            }

            ObjectData data = MapEditor.Instance.CurMap.FindObjectData(1000);
            key.transform.position = data.position;
            Vector2 launchDirection = new Vector2(-1, 1).normalized;

            key.GetComponent<Rigidbody2D>().AddForce(launchDirection * 5f, ForceMode2D.Impulse);

            CloseUI();
        }

    }
    #endregion

}
