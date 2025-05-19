using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Mirror;
using UnityEngine;


public class WDMP_Net : NetworkBehaviour
{
    [SerializeField] GameObject rail_Node_Prefabs;
    [SerializeField] LineRenderer rail_Line_Prefabs;

    // private Collider2D Collider => GetComponent<Collider2D>();

    [SyncVar] public float moveDistance;
    [SyncVar(hook = nameof(OnDataPathUpdated))]
    public Vector2 position;

    [SyncVar] public float rayLength;
    [SyncVar] public float moveSpeed;
    [SyncVar] public Vector2 dir;
    [SyncVar] public float step;


    [SyncVar] public float minDis_Clamp;
    [SyncVar] public float maxDis_Clamp;

    private float recoveryRate = 5;
    [ReadOnly]
    public float curRecoveryRate;

    Coroutine recoveryCoroutine;
    private Rigidbody2D rb;
    private bool onRecover;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        AnimationTilt();
    }
    private void Update()
    {
        if (!isServer) return;

        if (moveDistance != 0 && !Compare(transform.position, position) && !onRecover)
        {
            curRecoveryRate += Time.fixedDeltaTime;
            if (curRecoveryRate >= recoveryRate)
            {
                onRecover = true;
                //Recover
                recoveryCoroutine = StartCoroutine(Recover_Co());
            }
        }

    }
    IEnumerator Recover_Co()
    {
        var rb = GetComponent<Rigidbody2D>();

        while (!Compare(rb.position, position))
        {
            rb.position = Vector2.MoveTowards(rb.position, position, moveSpeed * Time.fixedDeltaTime);
            yield return null;
        }
        recoveryCoroutine = null;
        rb.position = position;
        onRecover = false;
    }

    [Server]
    public void Server_SetMoveDistance(float moveDistance, Vector2 position, float moveSpeed)
    {
        this.moveDistance = moveDistance;
        this.position = position;
        this.moveSpeed = moveSpeed;
    }

    [Server]
    public void Server_SetDir(Vector2 dir)
    {
        this.dir = dir;
    }
    [Server]
    public void Server_SetStep(float step)
    {
        if (step != 0)
        {
            //Stop Recover
            if (recoveryCoroutine != null)
            {
                StopCoroutine(recoveryCoroutine);
                onRecover = false;
            }
            curRecoveryRate = 0;
        }

        this.step = step;
    }
    [Server]
    public void Server_SetClamp(float min, float max)
    {
        this.minDis_Clamp = min;
        this.maxDis_Clamp = max;
    }
    [Server]
    public void Server_SetRayLength(float rayLength)
    {
        this.rayLength = rayLength;
    }

    Transform container;
    GameObject railNode_1;
    GameObject railNode_2;
    LineRenderer line;

    private void CreateRail() //rail node, rail lineRenderer
    {
        Transform parents = MapEditor.Instance.dontSaveObjectTransform;
        container = new GameObject("Rail_Container").transform;
        container.SetParent(parents);

        line = Instantiate(rail_Line_Prefabs, container);
        //Draw Line
        DrawLine(line);

        railNode_1 = Instantiate(rail_Node_Prefabs, container);
        railNode_1.transform.position = line.GetPosition(0);

        railNode_2 = Instantiate(rail_Node_Prefabs, container);
        railNode_2.transform.position = line.GetPosition(1);
    }
    private void DrawLine(LineRenderer line)
    {
        line.positionCount = 2;
        line.SetPosition(0, position);
        Vector2 target = new Vector2(position.x + moveDistance, position.y);
        line.SetPosition(1, target);
    }


    //private void CreateNode()
    //{
    //    DrawLine(line);
    //    railNode_1.transform.position = line.GetPosition(0);
    //    railNode_2.transform.position = line.GetPosition(1);
    //}

    private void OnDataPathUpdated(Vector2 old, Vector2 newVal)
    {
        if (newVal != Vector2.zero)
        {
            CreateRail();
        }
    }

    private bool Compare(Vector2 a, Vector2 b, float threshold = 0.01f)
    {
        return Vector2.Distance(a, b) < threshold;
    }



    #region Animation
    private readonly int leftDown = Animator.StringToHash("LeftDown");
    private readonly int rightDown = Animator.StringToHash("RightDown");
  
    public void AnimationTilt()
    {
        StartCoroutine(AnimaionTiltCoroutine());
    }
    Animator animator;
    Animator Animator { get { animator ??= GetComponent<Animator>(); return animator; } }
    bool leftAni;
    bool rightAni;
    WaitForSeconds wait = new WaitForSeconds(0.1f);
    private IEnumerator AnimaionTiltCoroutine()
    {
        while(true)
        {
            var z = rb.rotation;
            if (z > 0)
            {
                if (!leftAni)
                {
                    leftAni = true;
                    Animator.SetBool(leftDown, leftAni);
                }
                if (rightAni)
                {
                    rightAni = false;
                    Animator.SetBool(rightDown, rightAni);
                }
            }
            else if (z < 0)
            {
                if (leftAni)
                {
                    leftAni = false;
                    Animator.SetBool(leftDown, leftAni);
                }
                if (!rightAni)
                {
                    rightAni = true;
                    Animator.SetBool(rightDown, rightAni);
                }
            }else if(Mathf.Abs(z) < 0.1)
            {
                if(leftAni) { leftAni = false; Animator.SetBool(leftDown, leftAni); }
                if (rightAni) {  rightAni = false; Animator.SetBool(rightDown, rightAni); }
            }
                yield return wait;
        }
    }

    #endregion
}
