
using Unity.VisualScripting;
using UnityEngine;

public enum DrawType{
    Cube,
    Sphere

}

public class DetectionFieldDestroyer : MonoBehaviour
{
    [Tooltip("Select Detection Field Type")]
    public DrawType drawType;

    public LayerMask groundLayerMask;
    public LayerMask playerLayerMask;

    //Editor
    public Vector2 offset;
    public Vector2 _CubeSize;
    public float _SphereRadius;

    private Vector3 previousPosition;
    private float previousDistanceToFloor;
    private Color color = new Color(222/255f,111/255f/31/255f,0.5f);

   private void OnDrawGizmosSelected(){
    Gizmos.color = color;
    switch(drawType){
        case DrawType.Cube:
        Gizmos.DrawCube(transform.position+ (Vector3)offset,_CubeSize);
        break;
        case DrawType.Sphere:
        Gizmos.DrawSphere(transform.position+ (Vector3)offset,_SphereRadius);
        break;
    }

   }
   private void Start(){
    previousPosition = transform.position;
    previousDistanceToFloor = float.MaxValue;
   }

   private void Update(){
    if(previousPosition != transform.position){
        DetectionField();
    }
    previousPosition = transform.position;
   }

    private void DetectionField(){
        switch(drawType){
            case DrawType.Cube:
            Collider2D[] colCube = Physics2D.OverlapBoxAll(transform.position+(Vector3)offset,_CubeSize,0,groundLayerMask | playerLayerMask);
            DetectionAndDestroy(colCube);
            break;
            case DrawType.Sphere:
            Collider2D[] colSphere = Physics2D.OverlapCircleAll(transform.position+(Vector3)offset,_SphereRadius,groundLayerMask | playerLayerMask);
            DetectionAndDestroy(colSphere);
            break;
        }
    }


    private void DetectionAndDestroy(Collider2D[] cols){
        if(cols.Length == 0) return;
        bool detectFloor = false;
        GameObject playerObj = null;
        foreach(Collider2D col in cols){

            if((groundLayerMask.value & (1 << col.gameObject.layer)) != 0){
                float _Distance = Vector3.Distance(transform.position,col.ClosestPoint(transform.position));
                if(previousDistanceToFloor > _Distance){
                    Debug.Log($"col : {col.gameObject.name} , {_Distance}");
                    previousDistanceToFloor = _Distance;
                    detectFloor = true;
                }else{
                    previousDistanceToFloor = _Distance;
                }
            }

            if((playerLayerMask.value & (1 << col.gameObject.layer)) != 0){
                Debug.Log(col.gameObject.name);
                playerObj = col.gameObject;
            }

            if(detectFloor && (playerObj != null)){
                playerObj.GetComponent<IDamageable>().TakeDamage();
                return;
            }
        }
   
    }
}
