
using UnityEngine;


    public class BridgeBox : ActivatableObjectEntity
    {
        [CustomHeader("Bridge Box")]
        public float bridgeLength;
        private Vector2 connectionPoint;
        [ReadOnly]
        [SerializeField] LineRenderer lineRenderer;
        [ReadOnly]
        [SerializeField] GameObject spriteObj;
        #region  Components
        // Rigidbody2D rb;
        // BoxCollider2D boxCol;
        BoxCollider2D bridgeCol;
        #endregion


        #region Network
        private BridgeBox_Net BridgeBox_Net => GetComponent<BridgeBox_Net>();
        #endregion

        #region  Get,Set
        public override T GetData<T>()
        {
            if(typeof(T) == typeof(ButtonActivatableObjectStruct)){
                return (T)(object) new ButtonActivatableObjectStruct(id,activeRequirAmount,transform.position,transform.rotation,transform.localScale,bridgeLength,GetConnectionPoint(),indicator);
            }   
            return default(T);
        }
    public override async void SetData<T>(T data)
    {
        base.SetData(data);
        bridgeLength = ButtonActivatedObjectStruct.bridgeLength;
        connectionPoint = ButtonActivatedObjectStruct.connectionPoint;

        if (Application.isPlaying)
        {
            BridgeBox_Net.Server_InitSync();
            await util.Delay(() => { CheckActiveRequirAmount(); });
        }
          
    }
    #endregion




   
    #region  Main
    protected override void Activation()
    {
        BridgeBox_Net.Server_ChangeOnActive(true);
    }
    protected override void Deactivated()
    {
        BridgeBox_Net.Server_ChangeOnActive(false);

    }


    #endregion


    private Vector2 GetConnectionPoint(){
            Vector2 dir = transform.right;
            return (Vector2)transform.position + dir*bridgeLength;
            
        }
    }

