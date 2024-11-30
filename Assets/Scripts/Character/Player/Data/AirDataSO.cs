using UnityEngine;

[CreateAssetMenu(fileName = "AirCharacter", menuName = "PlayerData/AirCharacter")]
public class AirDataSO : PlayerDataSO
{
    [field: Header("AirGun")]
    [field: SerializeField] public float AirGunDistance { get; private set; }
    [field: SerializeField] public LayerMask objectLayerMask { get; private set; }
    [field: SerializeField] public float minShootPower { get; private set; }
    [field: SerializeField] public float maxShootPower { get; private set; }
    
    [field: Header("HookInteraction")]
    [field: SerializeField] public float flyPower { get; private set; }
    [field: SerializeField] public float stickToHookSpeed { get; private set; }
    
    [field: Header("lineRenderer")]
    [field: SerializeField] public int numberOfPoints { get; private set; }
    [field: SerializeField] public float spaceBetweenPoints { get; private set; }
}
