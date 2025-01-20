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



    private void CheckNull()
    {
        for (int i = group.Count - 1; i >= 0; i--)
        {
            if (group[i] == null)
            {
                group.RemoveAt(i);
            }
        }
    }
}
