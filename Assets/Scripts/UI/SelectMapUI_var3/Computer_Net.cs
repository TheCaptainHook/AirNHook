using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Computer_Net : NetworkBehaviour
{
    [SerializeField] GameObject screen;

    [SyncVar] public GameObject computer;
    public GameObject main;
    private UI_StageSelect_var3 Main
    {
        get
        {
            if(main == null) return null;
            return main.GetComponent<UI_StageSelect_var3>();
            
        }
    }
    public GameObject dummy;
    private UI_StageSelect_var3_Dummy Dummy
    {
        get
        {
            if(dummy == null) return null;
            return dummy.GetComponent<UI_StageSelect_var3_Dummy>();
        }
    }

    [SyncVar] public bool onPower;
    // [SyncVar] public bool previousOnPower;
    [SyncVar] public bool isOpen;



    private void Update()
    {
        if(isServer && onPower)
        {
            GetKeyEvent();
        }

    }



    [Server]
    public void Server_SetOnPower()
    {
     if(isOpen) return;
     isOpen = true;
     onPower = true;

     Rpc_ShowUi();
        //sync setting, main, dummy
        // -> Open ui,
    }
    [Server]
    public void Server_SetComputer(GameObject computer)
    {
        this.computer = computer;
    }
    [Server]
    public void Server_SetIsOpen(bool val)
    {
        isOpen = val;
    }
   
    #region  Show Ui
    [ClientRpc]
    private void Rpc_ShowUi()
    {
       if(!isServer)
       {
        Debug.Log("Client Show Ui");
       }else{
        ShowMain(true);
        ShowDummy();
       }
        
    }
    
    private void ShowDummy()
    {
            var dummy =  Managers.UI.ShowUI<UI_StageSelect_var3_Dummy>();
            UI_StageSelect_var3_Dummy _dummy = dummy.GetComponent<UI_StageSelect_var3_Dummy>();
            Canvas canvas = dummy.GetComponent<Canvas>();
            canvas.worldCamera = CameraHolder.Instance.StageSelectCamera();

            this.dummy = dummy.gameObject;

            _dummy.StartUi(computer.GetComponent<NetworkIdentity>().netId);
    }
    private void ShowMain(bool server)
    {
        if(server)
        {
            var main =  Managers.UI.ShowUI<UI_StageSelect_var3>();
            UI_StageSelect_var3 _main = main.GetComponent<UI_StageSelect_var3>();
            this.main = main.gameObject;
            _main.StartUi(computer.GetComponent<NetworkIdentity>().netId);
            // main.GetComponent<UI_StageSelect_var3>().HideUIOutsideCamera();
        }else
        {

        }
    }
    #endregion
   
    [ClientRpc]
    private void Rpc_SetKey(int num)
    {
        switch(num)
        {
            case 1:
                Main.SetInputKey(1);
                Dummy.SetInputKey(1);
            break;
            case 2:
                Main.SetInputKey(2);
                Dummy.SetInputKey(2);
            break;
            case 3:
                Main.SetInputKey(3);
                Dummy.SetInputKey(3);
            break;
            case 4:
                Main.SetInputKey(4);
                Dummy.SetInputKey(4);
            break;
            case 5:
                Main.SetInputKey(5);
                Dummy.SetInputKey(5);
            break;

        }
    }


    #region  Utile

    private void GetKeyEvent(){
         if (Input.GetKeyDown(KeyCode.DownArrow))
            {
              Rpc_SetKey(1);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
               Rpc_SetKey(2);
            }
            
            if (Input.GetKeyDown(KeyCode.Return))
            {
               Rpc_SetKey(3);
            }


            if (Input.GetKeyDown(KeyCode.Backspace))
            {
             Rpc_SetKey(4);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
              Rpc_SetKey(5); 
            }
    }
    #endregion
}
