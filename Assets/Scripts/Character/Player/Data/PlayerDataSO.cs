using UnityEngine;

[CreateAssetMenu(fileName = "DefaultCharacter", menuName = "PlayerData/DefaultCharacter")]
public class PlayerDataSO : ScriptableObject
{
    [field: SerializeField] public CharacterType characterType { get; private set; }

    [field: Header("Movement")]
    [field: SerializeField] public float moveSpeed { get; private set; }
    [field: SerializeField] public float jumpPower { get; private set; }
    [field: SerializeField] public LayerMask floorLayerMask { get; private set; }
    [field: SerializeField] public float coyoteTime { get; private set; }
    
    [field: Header("Interaction")]
    [field: SerializeField] public LayerMask interactableLayerMask { get; private set; }
    [field: SerializeField] public LayerMask obstacleLayerMask { get; private set; }
    [field: SerializeField] public float detectDistance { get; private set; }
}
