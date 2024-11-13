using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightDetectionMoveingPlatform : MonoBehaviour
{
    
    private RaycastHit2D leftHit;
    private RaycastHit2D rightHit;
    private float rayLength;


    #region Main
    public float platformSpeed;
    public float maxRotateRate;

    private float bodyCenter;

    private Vector2 leftRayPosition;
    private Vector2 rightRayPosition;
    #endregion

    
   #region  Components
   private Rigidbody2D rb;
   private Collider2D bodyCol;
   #endregion





}
