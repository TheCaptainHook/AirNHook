
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class TEST_Projectile : MonoBehaviour
{

    [Header("COMPONENETS")]
    public Rigidbody2D rigi;

    [Header("INFO")]
   public Vector2 dir;
   public float speed;
    

private void Update(){
    CastRay(transform.position,transform.right);
}
private void CastRay(Vector2 rayPot,Vector2 dir){
    for(int i = 0; i<3;i++){
    var ray = new Ray(rayPot,dir);
    RaycastHit2D rr = Physics2D.Raycast(ray.origin, ray.direction, 10);
    if(rr.collider != null){

        Vector2 colDir = (Vector2)rr.collider.transform.right;

        Debug.DrawLine(rayPot,rr.point,Color.green);
        rayPot = rr.point;
        dir =Vector2.Reflect(rayPot,colDir);

    }else{
        Debug.DrawLine(rayPot,dir*10f,Color.red);
        break;
    }
    }
   
}
 

}
