
using Steamworks;
using Unity.VisualScripting;
using UnityEngine;


public class Drone_Laser : DroneEntity
{
   //Animation Parameter transform rotation

    [SerializeField]Animator _Animator;
    [SerializeField] Animator _LazerAnimator;

    //Limit Rot : 0 ~ 150

#region  Animation
readonly int _Directon = Animator.StringToHash("Direction");
#endregion

   #region Laser
   [Header("Laser")]
   public GameObject target;
   [SerializeField] Transform _LazerBody;
   [SerializeField] Transform attackPot;
   [SerializeField] LineRenderer lineRenderer;
   private Ray ray;
   [SerializeField] LayerMask layerMask;


   private bool onLazer;
   public bool OnLazer{get{return onLazer;}}
    // 1008
    private Vector2 previousTargetPosition;

   #endregion

//TEST
    // protected override void Start()
    // {
    //    //TEST CODE
    //   paths = ConvertPaths(paths);
    //     //TEST CODE

    //   base.Start();
       
    // }

   //  public override void TakeDamage()
   //  {
   //      if(!IsBroken){
   //       IsBroken = true;
   //       CallBrokenAction();
   //      }
   //  }

    // private void Update(){
    //   if(Input.GetKeyDown(KeyCode.A)){
    //      IsBroken = false;
    //    CallPrograssAction();
    //   }

    //   if(Input.GetKeyDown(KeyCode.S)){
    //      CallBrokenAction();
    //   }
    // }




public void TurnOnLazer(){
  if(!onLazer){
    onLazer = true;
  }
}
public void TurnOffLazer(){
  if(onLazer){
    lineRenderer.positionCount = 0;
    onLazer = false;
  }
}
   private void FixedUpdate()
   {
    if(onLazer){
      UpdateLaser();
    }
   }




#region  Laser

   public void UpdateLaser()
        {
         if(target ==null) return;
            Vector2 start = attackPot.position;
            Vector2 dir = (target.transform.position - attackPot.transform.position).normalized;

            int hitCount = 0;  
            
            for (int i = 0; i < 10; i++)
            {
                ray = new Ray(start, dir);  
                RaycastHit2D rh = Physics2D.Raycast(ray.origin, ray.direction, 10,layerMask);
                // Debug.DrawRay(start,dir*10,Color.blue);
                if (rh.collider != null)
                {
                    Vector2 colDir = rh.normal;
                    DrawLaser(i,start, rh.point);
                    hitCount++;
            
                    //Check collider
                     if(rh.collider.TryGetComponent(out Player component)  && Application.isPlaying){
                        //  SetHitParticleRotate(start,rh.point); // todo 0914
                        //  component.TakeDamage();
                         break;
                     }else if(rh.collider.gameObject.name == "Mirror"){
                         start = rh.point;
                         dir = Vector2.Reflect(ray.direction, colDir);
                     }else if(rh.collider.TryGetComponent(out LaserTriggerButton component2)){
                            if(Application.isPlaying){
                                Debug.Log("Is playing,Detected Laser Object");
                              //   SetHitParticleRotate(start,rh.point); // todo 0914
                                component2.SendMessage("Charging",SendMessageOptions.DontRequireReceiver);
                            }else{
                                Debug.Log("Detected Laser Trigger Object");
                            }
                         break;
                     }else{
                        // SetHitParticleRotate(start,rh.point);
                        if(rh.collider.TryGetComponent(out IDamageable damageable)){
                            damageable.TakeDamage();
                        }
                        break;
                     }
                }
                else
                {
                    if(hitCount == 0){
                        lineRenderer.positionCount = 0;
                    }
                    break;
                }
            }
        }
        
  public void RotLazerAnimation(float deg){
    int newDeg = Mathf.FloorToInt(deg);
    _LazerAnimator.SetFloat(_Directon,newDeg);
    NormalizationLazerAndAttackPot(newDeg);
  }

  private void NormalizationLazerAndAttackPot(int locDeg){
    if(target == null) return;
    if(CompareInverseTransformPoint(attackPot,previousTargetPosition,target.transform)){
        return;
    }
    float deg;
    _LazerBody.eulerAngles = Vector3.zero;
    previousTargetPosition = attackPot.transform.InverseTransformPoint((Vector2)target.transform.position);

    Vector2 dir = (previousTargetPosition - (Vector2)attackPot.position).normalized;
    if(locDeg<-100){
        deg = GetAngleFromVector(dir)*-1;
    }else{
        deg = GetAngleFromVector(dir);
    }
    
    Vector3 rot = _LazerBody.eulerAngles;
    rot.z = deg;
    _LazerBody.eulerAngles = rot;

  }
  private float GetAngleFromVector(Vector3 dir){
        float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
        return angle;
    }
  
   private void DrawLaser(int num,Vector2 start, Vector2 endPos)
   {
      lineRenderer.positionCount = num + 2;
      lineRenderer.SetPosition(num, start);
      lineRenderer.SetPosition(num+1, endPos);
   }
   
    private bool CompareInverseTransformPoint(Transform mainLocalTr,Vector2 previousPot,Transform comparisonTransform){
        Vector3 convertVec = mainLocalTr.InverseTransformPoint(comparisonTransform.position);
        return previousPot == (Vector2)convertVec;
    }
#endregion

}
