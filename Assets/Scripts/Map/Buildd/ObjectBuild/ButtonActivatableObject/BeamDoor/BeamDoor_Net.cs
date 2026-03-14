
public class BeamDoor_Net : ActivatableObject_Net_Entity
{
    protected override void SetData(ButtonActivatableObjectStruct data)
    {
        base.SetData(data);
        Main.GetComponent<BeamDoor>().CloseSound();
    }
}
