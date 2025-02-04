
using System.Collections;
using UnityEngine;
using System;
using Unity.VisualScripting;



public class MovingPlatform :  ActivatableObjectEntity
{
    [CustomHeader("Moving Platform")]
    public Vector2[] paths;
    public float moveSpeed;

    [Header("Main")]

    [ReadOnly]
    public float step;
    [ReadOnly]
    public Vector2 dir;
    public event Action<Vector2> MoveAction;
    private AddForcePlatform addForcePlatform;
    private bool onActive;

    // [SerializeField] GameObject rail_Prefabs;
    // [SerializeField] LineRenderer rail_Line;


    private MovingPlatform_Net MovingPlatform_Net => GetComponent<MovingPlatform_Net>();
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        addForcePlatform = GetComponent<AddForcePlatform>();
        addForcePlatform.Init();
    }


    #region  GET,SET (Will take care this logic)
    public override T GetData<T>()
    {
        if(typeof(T)==typeof(ButtonActivatableObjectStruct)){
            return (T)(object)new ButtonActivatableObjectStruct(id,activeRequirAmount,transform.position,transform.rotation,transform.localScale,paths,moveSpeed);
        }
        
        return default(T);

    }
    
    public override async void SetData<T>(T data)
    {
         try{
            if (typeof(T) == typeof(ButtonActivatableObjectStruct))
            {
            //addForcePlatform = GetComponent<AddForcePlatform>();
            
            ButtonActivatableObjectStruct objData = (ButtonActivatableObjectStruct)(object)data;
            ButtonActivatedObjectStruct = objData;

            //Moving Platform
            paths = ConvertPaths(objData.paths);     
            moveSpeed = objData.moveSpeed;
            
            }
        }catch(Exception ex){
                Debug.Log($"{ex},{typeof(T)}");
        }

        if (Application.isPlaying)
        {
            // CreateRail(paths);
            MovingPlatform_Net.Server_CreateRail(paths);

            Util util = new Util();
            await util.Delay(() => { CheckActiveRequirAmount(); });
            Prograss();
        }
    }
    #endregion

    //1213
    // private void CreateRail(Vector2[] paths){ //rail node, rail lineRenderer
    //     Transform parents = MapEditor.Instance.dontSaveObjectTransform;
    //     Transform container = new GameObject("Rail_Container").transform;
    //     container.SetParent(parents);

    //     //Rail Node
    //     LineRenderer line = Instantiate(rail_Line,container);
    //     //Draw Line
    //     DrawLine(line,paths);

    //     Vector2 startPot = line.GetPosition(0);
    //     Vector2 endPot = line.GetPosition(paths.Length-1);

    //     GameObject railNode_1;
    //     GameObject railNode_2;

    //     if(startPot == endPot){
    //         railNode_1 = Instantiate(rail_Prefabs,container);
    //         railNode_1.transform.position = startPot;
    //     }else{
    //         railNode_1 = Instantiate(rail_Prefabs,container);
    //         railNode_1.transform.position = startPot;
    //         railNode_2 = Instantiate(rail_Prefabs,container);
    //         railNode_2.transform.position = endPot;
    //     }

    // }
    // private void DrawLine(LineRenderer line,Vector2[] path){
    //     line.positionCount = path.Length;
    //     for(int i = 0; i<path.Length;i++){
    //         line.SetPosition(i,path[i]+new Vector2(0,0.25f));
    //     }

    // }


    public void AddForce()
    {
        StartCoroutine(AddForceCo());
    }

    IEnumerator AddForceCo()
    {
        while(true)
        {
            //MoveAction?.Invoke(MovingPlatform_Net.velocity);
            addForcePlatform.AddForce(MovingPlatform_Net.velocity);
            yield return null;
        }
    }

    #region Test Code, [latest update: 11/12 ]
    // private void Start(){
    //     paths = ConvertPaths(paths);
    //     Prograss();
    // }
    #endregion

    public void Prograss(){
        if(paths.Length <=0) return;
        StartCoroutine(Prograss_Co(paths));
    }

    IEnumerator Prograss_Co(Vector2[] paths){
        int maxIndex = paths.Length;
        int index = 0;
        int increment = 1;
        Vector2 targetPosition = paths[index];
        
        while (true)
        {
                while(!onActive){
                    dir = Vector2.zero;
                    yield return null;
                }
            if (CheckDistance(_rb.position, targetPosition))
            {
                // _rb.velocity = Vector2.zero;
                _rb.position = targetPosition;

                index += increment;
                if (index >= maxIndex || index < 0)
                {
                    if(index >=maxIndex && paths[maxIndex-1] == paths[0]){
                        index = 0;
                    }else{
                        increment *= -1;
                        index += increment;
                    }
                    
                }

                targetPosition = paths[index];

            }
            
            MoveTowards(_rb.position, targetPosition);
            //MoveAction?.Invoke(dir*step);
            yield return null; 
        }
    }
    #region  Activatable
    protected override void Activation()
    {
        onActive = true;
    }
    protected override void Deactivated()
    {
        onActive = false;
    }
    #endregion

    #region  Util
    private void MoveTowards(Vector2 curP,Vector2 target){
        dir = (target-curP).normalized;
        step = moveSpeed * Time.fixedDeltaTime;

        MovingPlatform_Net.Server_SetVelocity(dir * step,step);

        _rb.position = Vector2.MoveTowards(_rb.position,_rb.position +dir,step);
        
    }

    private bool CheckDistance(Vector2 curPos,Vector2 targetPos){
        if(Vector3.Distance(curPos,targetPos) < 0.1f){
            return true;
        }
        return false;
    }
    /// <summary>
    /// This function adds the first index’s transform position to the paths array.
    /// </summary>
    /// <param name="paths"></param>
    /// <returns></returns>
     private Vector2[] ConvertPaths(Vector2[] paths){
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


}

