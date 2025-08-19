
using UnityEngine;

public interface IPowerConsumer
{
    /**
    [Guide for Using the IPowerConsumer Interface] 
        *** Do not use or check the chargeRequired field in ‘PowerSupply’ ***
    When providing power, call the PowerOn method of the interface and set the hasPower value of the power-supplying object to true.
        1.	Inherit from IPowerConsumer.
        2.	In the Inspector, check the chargeRequired option as true.
        3.	Override the Get and Set methods (+ chargeRequired).
        4.	Define IPowerConsumer, Refer to 'ToggleButtonObject' as an example.
        5.  network sync 'chargeRequired', 'hasPower' field 
    **/
    public int GetConsumption();
    public bool hasPower{get; set;}
   public void PowerOn();
   public void PowerOff();
   public Vector2 GetPowerLineConnectionPoint();
   public Vector2 GetTransformPosition();
}
