using UnityEngine;

public class PlayerAnimationData
{
    public int WalkParameterHash { get; protected set; }
    public int JumpParameterHash { get; protected set; }
    public int FallingParameterHash { get; protected set; }
    public int RespawningParameterHash { get; protected set; }
    public int RespawnEndParameterHash { get; protected set; }
    public int DefaultDeathParameterHash { get; protected set; }
    
    public PlayerAnimationData()
    {
        WalkParameterHash = Animator.StringToHash(GlobalText.MOVE_ANIMATION_STRING);
        JumpParameterHash = Animator.StringToHash(GlobalText.JUMP_ANIMATION_STRING);
        FallingParameterHash = Animator.StringToHash(GlobalText.JUMPING_ANIMATION_STRING);
        RespawningParameterHash = Animator.StringToHash(GlobalText.RESPAWNING_ANIMATION_STRING);
        RespawnEndParameterHash = Animator.StringToHash(GlobalText.RESPAWNEND_ANIMATION_STRING);
        DefaultDeathParameterHash = Animator.StringToHash(GlobalText.DEFAULT_DEATH_ANIMATION_STRING);
    }
}
