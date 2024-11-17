using UnityEngine;

[CreateAssetMenu(fileName = "HookCharacter", menuName = "PlayerData/HookCharacter")]
public class HookDataSO : PlayerDataSO
{
    [field: SerializeField] public float swingForce { get; private set; }
    [field: SerializeField] public float swingJumpForce { get; private set; }
    [field: SerializeField] public float climbSpeed { get; private set; }
    [field: SerializeField] public float ropeSpeed { get; private set; }
    [field: SerializeField] public float ropeMaxDistance { get; private set; }
    [field: SerializeField] public float coolDown { get; private set; }
    
    [field: Header("Inhaled")] 
    [field: SerializeField] public float inhalePower { get; private set; }
}
