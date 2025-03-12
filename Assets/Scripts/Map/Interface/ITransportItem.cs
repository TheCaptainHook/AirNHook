
using UnityEngine;

public interface ITransportItem
{
    public void TransportItem_Constraint(uint netId);
    public void TransportItem_DropItem();
}