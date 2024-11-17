using UnityEngine;

public class AirAnimationData : PlayerAnimationData
{
    public int InhalingParameterHash { get; private set; }
    public int ExhalingParameterHash { get; private set; }
    public int FlyingParameterHash { get; private set; }
    public int HookInhaledParameterHash { get; private set; }
    public int AirAttachedParameterHash { get; private set; }
    

    public AirAnimationData() : base()
    {
        InhalingParameterHash = Animator.StringToHash(GlobalText.INHAILING_ANIMATION_STRING);
        ExhalingParameterHash = Animator.StringToHash(GlobalText.EXHAILING_ANIMATION_STRING);
        FlyingParameterHash = Animator.StringToHash(GlobalText.FLYING_ANIMATION_STRING);
        HookInhaledParameterHash = Animator.StringToHash(GlobalText.HOOK_INHALED_ANIMATION_STRING);
        AirAttachedParameterHash = Animator.StringToHash(GlobalText.AIR_ATTACHED_ANIMATION_STRING);
    }
}
