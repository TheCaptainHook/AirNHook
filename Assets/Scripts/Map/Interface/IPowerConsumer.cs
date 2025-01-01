
using UnityEngine;

public interface IPowerConsumer
{
    ///Guide for Using the IPowerConsumer Interface
    /// When providing power, call the PowerOn method of the interface and set the hasPower value of the power-supplying object to true.
   public void PowerOn();
   public void PowerOff();
   public Vector2 GetPowerLineConnectionPoint();
}
