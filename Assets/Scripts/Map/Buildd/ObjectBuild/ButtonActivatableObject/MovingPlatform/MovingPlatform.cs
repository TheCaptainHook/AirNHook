
using System.Collections;
using UnityEngine;
using System;
using Mirror;

public class MovingPlatform : ActivatableObjectEntity
{
    [CustomHeader("Moving Platform")]
    public Vector2[] paths;

    [ContextMenu("Add Current Position")]
    public void AddCurrentPosition()
    {
        if (paths.Length > 0)
        {
            Vector2[] newPaths = new Vector2[paths.Length + 1];
            for (int i = 0; i < paths.Length; i++)
            {
                newPaths[i] = paths[i];
            }
            newPaths[paths.Length] = transform.position;
            paths = newPaths;
        }
        else
        {
            paths = new Vector2[1];
            paths[0] = transform.position;
        }


    }

    [Range(0, 5)]
    public float moveSpeed;



    private MovingPlatform_Net MovingPlatform_Net;
    public NetworkRigidbodyUnreliable2D netRb;
    protected override void Awake()
    {
        MovingPlatform_Net = GetComponent<MovingPlatform_Net>();
        netRb = GetComponent<NetworkRigidbodyUnreliable2D>();
    }


    #region  GET,SET (Will take care this logic)
    public override T GetData<T>()
    {
        if (typeof(T) == typeof(ButtonActivatableObjectStruct))
        {
            return (T)(object)new ButtonActivatableObjectStruct(id, transform.position, transform.rotation, transform.localScale, activeRequirAmount, indicatorStruct, paths, moveSpeed);
        }

        return default(T);

    }

    public override async void SetData<T>(T data)
    {

        if (typeof(T) == typeof(ButtonActivatableObjectStruct))
        {
            util = new Util();
            ButtonActivatableObjectStruct objData = (ButtonActivatableObjectStruct)(object)data;
            ButtonActivatedObjectStruct = objData;
        }

        //Moving Platform
        paths = ConvertPaths(ButtonActivatedObjectStruct.paths);
        moveSpeed = ButtonActivatedObjectStruct.moveSpeed;

        if (Application.isPlaying)
        {
            MovingPlatform_Net.Server_InitSync();

            await util.Delay(() => { CheckActiveRequirAmount(); });
        }

    }
    #endregion

    //--------------------------------------------------------------------------------------------------------Refectoring 0406

    [ReadOnly]
    public Vector2 dir;
    // public event Action<Vector2> movingEvent;
    private void FixedUpdate()
    {
        if (!MovingPlatform_Net.onActive) return;
        if (onArrivalPoint) return;

        onArrivalPoint = MoveToward();
    }
    [ReadOnly]
    public bool onArrivalPoint;

    private bool MoveToward()
    {
        if (MovingPlatform_Net.targetPosition == null) return true;
        if (CheckDistance(_rb.position, MovingPlatform_Net.targetPosition))
        {
            dir = Vector2.zero;
            return true;
        }
        dir = GetMovePosition();
        _rb.position += dir;
        //_rb.MovePosition(_rb.position + dir);

        return false;
    }
    private Vector2 GetMovePosition()
    {
        return (MovingPlatform_Net.targetPosition - _rb.position).normalized * MovingPlatform_Net.dataPath.moveSpeed * Time.fixedDeltaTime;
    }

    /**
        1. MovingPlatform.FixedUpdate -> if(!onActive) return; = return
        2. MovingPlatform_Net.Server_FixedUpdateReady(paths.Length>0); -> onFixedUpdataReady = true;
        3. MovingPlatform_Net.FixedUpdate ->
        4. MovingPlatform.CheckActiveRequirAmount(); -> Server_OnActive
        5. MovingPlatform.FixedUpdate -> MoveToward.
    **/
    //--------------------------------------------------------------------------------------------------------Refectoring 0406



    #region  Activatable
    public override void Activation()
    {
        // onActive = true;
        MovingPlatform_Net.onActive = true;
    }
    public override void Deactivated()
    {
        MovingPlatform_Net.onActive = false;
    }
    #endregion

    #region  Util


    private bool CheckDistance(Vector2 curPos, Vector2 targetPos)
    {
        if (Vector3.Distance(curPos, targetPos) < 0.1f)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// This function adds the first index’s transform position to the paths array.
    /// </summary>
    /// <param name="paths"></param>
    /// <returns></returns>
    private Vector2[] ConvertPaths(Vector2[] paths)
    {
        if (Application.isPlaying)
        {
            Vector2[] targetPaths = new Vector2[paths.Length + 1];
            targetPaths[0] = transform.position;
            for (int i = 1; i <= paths.Length; i++)
            {
                targetPaths[i] = paths[i - 1];
            }
            return targetPaths;
        }

        return paths;
    }

    #endregion

    #region Clean
    public override void Clean()
    {
        
    }
#endregion


}

