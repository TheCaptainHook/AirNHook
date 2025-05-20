
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
                return (T)(object) new ButtonActivatableObjectStruct(id,activeRequirAmount,transform.position,transform.rotation,transform.localScale,bridgeLength,GetConnectionPoint());
            }   
            return default(T);
        }
    public override async void SetData<T>(T data)
    {
        try
        {
            if (typeof(T) == typeof(ButtonActivatableObjectStruct))
            {
                ButtonActivatableObjectStruct objData = (ButtonActivatableObjectStruct)(object)data;
                ButtonActivatedObjectStruct = objData;

                bridgeLength = objData.bridgeLength;
                connectionPoint = objData.connectionPoint;

                if (Application.isPlaying)
                {
                    //BridgeBox_Net.SetData(ButtonActivatedObjectStruct);
                    BridgeBox_Net.Server_InitSync();

                    await new Util().Delay(() => { CheckActiveRequirAmount(); });
                }
            }
        }
        catch
        {
            Debug.Log($"ERROR,{typeof(T)}");
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

