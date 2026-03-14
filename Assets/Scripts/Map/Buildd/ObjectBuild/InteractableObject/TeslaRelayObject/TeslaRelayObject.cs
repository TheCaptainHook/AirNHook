using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class TeslaRelayObject : InteractableObjectEntity
{
     [CustomHeader("Tesla Relay Object")]

    public Transform headPoint;
    public LayerMask detectLayerMask;
    public bool isPowerSupplied;
    public float maxResetRate = 2f;
    public float curResetRate = 0;
    public float supplyEnergyRadius;
   
    #region Get,Set
  
    #endregion

    //void Update()
    //{
    //    if(isPowerSupplied)
    //    {
    //        curResetRate += Time.deltaTime;
    //        if(curResetRate> maxResetRate)
    //        {
    //            curResetRate = 0;
    //            isPowerSupplied = false;
    //            PowerSupplyOff();
    //        }
    //    }
    //}

    public override void TakeDamage(DamageType damageType = DamageType.Default)
    {
        if(damageType == DamageType.Electric)
        {
            //Server
            if(NetworkServer.active)
            {
                // if(!isPowerSupplied) isPowerSupplied = true;
                // PowerSupply();
                // curResetRate = 0;
            }
            //Server
            
            //Cmd(Effect)
            //Cmd(Effect)
        }
        else base.TakeDamage(damageType);
    }
#region  Power
    private Collider2D[] targets = new Collider2D[5];
    private CustomHashSet_TeslaNode<Collider2D> previousHashSet = new();
    private CustomHashSet_TeslaNode<Collider2D> curDetectTargetHashSet = new();
    private CustomHashSet_TeslaNode<Collider2D> removeBufferHashSet= new();

    private void PowerSupply()
    {   
        int count = Physics2D.OverlapCircleNonAlloc(headPoint.position, supplyEnergyRadius, targets, detectLayerMask);
        if(count == 0) return;

    //Activate power for newly detected objects
       curDetectTargetHashSet.Clear();
       for(int i =0;i<count;i++)
       {
            curDetectTargetHashSet.Add(targets[i]);

            if(targets[i].TryGetComponent(out IPowerConsumer component) && previousHashSet.Add(targets[i]))
            {
                component.PowerOn();
            }
       }
    //Activate power for newly detected objects

    //Deactivate power for objects no longer detected
       removeBufferHashSet.Clear();
       foreach(var item in previousHashSet)
       {
            if(!curDetectTargetHashSet.Contains(item))
            {
                if(item.TryGetComponent(out IPowerConsumer component)) component.PowerOff();
                removeBufferHashSet.Add(item);
            }
       }
    //Deactivate power for objects no longer detected
       
    //Update the previous detection record
        foreach(var item in removeBufferHashSet)
        {
            previousHashSet.Remove(item);
        }
    //Update the previous detection record
 
    }
    

 private void PowerSupplyOff()
 {
    foreach(var item in previousHashSet)
    {
        if(item.TryGetComponent(out IPowerConsumer component))
        {
            component.PowerOff();
        }
        previousHashSet.Clear();
        
    }

 }
    #endregion


    #region  Debug
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(headPoint.position,supplyEnergyRadius);
    }
#endif
#endregion
}

public class CustomHashSet_TeslaNode<T>
{
    private  List<T> list;
    private  HashSet<T> hashSet;

    public int Count => list.Count;
    public T this[int index] => list[index];
    public bool Add(T item)
    {
        if(hashSet == null) hashSet= new();
        if(hashSet.Add(item))
        {
            if(list == null) list = new();
            list.Add(item);
            return true;
        }
        return false;
    }
    public bool Remove(T item)
    {
        if(hashSet == null) return false;
        if(hashSet.Remove(item))
        {
            list.Remove(item);
            return true;
        }
        return false;
    }
    public void Clear()
    {
        if(list != null)
        list.Clear();
        if(hashSet != null)
        hashSet.Clear();
    }

    public bool Contains(T item) => hashSet.Contains(item);
    public List<T>.Enumerator GetEnumerator() => list.GetEnumerator();

}