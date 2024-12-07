
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

    #if UNITY_EDITOR
    #region  Init
    public void Init(){
        foreach(Transform transform in transform){
            if(transform.TryGetComponent(out LineRenderer component)){
                lineRenderer = component;
            }
            if(transform.name == "Sprite"){
                dumeObj = Instantiate(transform.gameObject);
                DumeSetting();
            }
        }
    }
    #endregion
    
    private Vector2 GetOffset(){
        return transform.position - lineRenderer.transform.position;
        
    }

    public void RefrashTrnasform(Transform baseTransform,float length){
        if(onBridge){
            //Setting Position and Refrash Bridge
            RefrashBridge(baseTransform,length);
        }
    }
    public void RefrashBridge(Transform baseTrasform,float length){
        if(!onBridge) onBridge = true;
        Vector2 dir = baseTrasform.right;
        Vector2 targetPot = (Vector2)transform.position + dir*length;
        
        GetOffset();
        
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0,lineRenderer.gameObject.transform.position);
        lineRenderer.SetPosition(1,targetPot);

        dumeObj.transform.rotation =baseTrasform.rotation;
        dumeObj.transform.position = targetPot - GetOffset();
        dumeObj.SetActive(true);
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
