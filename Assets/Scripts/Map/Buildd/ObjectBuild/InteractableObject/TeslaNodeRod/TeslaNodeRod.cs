using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeslaNodeRod : BuildObj
{
    [CustomHeader("Tesla Node Rod")]

    public Transform headPoint;
    public LayerMask detectLayerMask;
    public bool isPowerSupplied;
    public float maxResetRate = 2f;
    private float curResetRate = 0;
    public float supplyEnergyRadius;
    private TeslaNodeRod_Net Net => GetComponent<TeslaNodeRod_Net>();
    void Awake()
    {
        DissolveInitSetting();
    }

    #region Get,Set
  
    public override void SetData<T>(T data)
    {
        base.SetData(data);
        Net.onSync = true;
        Net.Server_InitSync();
    }
    #endregion

    void Update()
    {
        if(isPowerSupplied)
        {
            curResetRate += Time.deltaTime;
            if(curResetRate> maxResetRate)
            {
                curResetRate = 0;
                isPowerSupplied = false;
                PowerSupplyOff();
            }
        }
    }

    public override void TakeDamage(DamageType damageType = DamageType.Default)
    {
        if(damageType == DamageType.Electric)
        {
            PowerSupply();
        }
        else base.TakeDamage(damageType);
    }
#region  Power
    private Collider2D[] targets;
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

        curResetRate = 0;
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
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(headPoint.position,supplyEnergyRadius);
    }
    #endregion
}

public class CustomHashSet_TeslaNode<T>
{
    private readonly List<T> list;
    private readonly HashSet<T> hashSet;

    public int Count => list.Count;
    public T this[int index] => list[index];
    public bool Add(T item)
    {
        if(hashSet.Add(item))
        {
            list.Add(item);
            return true;
        }
        return false;
    }
    public bool Remove(T item)
    {
        if(hashSet.Remove(item))
        {
            list.Remove(item);
            return true;
        }
        return false;
    }
    public void Clear()
    {
        list.Clear();
        hashSet.Clear();
    }

    public bool Contains(T item) => hashSet.Contains(item);
    public List<T>.Enumerator GetEnumerator() => list.GetEnumerator();

}
