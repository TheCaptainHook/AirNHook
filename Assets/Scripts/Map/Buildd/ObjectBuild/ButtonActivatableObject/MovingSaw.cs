
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;


public class MovingSaw :  ActivatableObjectEntity
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

    [SerializeField] private GameObject _greenLight;
    private Animator _animator;

  private void Awake()
  {
      _rb = GetComponent<Rigidbody2D>();
    _animator = GetComponent<Animator>();
    _collider = GetComponent<Collider2D>();
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
            addForcePlatform = GetComponent<AddForcePlatform>();
            
            ButtonActivatableObjectStruct objData = (ButtonActivatableObjectStruct)(object)data;
            ButtonActivatedObjectStruct = objData;
            //Moving Platform
            paths = ConvertPaths(objData.paths);
            moveSpeed = objData.moveSpeed;
            addForcePlatform.Init();
            }
        }catch(Exception ex){
                Debug.Log($"{ex},{typeof(T)}");
        }

        if (Application.isPlaying)
        {
            Util util = new Util();
            await util.Delay(() => { CheckActiveRequirAmount(); });
            Prograss();
        }
    }
    #endregion

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
            MoveAction?.Invoke(dir*step);
            yield return null; 
        }
    }
    #region  SawObj

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 객체가 IDamageable 인터페이스를 가지고 있는지 확인
        if (other.TryGetComponent(out IDamageable damageable) && !turnOff)
        {
            // If successful, apply damage
            damageable.TakeDamage();
        }
    }
    #endregion
    #region  Activatable
    protected override void Activation()
    {
        _greenLight.SetActive(true);
        onActive = true;
        turnOff = false;
        _animator.enabled = true;
    }
    protected override void Deactivated()
    {
        _greenLight.SetActive(false);
        onActive = false;
        _animator.enabled = false;
        turnOff = true;
    }
    #endregion

    #region  Util
    private void MoveTowards(Vector2 curP,Vector2 target){
        dir = (target-curP).normalized;
        step = moveSpeed * Time.fixedDeltaTime; 
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

