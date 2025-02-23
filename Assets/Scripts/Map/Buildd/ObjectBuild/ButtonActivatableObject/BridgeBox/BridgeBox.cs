
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

            }
            }
            catch
            {
                Debug.Log($"ERROR,{typeof(T)}");
            }

                if (Application.isPlaying)
                {
                    // BridgeSetting();
                    BridgeBox_Net.Server_SetData(bridgeLength,connectionPoint,ButtonActivatedObjectStruct.position);

                    await new Util().Delay(() => { CheckActiveRequirAmount(); });
                }
            }
        #endregion


        private void Awake(){
            // rb = GetComponent<Rigidbody2D>();
            // boxCol = GetComponent<BoxCollider2D>();
        }

    // private void BridgeSetting(){

    //     // CreateConnectionObject();
    //     // SetBridgeCollider();
    // }


    #region  Main
    protected override void Activation()
    {
        BridgeBox_Net.Cmd_SetOnActive();
    }
    protected override void Deactivated()
    {
        BridgeBox_Net.Cmd_SetOnActive();

    }
    // private void ConnectBridge(){
    //     DrawLine();
    //     bridgeCol.enabled = true;
    // }
    // private void DrawLine(){
    //     lineRenderer.positionCount =2;
    //     lineRenderer.SetPosition(0,lineRenderer.transform.position);
    //     lineRenderer.SetPosition(1,connectionPoint+GetOffset());
    // }

    //  private void DisconnectBridge(){
    //     lineRenderer.positionCount = 0;
    //     bridgeCol.enabled =false;
    // }


    // private void SetBridgeCollider(){
    //     GameObject obj = new GameObject("bridge");
    //     bridgeCol = obj.AddComponent<BoxCollider2D>();

    //     Vector2 a = lineRenderer.gameObject.transform.position;
    //     Vector2 b = connectionPoint + GetOffset();

    //     Vector2 mid = (a+b)/2;
    //     float distance = Vector2.Distance(a,b);
    //     Vector2 dir = (b-a).normalized;

    //     bridgeCol.size = new Vector2(distance,bridgeCol.size.y);
    //     obj.transform.position = mid;
    //     float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
    //     obj.transform.rotation = Quaternion.Euler(0,0,angle);

    //     obj.layer = LayerMask.NameToLayer("Ground/NotHookable");
    //     obj.transform.SetParent(transform);

    //     bridgeCol.enabled = false;
    // }


    // private Vector2 GetOffset(){
    //     return transform.position - lineRenderer.transform.position;
    // }

    #endregion


    // private void CreateConnectionObject(){
    //     GameObject obj = new GameObject("Connect Object");

    //     GameObject spO = Instantiate(spriteObj);
    //     spO.transform.localScale = new Vector3(-1,1,1);
    //     spO.transform.SetParent(obj.transform);

    //     Rigidbody2D rb = obj.AddComponent<Rigidbody2D>();
    //     rb.isKinematic = true;
    //     rb.gravityScale = 0;

    //     BoxCollider2D bcol = obj.AddComponent<BoxCollider2D>();
    //     bcol.offset = this.boxCol.offset;
    //     bcol.size  = this.boxCol.size;

    //     obj.transform.rotation = transform.rotation;
    //     obj.transform.position = connectionPoint;

    //     obj.transform.SetParent(transform);
    //     obj.layer  = transform.gameObject.layer;

    // }

    private Vector2 GetConnectionPoint(){
            Vector2 dir = transform.right;
            return (Vector2)transform.position + dir*bridgeLength;
            
        }
    }

