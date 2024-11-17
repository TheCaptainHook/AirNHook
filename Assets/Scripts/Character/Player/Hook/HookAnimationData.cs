using UnityEngine;

public class HookAnimationData : PlayerAnimationData
{
    public int GrabbingParameterHash { get; private set; }
    public int GrapplingParameterHash { get; private set; }
    public int SwingForceParameterHash { get; private set; }

    public HookAnimationData() : base()
    {
        GrabbingParameterHash = Animator.StringToHash(GlobalText.GRABBING_ANIMATION_STRING);
        GrapplingParameterHash = Animator.StringToHash(GlobalText.GRAPPLING_ANIMATION_STRING);
        SwingForceParameterHash = Animator.StringToHash(GlobalText.SWINGING_ANIMATION_STRING);
    }
}
