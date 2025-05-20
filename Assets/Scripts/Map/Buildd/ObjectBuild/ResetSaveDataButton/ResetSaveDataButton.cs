using System.Collections;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ResetSaveDataButton : BuildObj, IInteractable
{
    private bool onReset;
    private float maxResetCount = 10;
    private float curResetCount = 0;

    [SerializeField] Slider slider;
    [SerializeField] Collider2D col;
    void Update()
    {
        if (localPlayer)
        {
            if (Input.GetKeyDown(KeyCode.E) && !onReset)
            {
                HideE();
                onReset = true;
                ResetSaveData();

                text.enabled = true;
                typingEffectCoroutine = StartCoroutine(TextEffectCoroutine());
                
            }
        }

        if (onReset)
        {
            curResetCount += Time.deltaTime;
            slider.value = curResetCount / maxResetCount;

            if (curResetCount >= maxResetCount)
            {
                ResetBtn();
            }
        }

    }
    private void ResetBtn()
    {
        curResetCount = 0;
        slider.value = 0;
        // text.ForceMeshUpdate();
        if (typingEffectCoroutine != null) StopCoroutine(typingEffectCoroutine);
        text.enabled = false;
        onReset = false;

        col.enabled = false;
        col.enabled = true;
    }

    public PlayerSM localPlayer;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (localPlayer) return;
        if (onReset) return;

        if (collision.TryGetComponent(out PlayerSM player))
        {
            if (player.TryGetComponent(out NetworkIdentity identity))
            {
                if (identity.isLocalPlayer)
                {
                    ShowE();
                    localPlayer = player;
                }
            }
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision)
        {
            if (localPlayer)
            {
                if (localPlayer.gameObject == collision.gameObject)
                {
                    //ShutDown
                    localPlayer = null;
                    HideE();
                    //ShutDown
                }
            }

        }
    }



    #region  Main
    private void ResetSaveData()
    {
        Managers.Data.saveData.DeleteSaveFile();
        Managers.Data.saveData.SetUp();
    }
    #endregion


    //#region  UI
    //private void ShowE()
    //{
    //    Managers.UI.ShowUI<UI_ShowEButton>();
    //}
    //private void HideE()
    //{
    //    Managers.UI.HideUI<UI_ShowEButton>();
    //}
    //#endregion


    #region  Interacable
    private UI_Base _E_Btn;
    public ObjectTypeEnum _objectType = ObjectTypeEnum.Interaction;
    [SerializeField] float _BtnOffset;

    public void Interaction(Transform accessor = null)
    {


    }

    public bool CanInteract()
    {
        return true;
    }

    public void Interacting(bool value)
    {
        return;
    }

    public ObjectTypeEnum GetObjectType()
    {
        return _objectType;
    }

    public void ShowEButton()
    {
        return;
    }
    public void HideEButton()
    {
        return;
    }

    public void ShowE()
    {
        _E_Btn = Managers.UI.ShowUI<UI_ShowEButton>();
        _E_Btn.transform.position = transform.position + (transform.up * _BtnOffset);
    }
    public void HideE()
    {
        _E_Btn = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }



    #endregion

    void Awake()
    {
        Init();
    }

    #region Typing Effect
    Coroutine typingEffectCoroutine;
    private void Init()
    {
        text.ForceMeshUpdate();

        charInfos = new CharInfoField[text.text.Length];
        for (int i = 0; i < text.text.Length; i++)
        {
            charInfos[i] = new CharInfoField(text.textInfo, text.textInfo.characterInfo[i]);
        }
        answer = text.text;

        text.enabled = false;
    }
    
    int randomNum;
    IEnumerator TextEffectCoroutine()
    {
          // randomNum = Random.Range(3, 7);
            yield return new WaitForSeconds(2);
            StartCoroutine(CharEffectCo());
    }

    [SerializeField] TextMeshProUGUI text;
    string answer;
    CharInfoField[] charInfos;
    float changeCharDuration = 0.2f;
    Color correctColor = Color.red;
    IEnumerator CharEffectCo()
    {
        text.ForceMeshUpdate();
        float percent = 0;

        char[] cached = text.text.ToCharArray();
        float changeCharPercent = 0;

        int[] c = new int[text.text.Length];
        for (int i = 0; i < c.Length; i++)
        {
            int ran = Random.Range(0, text.text.Length);
            c[i] = ran;
        }

        float baseGlitchIntensity = 30f; // 기본 강도
        float glitchIntensity = baseGlitchIntensity * text.fontSize / 100f;

        float[] correctCharTimers = new float[c.Length]; 

        while (percent < 1f)
        {
            percent += Time.deltaTime;
            changeCharPercent += Time.deltaTime;

            for (int i = 0; i < c.Length; i++)
            {
                CharInfoField charInfoField = charInfos[c[i]];
                
                if (changeCharPercent > changeCharDuration && correctCharTimers[c[i]] <= 0f)
                {
                    if(Random.Range(0,100) <= 20)
                    {
                        cached[charInfoField.charIndex] = answer[c[i]];
                        correctCharTimers[c[i]] = changeCharDuration;
                    }else
                    {
                        char randomChar = (char)Random.Range(33, 126);
                        cached[charInfoField.charIndex] = randomChar; 
                        correctCharTimers[c[i]] = 0;
                    }
                    text.SetText(cached);
                    text.ForceMeshUpdate(); 
                }
                Color32[] colors = charInfoField.tmp.meshInfo[charInfoField.materialIndex].colors32;//color

                for (int j = 0; j < 4; j++) 
                {
                    Vector3 offset = new Vector3(
                        Random.Range(-glitchIntensity, glitchIntensity),
                        Random.Range(-glitchIntensity, glitchIntensity),
                        0);
                        charInfoField.vertices[charInfoField.vertexIndex + j] = charInfoField.originalVertices[charInfoField.vertexIndex + j] + offset;
                        if(correctCharTimers[c[i]] >0f){
                             colors[charInfoField.vertexIndex + j] = correctColor;
                        } 
                }

                var meshInfo = text.textInfo.meshInfo[charInfoField.materialIndex];
                meshInfo.mesh.vertices = meshInfo.vertices;
                meshInfo.mesh.colors32 = meshInfo.colors32;
                text.UpdateGeometry(meshInfo.mesh, charInfoField.materialIndex);

            }
              for (int i = 0; i < correctCharTimers.Length; i++)
                {
                    if (correctCharTimers[i] > 0f)
                    {
                        correctCharTimers[i] -= Time.deltaTime;
                    }
                }

            if (changeCharPercent > 1f){
                changeCharPercent = 0;
                // correctChars = new bool[c.Length];
            } 

            yield return new WaitForSeconds(0.05f);
        }
        //Recover Text
        text.SetText(answer);
        text.ForceMeshUpdate();

        typingEffectCoroutine = null;
    }

    #endregion
}




