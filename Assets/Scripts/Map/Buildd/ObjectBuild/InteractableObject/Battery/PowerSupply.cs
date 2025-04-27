using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSupply : ButtonEntity,IInteractable
{
    [CustomHeader("Power Supply")]
    [SerializeField] Transform socketPosition;
    [Header("Effect")]
    [SerializeField] Material mat;
    [SerializeField] Transform lineContainer;
    [ReadOnly]
    public Battery battery;
    private WaitForSeconds waitForSeconds = new WaitForSeconds(1);
    [Space(20)]
    [Header("Interacte")]
    public ObjectTypeEnum _objectType = ObjectTypeEnum.Interaction;
    [SerializeField] float _BtnOffset;
    private UI_Base _E_Btn;

    private Util util = new();
    //SetData -> FindTargetObject -> Add List Target Object
    //Activation -> PrograssButtonActivatedObject

    private PathFinder pathFinder;


    #region  Network
    private PowerSupply_Net P_Net => GetComponent<PowerSupply_Net>();

    #endregion

    private void Awake()
    {
        pathFinder = GetComponent<PathFinder>();
    }

    #region  Get,Set


    public override T GetData<T>()
    {
        if (typeof(T) == typeof(ButtonObjectStruct))
        {
            return (T)(object)new ButtonObjectStruct(
                id, 
            GetTargetPositions(), 
            GetLightPositions(),
            transform.position, 
            transform.rotation,
            transform.localScale, 
            false);
        }

        return default(T);
    }

    public async override void SetData<T>(T data)
    {
         if (typeof(T) == typeof(ButtonObjectStruct))
            {
                 ButtonObjectStruct buttonData = (ButtonObjectStruct)(object)data;
                 ButtonObjectData = buttonData;

                if(Application.isPlaying)

                await util.Delay(()=>
                {
                    FindTargetObject();
                    if(buttonData.lightPositions.Count > 0) FindLightObject();
                    CreateLine(P_Net.targets.targetPositions);
                    P_Net.onSync = true;

                    P_Net.Server_SetInit();

                });
            }

        
        
    }
    public Vector2 GetSocketPosition(){
        return socketPosition.position;
    }
 
    #endregion


    #region Active,Deactive
    protected override void Activation()
    {
        PrograssButtonActivatedObject(true);
        // LineOn(true);
    }
    protected override void Deactivated()
    {
        PrograssButtonActivatedObject(false);
        // LineOn(false);
    }
    public void Net_Activation()
    {
        Activation();
    }
    public void Net_Deactivated()
    {
        Deactivated();
    }

     protected override void PrograssButtonActivatedObject(bool onActivate)
    {
        TogglePowerSupply(onActivate);
    }
    
    private void TogglePowerSupply(bool toggle)
    {
        if(targetObjects.Count == 0) return; // This field is for server settings only
        // foreach(var item in targetObjects){
        foreach(var item in P_Net.targets.targets){
            if(item.TryGetComponent(out IPowerConsumer component)){
                if(toggle) component.PowerOn();
                else component.PowerOff();
            }
        }
        foreach(var item in lightObjects)
        {
            if(item.TryGetComponent(out LightObjectEntity component))
            {
                Debug.Log(item.name);
                component.hasPower = toggle;
            }
        }
    }
    #endregion

    #region Find Target

    public override void FindTargetObject()
    {
        // if(!Application.isPlaying) return;
        if(targetPosition.Count == 0) return;

        List<GameObject> objList = new();
        foreach(Vector2 vec in targetPosition){
            GameObject matchedObj = null;
            foreach(Transform tr in MapEditor.Instance.buttonActivatableObjectTransform){
                if(tr.TryGetComponent(out ActivatableObjectEntity component))
                {
                  if(CompareVec(component.ButtonActivatedObjectStruct.position,vec))
                  {
                    matchedObj = tr.gameObject;
                        objList.Add(matchedObj);
                        break;
                  }          
                }
            }

            if(matchedObj != null) continue;

            foreach(Transform tr in MapEditor.Instance.buttonObjectTransform){
                if(tr.TryGetComponent(out ButtonEntity component))
                {
                  if(CompareVec(component.ButtonObjectData.position,vec))
                  {
                        objList.Add(tr.gameObject);
                        break;
                  }          
                }
            }
            
        }

         targetObjects = objList;
        //Network Sync
        P_Net.Server_SetTargets(objList,targetPosition);
    }

    #if UNITY_EDITOR
    public async override void Editor_Setting(MapEditor mapEditor)
    {
        if(targetPosition.Count == 0) return;

            await util.Delay(()=>{
                List<GameObject> objList = new();
                foreach (Vector2 vec in targetPosition)
                {
                    GameObject matchedObj = null;
                    foreach (Transform tr in mapEditor.buttonActivatableObjectTransform)
                    {
                        if (tr.TryGetComponent(out ActivatableObjectEntity component))
                        {
                            if (CompareVec(component.ButtonActivatedObjectStruct.position, vec))
                            {
                                matchedObj = tr.gameObject;
                                objList.Add(matchedObj);
                                break;
                            }
                        }
                    }

                    if (matchedObj != null) continue;

                    foreach (Transform tr in mapEditor.buttonObjectTransform)
                    {
                        if (tr.TryGetComponent(out ButtonEntity component))
                        {
                            if (CompareVec(component.ButtonObjectData.position, vec))
                            {
                                objList.Add(tr.gameObject);
                                break;
                            }
                        }
                    }
                }

                targetObjects = objList;

                List<GameObject> list = new();
                OtherContainer otherContainer = mapEditor.otherContainer.GetComponent<OtherContainer>();

                foreach(Vector2 vec in ButtonObjectData.lightPositions)
                {
                    otherContainer.GetCompareVec(vec,ref list);
                    //otherObject vec 전달 -> group transform 순회 같은거 있는지 확인 -> 있으면 해당 IPowerConsumer 반환
                }
                lightObjects = list;
                //Debug.Log($"Light Object Count : {lightObjects.Count}");

            });
    }
    #endif


    #endregion


 #region  Main
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out HookSM component))
        {
            Transform grabItem = component.GetGrabbedItem();
            if (grabItem != null)
            {
                if (grabItem.TryGetComponent(out Battery battery))
                {
                    // ShowEButton(); 
                    P_Net.Cmd_ShowE(component.gameObject,true);

                    // battery.powerSupply = this;
                    // P_Net.Cmd_SetBattery(battery.gameObject);
                    battery.Net_SetPowerSupply(gameObject);
                }
            }
            else
            {
                if(P_Net.battery) P_Net.Cmd_ShowE(component.gameObject,true);
            }
          
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out HookSM component))
        {

            Transform grabItem = component.GetGrabbedItem();
            if (grabItem != null)
            {
                if (grabItem.TryGetComponent(out Battery battery))
                {
                    P_Net.Cmd_ShowE(component.gameObject,false);

                    // P_Net.Cmd_SetBattery(null);
                    battery.Net_SetPowerSupply(null);
                }

            }
        }
    }
    
    public void LineOn(bool onoff){
        foreach(Transform tr in lineContainer){
            tr.gameObject.SetActive(onoff);
        }
    }
    
    
#endregion


#region  Network
    public void SetBattery(GameObject battery)
    {
        P_Net.Cmd_SetBattery(battery);
    }
#endregion



#region  Interacable
     public void Interaction(Transform accessor = null){
            if(P_Net.battery){
                if(_E_Btn != null) HideE();
                P_Net.Cmd_SetBattery(null);
            }
       
    }

    public bool CanInteract(){
        return true;
    }

    public void Interacting(bool value)
    {
        return;
    }

    public ObjectTypeEnum GetObjectType(){
        return _objectType;
    }

    public void ShowEButton(){
        return;
    }
    public void HideEButton(){
       return;
    }

    public void ShowE()
    {
        _E_Btn = Managers.UI.ShowUI<UI_ShowEButton>();
        _E_Btn.transform.position = transform.position + (transform.up * _BtnOffset);
    }
    public void HideE()
    {
        _E_Btn = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }
    
    

#endregion

#region  Draw Line
    public void CreateLine(List<Vector2> list){

        Vector2 startPot = lineContainer.position;
        
        foreach(var position in list)
        {
            Vector2 endPot = position;
            LineRenderer line = GeneratorLineRenderer();
            SetLine(line,pathFinder.FindPath(startPot,endPot,true,Direction_Type.Four));
            line.gameObject.SetActive(false);
        }
    }
     private LineRenderer GeneratorLineRenderer(){

        GameObject obj = new GameObject("LineRenderer");
        LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = mat;
        lineRenderer.positionCount = 0;
        lineRenderer.sortingLayerName ="Map/Tiles";
        lineRenderer.sortingOrder = 3;
        obj.transform.SetParent(lineContainer);

        return lineRenderer;
    }
    //  private void SetLine(LineRenderer lineRenderer,List<Vector2Int> path){
    //    lineRenderer.positionCount = path.Count;
    //    for(int i = 0;i<path.Count;i++)
    //    {
    //         Vector3 worldPosition = pathFinder.GridToWorld(path[i]);
    //         lineRenderer.SetPosition(i, worldPosition);
    //    }
    // }
     private void SetLine(LineRenderer lineRenderer,List<Vector2> path){
       lineRenderer.positionCount = path.Count;
       for(int i = 0;i<path.Count;i++)
       {
            Vector3 worldPosition = path[i];
            lineRenderer.SetPosition(i, worldPosition);
       }
    }
#endregion
}
