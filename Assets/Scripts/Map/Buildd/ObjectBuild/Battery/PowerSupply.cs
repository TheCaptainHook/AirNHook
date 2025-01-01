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

    private void Awake()
    {
        pathFinder = GetComponent<PathFinder>();
    }

    #region  Get,Set

    public async override void SetData<T>(T data)
    {
         if (typeof(T) == typeof(ButtonObjectStruct))
            {
                 ButtonObjectStruct buttonData = (ButtonObjectStruct)(object)data;
                 ButtonObjectData = buttonData;
            }

        if(Application.isPlaying){
            await util.Delay(()=>
            {
                FindTargetObject();
                CreateLine();
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
        LineOn(true);
    }
    protected override void Deactivated()
    {
        PrograssButtonActivatedObject(false);
        LineOn(false);
    }

     protected override void PrograssButtonActivatedObject(bool onActivate)
    {
        TogglePowerSupply(onActivate);
    }

    private void TogglePowerSupply(bool toggle)
    {
        if(targetObjects.Count == 0) return;
        foreach(var item in targetObjects){
            if(item.TryGetComponent(out IPowerConsumer component)){
                if(toggle) component.PowerOn();
                else component.PowerOff();
            }
        }
    }
    #endregion

    #region Find Target

    public override void FindTargetObject()
    {
        if(!Application.isPlaying) return;
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
    }
    public async override void Editor_Setting(MapEditor mapEditor)
    {
        if(targetPosition.Count == 0) return;

            await util.Delay(()=>{
                List<GameObject> objList = new();
                    foreach(Vector2 vec in targetPosition){
                        GameObject matchedObj  = null;
                        foreach(Transform tr in mapEditor.buttonActivatableObjectTransform){
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
                    
                        foreach(Transform tr in mapEditor.buttonObjectTransform){
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
            });
       

    }


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
                    ShowEButton();
                    battery.powerSupply = this;
                }
            }
            else
            {
                if(battery != null) ShowEButton();
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
                    HideEButton();
                    battery.powerSupply = null;
                }

            }
        }
    }
    
    private void UseBattery()
    {
       StartCoroutine(UseBatteryCo());
    }
   
     private IEnumerator UseBatteryCo()
     {
        int consumption = targetObjects.Count;

        //Use Battery Effect

        //Use Battery Effect

        while(battery.BatteryCapacity >0)
        {
            battery.BatteryCapacity = -consumption;
            yield return waitForSeconds;
        }
        RemoveSocket();
       
    }

    public void InsertSocket(Battery battery){
        if(this.battery != null){
            RemoveSocket();
        }

        this.battery = battery;
        Activation();
        UseBattery();
    }

    private void RemoveSocket(){
        if(battery){
            //Effect Stop

            //Stop Use to Battery
            StopAllCoroutines();
            Deactivated();
            //Remove Socket
            battery.RemoveSocket();
            battery = null;
        }
    }

    private void LineOn(bool onoff){
        foreach(Transform tr in lineContainer){
            tr.gameObject.SetActive(onoff);
        }
    }
    
    
#endregion

#region  Interacable
     public void Interaction(Transform accessor = null){
            if(battery){
                RemoveSocket();
                HideEButton();
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
         _E_Btn = Managers.UI.ShowUI<UI_ShowEButton>();
        _E_Btn.transform.position = transform.position + (transform.up * _BtnOffset);
    }
    
    public void HideEButton(){
        _E_Btn = null;
        Managers.UI.HideUI<UI_ShowEButton>();
    }

#endregion

#region  Draw Line
    private void CreateLine(){
        Vector2Int startPot = pathFinder.WorldToGrid(transform.position);
        
        foreach(var position in ButtonObjectData.targetPositions)
        {
            Vector2Int endPot = pathFinder.WorldToGrid(position);
            LineRenderer line = GeneratorLineRenderer();
            SetLine(line,pathFinder.FindPath(startPot,endPot));
            line.gameObject.SetActive(false);
        }
    }
     private LineRenderer GeneratorLineRenderer(){

        GameObject obj = new GameObject("LineRenderer");
        LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = mat;
        lineRenderer.positionCount = 0;
        lineRenderer.sortingLayerName ="ForeGround";
        lineRenderer.sortingOrder = 0;
        obj.transform.SetParent(lineContainer);

        return lineRenderer;
    }
     private void SetLine(LineRenderer lineRenderer,List<Vector2Int> path){
       lineRenderer.positionCount = path.Count;
       for(int i = 0;i<path.Count;i++)
       {
            Vector3 worldPosition = pathFinder.GridToWorld(path[i]);
            lineRenderer.SetPosition(i, worldPosition);
       }
    }
#endregion
}
