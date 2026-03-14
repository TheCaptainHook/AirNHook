using Mirror;
using UnityEngine;



public class ExitPointObj : BuildObj
{
    [CustomHeader("Exit Door")]
    [Header("State")]
    [ReadOnly]
    // [SerializeField] bool stageClear;
    public string nextMapId;

    // [Header("Info")]
    // [ReadOnly]
    [ReadOnly]
    public int condition_KeyAmount;
    // private int current_KeyAmount;

    [Header("Componenets")]
    // DoorOpeningAnim doorOpeningAnim;
    // Collider2D _col;
    // UI_Dialogue dialogue; //TODO 0805

    [SerializeField] KeyBubble keyBubble;//TOdo 0802 Need Network
    [SerializeField] AbsencePanel absencePanel;//TOdo 0802 Need Network

    ExitPoint_Net exitPoint_Net;
    ExitPoint_Net ExitPoint_Net
    {
        get
        {
            if (exitPoint_Net == null) exitPoint_Net = GetComponent<ExitPoint_Net>();
            return exitPoint_Net;
        }
    }

    private void Awake()
    {
        // doorOpeningAnim = GetComponent<DoorOpeningAnim>();
        // _col = GetComponent<Collider2D>();
       
    }

    private void Start()
    {
        // dialogue = Managers.UI.GetUI<UI_Dialogue>().gameObject.GetComponent<UI_Dialogue>();//TODO 0805
    }


    public ExitObjStruct GetExitObjectStruct()
    {
      
        return new ExitObjStruct(id,transform.position, condition_KeyAmount, nextMapId);
    }
    //--------------------------------------------------------------------------------------------NetWork
    public void AddKeyAmount()
    {

        ExitPoint_Net.Server_AddKeyAmount();
    }
    //--------------------------------------------------------------------------------------------NetWork
    public ExitObjStruct data;
    public override void SetData<T>(T data)
    {
        if(typeof(T)==typeof(ExitObjStruct)){
            ExitObjStruct eData = (ExitObjStruct)(object)data;
            this.data = eData;

            if(Application.isPlaying)
            {
                ExitPoint_Net.Server_InitSync();
            }else{
                nextMapId = eData.nextMapId;
                transform.position = eData.position;
            }

        }
    }





    //TOdo 0729
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision == null) return;

        if(collision.TryGetComponent(out Key component)){
            if(NetworkServer.active)
            ExitPoint_Net.Server_GetKey(component.GetComponent<NetworkIdentity>().netId);
            Managers.Sound.PlaySound(GlobalText.KEY_SOUND, 0.45f);
        }
        
        if(collision.gameObject.TryGetComponent(out PlayerSM _) && MapEditor.Instance.stageClear && NetworkServer.active)
        {   
           ExitPoint_Net.Server_InPlayer(collision.GetComponent<NetworkIdentity>().netId);
        }
    }
  
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision == null || ExitPoint_Net.onReadyToMoveNextMap) return;
        
        if(collision.gameObject.TryGetComponent(out PlayerSM _) && MapEditor.Instance.stageClear && NetworkServer.active)
        {
            ExitPoint_Net.Server_OutPlayer(collision.GetComponent<NetworkIdentity>().netId);
        }
    }

    //Use Stage Select UI
    public void Net_SetNextMapId(string nextMapId)
    {
        ExitPoint_Net.Server_SetNextMapId(nextMapId);
    }



    public override void TurnOff()
    {
        base.TurnOff();
        turnOff = true;
    }
    public override void TurnOn()
    {
        base.TurnOn();
        turnOff = false;
    }


    public override void Reset()
    {
        keyBubble.gameObject.SetActive(false);
        absencePanel.gameObject.SetActive(false);
        // stageClear = false;
        //Door Lock
    }

    #region Util

    // private IEnumerator ExecuteAfterDelay(float delay, System.Action action)
    // {
    //     yield return new WaitForSeconds(delay);
    //     action();
    // }

    // private IEnumerator ExecuteAfterDelay(IEnumerator coroutine, System.Action action)
    // {
    //     yield return coroutine;
    //     yield return new WaitForSeconds(1f);
    //     action();
        
    // }


    #endregion

    //   #region  AbsenecePanel
    // public void Enter(GameObject obj)
    // {
    //     absencePanel.Enter(obj);
    // }
    // public void Exit(GameObject obj)
    // {
    //     absencePanel.Exit(obj);
    // }
    // public void OnAbsence()
    // {
    //     absencePanel.OnAbsencePanel();
    // }
    // #endregion
}
