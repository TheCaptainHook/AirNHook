

using UnityEngine;

public enum DrawType{
    Cube,
    Sphere

}

public class DetectionFieldDestroyer : MonoBehaviour
{
    [Tooltip("Select Detection Field Type")]
    public DrawType drawType;

    public LayerMask layerMask;
    //public LayerMask playerLayerMask;

    //Editor
    public Vector2 offset;
    public Vector2 _CubeSize;
    public float _SphereRadius;

    private Vector3 previousPosition;
    private float previousDistanceToFloor;
    private Color color = new Color(222/255f,111/255f/31/255f,0.5f);

    int groundLayerIndex;
    int playerLayerIndex;


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = color;

        Matrix4x4 matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.matrix = matrix;

        switch (drawType)
        {
            case DrawType.Cube:
                //Gizmos.DrawCube(transform.position + (Vector3)(transform.rotation * offset), _CubeSize);
                Gizmos.DrawCube(offset, _CubeSize);
                break;
            case DrawType.Sphere:
                //Gizmos.DrawSphere(transform.position + (Vector3)(transform.rotation * offset), _SphereRadius);
                Gizmos.DrawSphere(offset, _SphereRadius);
                break;
        }

        Gizmos.matrix = Matrix4x4.identity;

    }
#endif

    private void Awake()
    {
        groundLayerIndex = 1 << 6;
        playerLayerIndex = 1 << 10;
    }

    private void Start()
    {
        previousPosition = transform.position;
        previousDistanceToFloor = float.MaxValue;
    }

    private void Update()
    {
        if (previousPosition != transform.position)
        {
            DetectionField();
        }

        previousPosition = transform.position;
    }

    private void DetectionField()
    {
        switch (drawType)
        {
            case DrawType.Cube:
                Collider2D[] colCube = Physics2D.OverlapBoxAll(
                    // transform.position+(Vector3)offset,
                    transform.position + (Vector3)(transform.rotation * offset),
                    _CubeSize,
                    transform.eulerAngles.z,
                    layerMask);
                DetectionAndDestroy(colCube);
                break;
            case DrawType.Sphere:
                Collider2D[] colSphere = Physics2D.OverlapCircleAll(
                    // transform.position+(Vector3)offset,
                    transform.position + (Vector3)(transform.rotation * offset),
                    _SphereRadius,
                    layerMask);
                DetectionAndDestroy(colSphere);
                break;
        }
    }


    private void DetectionAndDestroy(Collider2D[] cols)
    {
        if (cols.Length <2) return;

        Collider2D groundCol = null;
        Vector2 groundPot = Vector2.zero;
        Collider2D playerCol = null;
        Vector2 playerPot = Vector2.zero;

        foreach(var item in cols)
        {
            if((groundLayerIndex & (1 << item.gameObject.layer)) != 0)
            {
                groundCol = item;
                groundPot = item.ClosestPoint(transform.position);
            }

            if((playerLayerIndex & (1 << item.gameObject.layer)) != 0)
            {
                playerCol = item;
                playerPot = item.ClosestPoint(transform.position);
            }
        }
        if(playerCol != null && groundCol != null)
        {
           
            var player = Vector3.Distance(playerPot, transform.position);
            var ground = Vector3.Distance(groundPot, transform.position);
            Debug.Log($"Player : {playerCol.transform.position},    {player}\n, ground : {groundPot},   {ground}");
            if (ground>= player)
            {
                if (playerCol.TryGetComponent(out IDamageable component))
                {
                    component.TakeDamage();
                }

            }
          
        }
 

        //if (cols.Length == 0) return;
        //bool detectFloor = false;
        //GameObject playerObj = null;
        //foreach (Collider2D col in cols)
        //{
        //    if ((groundLayerMask.value & (1 << col.gameObject.layer)) != 0)
        //    {
        //        float _Distance = Vector3.Distance(transform.position, col.ClosestPoint(transform.position));
        //        if (previousDistanceToFloor > _Distance)
        //        {
        //            previousDistanceToFloor = _Distance;
        //            detectFloor = true;
        //        }
        //        else
        //        {
        //            previousDistanceToFloor = _Distance;
        //        }
        //    }

        //    if ((playerLayerMask.value & (1 << col.gameObject.layer)) != 0)
        //    {
        //        playerObj = col.gameObject;
        //    }

        //    if (detectFloor && (playerObj != null))
        //    {
        //        playerObj.GetComponent<IDamageable>().TakeDamage();
        //        return;
        //    }
        //}

    }
}
