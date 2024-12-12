using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationData
{
    public int WalkParameterHash { get; protected set; }
    public int JumpParameterHash { get; protected set; }
    public int FallingParameterHash { get; protected set; }
    public int RespawningParameterHash { get; protected set; }
    public int RespawnEndParameterHash { get; protected set; }
    
    public Dictionary<DamageType, int> DeathParameterHashes { get; private set; }
    
    
    public PlayerAnimationData()
    {
        WalkParameterHash = Animator.StringToHash(GlobalText.MOVE_ANIMATION_STRING);
        JumpParameterHash = Animator.StringToHash(GlobalText.JUMP_ANIMATION_STRING);
        FallingParameterHash = Animator.StringToHash(GlobalText.JUMPING_ANIMATION_STRING);
        RespawningParameterHash = Animator.StringToHash(GlobalText.RESPAWNING_ANIMATION_STRING);
        RespawnEndParameterHash = Animator.StringToHash(GlobalText.RESPAWNEND_ANIMATION_STRING);
        DeathParameterHashes = new Dictionary<DamageType, int>
        {
            { DamageType.Default, Animator.StringToHash(GlobalText.DEFAULT_DEATH_ANIMATION_STRING) },
            { DamageType.Fire, Animator.StringToHash(GlobalText.FIRE_DEATH_ANIMATION_STRING) },
            { DamageType.Electric, Animator.StringToHash(GlobalText.ELECTRIC_DEATH_ANIMATION_STRING) },
            { DamageType.Suicide, Animator.StringToHash(GlobalText.SUICIDE_DEATH_ANIMATION_STRING) }
        };
    }
}
