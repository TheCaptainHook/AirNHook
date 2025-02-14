using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class OtherContainer : MonoBehaviour
{
   public List<Transform> group;

    public void SetGroup(Transform tr){
        if(group == null) group = new();

        if(!group.Contains(tr)){
            group.Add(tr);
        }
        
    }
    public List<T> GetTypeObject<T>(){
        CheckNull();
        List<T> list = new();
        if(group == null) return null;
        foreach(Transform tr in group){
            foreach(Transform item in tr){
                if(item.TryGetComponent(out BuildObj component)){
                    list.Add(component.GetData<T>());
                }
            }
        }
        return list;
    }

    public void GetCompareVec(Vector2 vec,ref List<GameObject> list)
    {
        foreach(Transform tr in group)
        {
            foreach(Transform defTr in tr)
            {
                if(CompareVec(defTr.position,vec))
                {
                    list.Add(defTr.gameObject);
                    return;
                }
            }
        }
        
    }

    private void CheckNull()
    {
        if(group == null ||  group.Count == 0) return;
        for (int i = group.Count - 1; i >= 0; i--)
        {
            if (group[i] == null)
            {
                group.RemoveAt(i);
            }
        }
    }

    private bool CompareVec(Vector3 p1,Vector3 p2){
        bool x = Mathf.Approximately(p1.x,p2.x);
        bool y = Mathf.Approximately(p1.y,p2.y);

        return x&&y;
    }
}
