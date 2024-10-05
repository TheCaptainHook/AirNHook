
using UnityEngine;

public class Drone_Laser : DroneEntity
{
   //Animation Parameter transform rotation

    [SerializeField]Animator _Animator;


    //Limit Rot : 0 ~ 150

     private void Start(){
       paths = ConvertPaths(paths);
       Prograss();
   }





}
