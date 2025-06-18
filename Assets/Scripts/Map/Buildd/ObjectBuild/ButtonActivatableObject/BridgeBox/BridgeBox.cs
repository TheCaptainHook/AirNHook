
public class BridgeBox : ActivatableObjectEntity
{
    [CustomHeader("Bridge Box")]
    public float bridgeLength;


    protected override void AdditionalInspectorConfig()
    {
        bridgeLength = ButtonActivatedObjectStruct.bridgeLength;
    }
    
    #region  Main
    public override void Activation()
    {
        Net.Server_ChangeOnActive(true);

    }
    public override void Deactivated()
    {
        Net.Server_ChangeOnActive(false);

    }


    #endregion
}

