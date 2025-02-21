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
    [SyncVar] public float rayLength;

    [SyncVar] public Vector2 dir;
    [SyncVar] public float step;


    [SyncVar] public float minDis_Clamp;
    [SyncVar] public float maxDis_Clamp;

    [Server]
    public void Server_SetMoveDistance(float moveDistance)
    {
       this.moveDistance = moveDistance;   
    }

    [Server]
    public void Server_SetDir(Vector2 dir)
    {
        this.dir = dir;
    }
    [Server]
    public void Server_SetStep(float step)
    {
        this.step = step;
    }
    [Server]
    public void Server_SetClamp(float min,float max)
    {
        this.minDis_Clamp = min;
        this.maxDis_Clamp = max;
    }
    [Server]
    public void Server_SetRayLength(float rayLength)
    {
        this.rayLength = rayLength;
    }

    private void CreateRail() //rail node, rail lineRenderer
    { 
        Transform parents = MapEditor.Instance.dontSaveObjectTransform;
        Transform container = new GameObject("Rail_Container").transform;
        container.SetParent(parents);

        LineRenderer line = Instantiate(rail_Line_Prefabs,container);
        //Draw Line
        DrawLine(line);

        GameObject railNode_1 = Instantiate(rail_Node_Prefabs,container);
        railNode_1.transform.position = line.GetPosition(0);
        GameObject railNode_2 = Instantiate(rail_Node_Prefabs,container);
        railNode_2.transform.position = line.GetPosition(1);
    }
    private void DrawLine(LineRenderer line)
    {
        line.positionCount = 2;
        line.SetPosition(0,transform.position);
        Vector2 target = new Vector2(transform.position.x + moveDistance,transform.position.y);
        line.SetPosition(1,target);
    }


    float timeOut = 10;
    float elapsedTime = 0f;
    IEnumerator WaitforSync()
    {
        while (moveDistance == 0 && elapsedTime < timeOut)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
        }

        if (moveDistance == 0)
        {
            Debug.LogWarning("WaitforSync: 데이터 동기화가 시간 초과됨. 작업 취소됨.");
            yield break;
        }
        elapsedTime = 0f;

        CreateRail();

    }


    public override void OnStartClient()
    {
        base.OnStartClient();
        //CreateRail();
        StartCoroutine(WaitforSync());
    }

}
