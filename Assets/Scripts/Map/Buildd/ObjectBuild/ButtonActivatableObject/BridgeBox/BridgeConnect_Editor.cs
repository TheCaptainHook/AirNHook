
using UnityEngine;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif
[ExecuteInEditMode]
public class BridgeConnect_Editor : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Vector2 offset;
    private bool onBridge;

    private GameObject dumeObj;


    // 0113
    BridgeBox bridgeBox;
    Vector2 previousPosition;
    Vector3 previousRot;
    float previousBridgeLength;


    #if UNITY_EDITOR
    #region  Init
    public void Init(){

        bridgeBox = GetComponent<BridgeBox>();

        foreach(Transform transform in transform){
            if(transform.TryGetComponent(out LineRenderer component)){
                lineRenderer = component;
            }
            // if(transform.name == "Sprite"){
            //     dumeObj = Instantiate(transform.gameObject);
            //     DumeSetting();
            // }
        }

        if(dumeObj == null)
        {
            dumeObj = Dume();
            if(dumeObj != null) DumeSetting();
            
        }

        previousPosition = bridgeBox.transform.position;
        previousRot = bridgeBox.transform.rotation.eulerAngles;
        previousBridgeLength = bridgeBox.bridgeLength;

        if(previousBridgeLength >=1)
        {
            RefrashBridge();
        }

    }
    #endregion
    
    private Vector2 GetOffset(){
        return lineRenderer.transform.position - transform.position;
        
    }

    private bool CheckValueAndRefrash()
    {
        bool pot;
        bool rot;
        bool len;

        pot = previousPosition == (Vector2)bridgeBox.transform.position;
        rot = previousRot == bridgeBox.transform.rotation.eulerAngles;
        len = previousBridgeLength == bridgeBox.bridgeLength;

        return pot && rot && len;
    }

    private void RefrashPreviousValue()
    {
        previousPosition = transform.position;
        previousRot = transform.rotation.eulerAngles;
        previousBridgeLength = bridgeBox.bridgeLength;
    }
    public void Refrash()
    {
        if(bridgeBox == null) return;

        if(!CheckValueAndRefrash())
        {
            RefrashPreviousValue();

            if(previousBridgeLength <1) DisconnectBridge();
            else RefrashBridge();
            
        }
    }
    // public void RefrashTrnasform(Transform baseTransform,float length){
    //     if(onBridge){
    //         //Setting Position and Refrash Bridge
    //         RefrashBridge(baseTransform,length);
    //     }
    // }
    public void RefrashBridge(){
        if(!onBridge) onBridge = true;
        Vector2 dir = transform.right;
        Vector2 targetPot = (Vector2)transform.position + dir*previousBridgeLength;
        
        // GetOffset();
        
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0,lineRenderer.gameObject.transform.position);
        lineRenderer.SetPosition(1,targetPot);

        dumeObj.transform.rotation = transform.rotation;
        dumeObj.transform.position = targetPot + GetOffset();

        if(!dumeObj.activeSelf) dumeObj.SetActive(true);
    }

    public void DisconnectBridge(){
        onBridge = false;
        if(dumeObj != null) dumeObj.SetActive(false);
        lineRenderer.positionCount =1;
    }

    public void Reset(){
        if(dumeObj != null){
            Undo.DestroyObjectImmediate(dumeObj);
        }
        if(lineRenderer != null)
        lineRenderer.positionCount = 1;
    }
   

    
#region Dume

    private GameObject Dume()
    {
        GameObject spriteObj = null;
        foreach(Transform tr in transform)
        {
            if(tr.TryGetComponent(out SpriteRenderer component))
            {
                spriteObj = tr.gameObject;
                break;
            }
        }

        return spriteObj != null ? Instantiate(spriteObj) : null;
    }
    private void DumeSetting(){
        if(dumeObj == null) return;
        dumeObj.SetActive(false);
        dumeObj.name = "DumeObject";
        dumeObj.transform.localScale = new Vector3(-1,1,1);
        Color color = dumeObj.GetComponent<SpriteRenderer>().color;
        color.a = 0.5f;

        dumeObj.GetComponent<SpriteRenderer>().color = color;
    }
    
#endregion
    #endif
}
