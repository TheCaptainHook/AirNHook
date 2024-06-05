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

    
    //event Action<string> OnSelectMap;


    private void Awake()
    {
        mainBtn.onClick.AddListener(() => { if (!onInteractable) return;
            //reset
            if (currentMainAndUserMapChangeBtn != null && mainBtn != currentMainAndUserMapChangeBtn)
            {
                StartCoroutine(FadeOutChangeBtn(currentMainAndUserMapChangeBtn));
            }

            currentMainAndUserMapChangeBtn = mainBtn;
            StartCoroutine(FadeInChangeBtn(mainBtn));

            mainMapSelectContainer.SetActive(true);
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
            }

        });

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
        base.CloseUI();
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

}
