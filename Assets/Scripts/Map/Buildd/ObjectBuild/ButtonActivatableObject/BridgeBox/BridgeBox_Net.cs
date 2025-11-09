using UnityEngine;

public class BridgeBox_Net : ActivatableObject_Net_Entity
{
    [SerializeField] BoxCollider2D bridgeCollider;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] GameObject spriteObj;

    [Space(20)]
    [Header("Sync Data")]

    public float bridgeLength;
    public Vector2 connectionPoint;
    public Vector2 position;


    private BoxCollider2D Collider => GetComponent<BoxCollider2D>();


    #region Init



    protected override void SetData(ButtonActivatableObjectStruct data)
    {
        base.SetData(data);

        bridgeLength = data.bridgeLength;
        connectionPoint = data.connectionPoint;
        position = data.position;

        CreateBridge();

    }

    public override void Clean_Value()
    {
        if(_connectObject != null) Destroy(_connectObject);
    }
    #endregion



    #region  ---------------------------------------Server_Util

    public void CreateBridge()
    {
        CreateConnectionObject();
        SetBridgeCollider();
    }
    private GameObject _connectObject;
    private void CreateConnectionObject()
    {
        _connectObject = Instantiate(spriteObj);
        _connectObject.name = "Connect Object";
        _connectObject.transform.SetParent(transform);
        _connectObject.transform.position = transform.right * bridgeLength + transform.position;

        _connectObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
        _connectObject.transform.localScale = new Vector3(-1, 1, 1);

        BoxCollider2D bcol = _connectObject.AddComponent<BoxCollider2D>();
        bcol.offset = Collider.offset;
        bcol.size = Collider.size;

        _connectObject.layer = transform.gameObject.layer;

    }


    private void SetBridgeCollider()
    {

        Vector2 a = lineRenderer.gameObject.transform.position;
        Vector2 b = connectionPoint + GetOffset();

        Vector2 mid = (a + b) / 2;
        float distance = Vector2.Distance(a, b);
        Vector2 dir = (b - a).normalized;

        bridgeCollider.size = new Vector2(distance, bridgeCollider.size.y);
        bridgeCollider.transform.position = mid;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bridgeCollider.transform.rotation = Quaternion.Euler(0, 0, angle);

        bridgeCollider.gameObject.layer = LayerMask.NameToLayer("Ground/NotHookable");

        bridgeCollider.enabled = false;

        DrawLine();

    }
    private Vector2 GetOffset()
    {
        //return transform.position - lineRenderer.transform.position;
        return (Vector3)position - lineRenderer.transform.position;
    }
    #endregion



    protected override void Active()
    {
        lineRenderer.enabled = true;
        bridgeCollider.enabled = true;
        //DrawLine();
    }
    protected override void Deactive()
    {
        lineRenderer.enabled = false;
        bridgeCollider.enabled = false;
    }


     private void DrawLine()
     {
        lineRenderer.positionCount =2;
        lineRenderer.SetPosition(0, lineRenderer.transform.position);
        lineRenderer.SetPosition(1,connectionPoint+GetOffset());
    }



}
