
using UnityEngine;
using Mirror;

public class StageSelectorComputer : BuildObj, IInteractable
{
    #region StringCache
    private static readonly int Talk = Animator.StringToHash("Talk");
    private static readonly int Left = Animator.StringToHash("Left");
    private static readonly int Right = Animator.StringToHash("Right");
    private static readonly int Surprise = Animator.StringToHash("Surprise");
    private static readonly int Reset = Animator.StringToHash("Reset");
    private static readonly int Line = Animator.StringToHash("Line");
    #endregion

    private Animator _animator;

    // [SerializeField] private bool _isTalking = false;

    [SerializeField] private GameObject _key;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public override void SetData<T>(T data)
    {
        // base.SetData(data);
        if (typeof(T) == typeof(ObjectData))
        {
            ObjectData = (ObjectData)(object)data;
            if (Application.isPlaying)
                Net.Server_InitSync();
            else SetData(ObjectData);

        }

    }


    public void Talking()
    {
        _animator.SetTrigger(Talk);
    }
    public void LineIdle()
    {
        _animator.SetTrigger(Line);
    }
    public void Right_()
    {
        _animator.SetTrigger(Right);
    }
    public void Left_()
    {
        _animator.SetTrigger(Left);
    }

    public void Surprise_()
    {
        _animator.SetTrigger(Surprise);
    }



    public async void SpawnKey()
    {
        ExitPointObj obj = MapEditor.Instance.exitDoorObjectTransform.GetChild(0).gameObject.GetComponent<ExitPointObj>();
        if (obj.nextMapId != string.Empty)
        {
            if (_key == null)
            {
                _key = Managers.Stage.CmdBatchObject("Key");
            }

            ObjectData data = MapEditor.Instance.CurMap.FindObjectData(1000);
            _key.transform.position = data.position;
            Vector2 launchDirection = new Vector2(-1, 1).normalized;
            _animator.SetTrigger(Left);
            _key.GetComponent<Rigidbody2D>().AddForce(launchDirection * 5f, ForceMode2D.Impulse);

        }

        Util util = new Util();
        await util.Delay(() => { Talking(); });

    }


    #region  Interactable
    public ObjectTypeEnum objectType = ObjectTypeEnum.Interaction;
    public Vector2 btn_offset;

    [ReadOnly]
    public bool onPower;

    public void Interaction(Transform accessor = null)
    {
        if (NetworkServer.active && !Net.isOpen)
        {
            Net.Server_SetOnPower();
        }

    }


    //------------------------------------------------Network 250217
    public Computer_Net Net { get { return GetComponent<Computer_Net>(); } }

    //------------------------------------------------Network

    public bool CanInteract()
    {
        return true;
    }

    public bool Interacting(bool value, GameObject player)
    {
        return true;
    }

    public ObjectTypeEnum GetObjectType()
    {
        return objectType;
    }

    public void ShowEButton()
    {
        var eButtonUI = Managers.UI.ShowUI<UI_ShowEButton>();
        eButtonUI.transform.position = transform.position + (Vector3)btn_offset;
    }

    public void HideEButton()
    {
        Managers.UI.HideUI<UI_ShowEButton>();
    }
    #endregion
   

}
