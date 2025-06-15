
using System.Collections;
using UnityEngine;

public class HydraulicPress : ActivatableObjectEntity
{
    [CustomHeader("HydraulicPress")]

    #region Components
    private Animator animator;
    #endregion

    #region Steam Ani
    [Header("Steam Ani")]
    [SerializeField] Animator _UPStem;
    [SerializeField] Animator _DownStem;
    #endregion  


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private HydraulicPress_Net Net => GetComponent<HydraulicPress_Net>();
    public override async void SetData<T>(T data)
    {
        base.SetData(data);

        if (Application.isPlaying)
        {
            Net.onSync = true;
            Net.Server_InitSync();  

            await util.Delay(() => { CheckActiveRequirAmount(); });
        }
        

    }

    
    protected override void Activation()
    {   
        Net.Server_Press(true);
    }

    
    protected override void Deactivated()
    {
        Net.Server_Press(false);
    }

    public  void SteamOn(){
        IsAnimationPlaying("Steam");
    }

 public void IsAnimationPlaying(string animationName)
    {
        AnimatorStateInfo stateInfo1 = _UPStem.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo stateInfo2 = _DownStem.GetCurrentAnimatorStateInfo(0);

        if(!stateInfo1.IsName(animationName)){
            _UPStem.Play(animationName);
        }
         if(!stateInfo2.IsName(animationName)){
            _DownStem.Play(animationName);
        }

    }
   
}
