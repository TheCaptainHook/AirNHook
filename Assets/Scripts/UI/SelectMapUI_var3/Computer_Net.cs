using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Computer_Net : NetworkBehaviour
{
    [SerializeField] GameObject screen;

    // [SyncVar] public GameObject computer;
    public GameObject main;
    private UI_StageSelect_var3 Main
    {
        get
        {
            if (main == null) return null;
            return main.GetComponent<UI_StageSelect_var3>();

        }
    }
    public GameObject dummy;
    private UI_StageSelect_var3_Dummy Dummy
    {
        get
        {
            if (dummy == null) return null;
            return dummy.GetComponent<UI_StageSelect_var3_Dummy>();
        }
    }

    [SyncVar] public bool onPower; //Openning Animation cancel, Only Server
    [SyncVar] public bool isOpen;

    //UI Control Condition
    [SyncVar] public int readyAllClient;
    [SyncVar] public bool onReady;

    public int ConnectionClientCount
    {
        get
        {
            if (NetworkServer.active) return NetworkServer.connections.Count;
            return 0;
        }
    }

    #region Stage Select Manu Sync

    [SyncVar] public int index;
    [SyncVar] public Host_MapData[] host_MapDatas;

    [Server]
    public void Server_SetIndex(int index)
    {
        this.index = index;
    }
    [Server]
    public void Server_SetMapDatas(Host_MapData[] host_MapDatas)
    {
        this.host_MapDatas = host_MapDatas;
    }
    #endregion
    

//------------------------------------------------------- 0414

    private void LateUpdate()
    {
        if(isServer && onPower && isOpen && onReady)
        {
            if(!Main.onPrograss) GetKeyEvent();
        }
    }

//------------------------------------------------------- 0414
    public bool onSync;
    #region Server_Init
    private StageSelectorComputer Computer => GetComponent<StageSelectorComputer>();
    private ObjectData data;

   [Server]
   public void Server_InitSync()
   {
        data = Computer.ObjectData;
        Rpc_InitSync(data);
   }
   [ClientRpc]
   private void Rpc_InitSync(ObjectData data)
   {
    if(onSync) return;

    transform.position = data.position;
    onSync = true;
   }
   [Command(requiresAuthority =false)]
   private void Cmd_InitSync()
   {
    Server_InitSync();
   }
   
   public override void OnStartClient()
   {
        base.OnStartClient();
        if(!onSync)Cmd_InitSync();
   }

    #endregion

    #region Server_Reset
    [Server]
    public void Server_ReadyAllClientReset()
    {
        readyAllClient = 0;
        onReady = false;
    }
    #endregion

    public PlayerInputAction.PlayerActions playerActions => Managers.Game.playerInput.playerActions;
    public PlayerInputAction.UIActions uiActions => Managers.Game.playerInput.uiActions;

    [Server]
    public void Server_SetOnPower()
    {
        if (isOpen) return;
        isOpen = true;
        //------------------------------------player Move control
        FreezePlayerState(true);
        //------------------------------------player Move control

        Rpc_ShowUi();

    }
    private void FreezePlayerState(bool onOff)
    {
        var player = Managers.Game.Player.TryGetComponent(out PlayerSM sm) ? sm : null;
        if (player == null) return;

        sm.FreezePlayerState(onOff);
    }
    

    [Server]
    public void Server_SetIsOpen(bool val)
    {
        if (!val)
        {
            playerActions.Enable();
            uiActions.Enable();    
        } 


        isOpen = val;
    }
   
    #region  Show Ui
    [ClientRpc]
    private void Rpc_ShowUi()
    {
        if (!isServer)
        {
            ShowDummy();
        }
        else
        {
            //playerActions.Disable();
            uiActions.Disable();

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

            _dummy.StartUi(GetComponent<NetworkIdentity>().netId);
    }
    private void ShowMain(bool server)
    {
        if(server)
        {
            var main =  Managers.UI.ShowUI<UI_StageSelect_var3>();
            UI_StageSelect_var3 _main = main.GetComponent<UI_StageSelect_var3>();
            this.main = main.gameObject;
            _main.StartUi(GetComponent<NetworkIdentity>().netId);
        }
    }

    [Server]
    private void Server_SetReadyClient()
    {
        readyAllClient++;
        if(readyAllClient >= ConnectionClientCount)
        {
            onReady = true;
        }
    }

    [Command(requiresAuthority = false)]
    public void Cmd_ReadyClient()
    {
        Server_SetReadyClient();
    }
    #endregion

    #region Input

    /// <summary>
    /// 1 : Up, 
    /// 2 : Down, 
    /// 3 : Enter, 
    /// 4 : Backspace, 
    /// 5 : Q 
    /// </summary>
    /// <param name="num"></param>
    [ClientRpc]
    private void Rpc_SetKey(int num)
    {
        switch (num)
        {
            case 1:
                if (!isServer)
                {
                    //if(!Dummy)
                    Dummy.SetInputKey(1);
                }
                else
                {
                    Main.SetInputKey(1);
                    Dummy.SetInputKey(1);
                }

                break;
            case 2:
                if (!isServer)
                {
                    //if (!Dummy)
                        Dummy.SetInputKey(2);
                }
                else
                {
                    Main.SetInputKey(2);
                    Dummy.SetInputKey(2);
                }
                break;
            case 3:
                if (!isServer)
                {
                    //if (!Dummy)
                    Dummy.SetInputKey(3);
                }
                else
                {
                    Main.SetInputKey(3);
                    Dummy.SetInputKey(3);
                }
                break;
            case 4:
                if (!isServer)
                {
                    //if (!Dummy)
                        Dummy.SetInputKey(4);
                }
                else
                {
                    Main.SetInputKey(4);
                    Dummy.SetInputKey(4);
                }
                break;
            case 5:
                if (!isServer)
                {
                    //if (!Dummy)
                        Dummy.SetInputKey(5);
                }
                else
                {
                    Main.SetInputKey(5);
                    Dummy.SetInputKey(5);
                }
                break;

        }
    }

    [Server]
    private void GetKeyEvent()
    {
        server_CurSelectTextLineIndex = Main.curSelectTextLineIndex;
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

    [SyncVar]
    public int server_CurSelectTextLineIndex;

    [Server]
    public void Server_GetMousePointer(int index)
    {
        server_CurSelectTextLineIndex = index;
        Rpc_GetMousePointer(server_CurSelectTextLineIndex);
    }
    [ClientRpc]
    public void Rpc_GetMousePointer(int index)
    {
        if(!isServer)
        {
            Dummy.curSelectTextLineIndex = index;
            Dummy.SelectTextLine();
        }
        else
        {
            Main.curSelectTextLineIndex = index;
            Dummy.curSelectTextLineIndex = index;

            Main.SelectTextLine();
            Dummy.SelectTextLine();
        }

    }
    [Server]
    public void Server_MouseClick()
    {
        Rpc_SetKey(3);
    }   
   
    #endregion

}
