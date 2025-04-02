public static class GlobalText
{
    #region Animation String
    //public const string IDLE_ANIMATION_STRING = "";
    public const string MOVE_ANIMATION_STRING = "IsMoving";
    public const string RESPAWNING_ANIMATION_STRING = "IsRespawning";
    public const string RESPAWNEND_ANIMATION_STRING = "OnRespawnEnd";
    public const string DEFAULT_DEATH_ANIMATION_STRING = "IsDead";
    public const string JUMP_ANIMATION_STRING = "IsJumping";
    public const string JUMPING_ANIMATION_STRING = "IsStayJumping";
    public const string SUICIDE_ANIMATION_STRING = "TrySuicide";
    public const string SUICIDE_CANCELED_ANIMATION_STRING = "CancelSuicide";
    //HookParameter
    public const string GRABBING_ANIMATION_STRING = "IsGrabbing";
    public const string GRAPPLING_ANIMATION_STRING = "IsGrappling";
    public const string SWINGING_ANIMATION_STRING = "SwingingForce";
    public const string SWINGING_WITH_AIR_ANIMATION_STRING = "IsAirAttached";
    //AirParameter
    public const string INHAILING_ANIMATION_STRING = "IsInhaling";
    public const string EXHAILING_ANIMATION_STRING = "IsExhaling";
    public const string FLYING_ANIMATION_STRING = "IsFlying";
    public const string HOOK_INHALED_ANIMATION_STRING = "IsHookInhaled";
    public const string AIR_ATTACHED_ANIMATION_STRING = "IsAttached";
    public const string AIR_BALLON_USINGBTN_STRING ="IsUsingBallonButton";
    public const string AIR_BALLON_EXHAILING_STRING="BallonButton_Exhailing";
    //DeathParameter
    public const string FIRE_DEATH_ANIMATION_STRING = "DeathByFire";
    public const string ELECTRIC_DEATH_ANIMATION_STRING = "DeathByElectric";
    public const string SUICIDE_DEATH_ANIMATION_STRING = "DeathBySuicide";

    #endregion

    #region Sound String
    public const string AUDIO_SOURCE_PATH = "Prefabs/Sound/AudioSource";
    public const string AUDIO_CLIP_SO_PATH = "Audio/ScriptableObject/AudioClipSO";
    public const string AUDIOMIXER_PATH = "Sounds/AudioMixer";
    public const string EFFECTS_STRING = "Effects";
    public const string BGM_STRING = "BGM";
    public const string MASTER_PARAMETER_STRING = "MasterParam";
    public const string MASTER_VOLUME_STRING = "MasterVolume";
    public const string BGM_PARAMETER_STRING = "BGMParam";
    public const string BGM_VOLUME_STRING = "BGMVolume";
    public const string EFFECT_PARAMETER_STRING = "EffectsParam";
    public const string EFFECT_VOLUME_STRING = "EffectsVolume";

    #region Sound Name String
    
    #region FX
    public const string UI_CLICK_SOUND = "Click1";
    public const string DIALOGUE_CLICK_SOUND = "Click2";
    public const string KET_PRINTING_SOUND = "PrintClang";
    public const string COMPUTER_ON_SOUND = "ComTurnOn";
    public const string COMPUTER_OFF_SOUND = "ComTurnOff";
    #endregion

    #region BGM
        public const string TITLE_SOUND = "Danya Vodovoz - High NRG (mp3cut.net)";
        public const string LOBBY_SOUND = "LobbyMusic_Onion";
        public const string TUTORIAL_SOUND = "WBA Free Track - Race Against Sunset";
        public const string STAGE_1_NORMAL_SOUND = "WBA Free Track - Legend";
        public const string STAGE_1_FINAL_SOUND = "WBA Free Track - Hackers";
    #endregion
    #endregion
    #endregion

    #region Achievement Id
    //Player
    public const string PLAYER_JUMPING_100 = "Player_Jumping_100";
    //Object
    public const string USE_PORTAL_1 = "Use_Portal_1";
    public const string USE_PORTAL_50 = "Use_Portal_50";

    #endregion

    #region MapEditor
    public const string LOBBY = "Lobby";
    public const string SHADOW_PREFAB_PATH = "Prefabs/MapEditor/ShadowCaster";
    public const string TILEBASE_CABLE = "Prefabs/MapEditor/Tile/100";
    public const string TILEBASE_CHAIN = "Prefabs/MapEditor/Tile/101";
    #endregion
}
