using System.Collections.Generic;

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
    public const string AIR_BALLON_USINGBTN_STRING = "IsUsingBallonButton";
    public const string AIR_BALLON_EXHAILING_STRING = "BallonButton_Exhailing";
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
    public const string ROBOT_SPEAK = "Beep";
    public const string PLAYER_SPEAK = "Meh";
    public const string BUTTON_PRESS_SOUND_1 = "Button1";
    public const string BUTTON_LEVER_SOUND_1 = "Lever1";
    public const string DOOR_SOUND_1 = "Door1";
    public const string DOOR_SOUND_2 = "Door2";
    public const string DOOR_SOUND_3 = "Door3";
    public const string DOOR_SOUND_4 = "Door4";
    public const string DOOR_SOUND_5 = "Door5";
    public const string BUTTON_PRESS_SOUND_2 = "Press_Down";
    public const string BUTTON_RELEASE_SOUND_1 = "Press_Up";
    public const string ENERGY_UP_SOUND = "Energy_Up";
    public const string ENERGY_HUMMING_SOUND = "Energy_Humming";
    public const string ROCK_DESTROY_SOUND = "Rock_Destroy";
    public const string ELECTRIC_SHOCK_SOUND = "Electric_Shock";
    public const string HIT_SOUND = "Hit";
    public const string CANNON_FIRE_SOUND = "Cannon_Fire";
    public const string LOCKER_OPEN_SOUND = "Locker_Open";
    public const string LOCKER_CLOSE_SOUND = "Locker_Close";
    public const string KEY_SOUND = "KeyPing";
    public const string PLAYER_RESURRECT = "Resurrect";
    public const string UI_PING = "Ping";
    public const string PLAYER_JUMP = "Jump1";
    //Laser
    public const string LASER_BEAM_START = "Laser_Beam_Start";
    public const string LASER_BEAM_LOOP = "Laser_Beam_Loop";
    public const string LASER_BEAM_END = "Laser_Beam_End";
    //Tesla Tower
    public const string TESLATOWER_ON = "TeslaTower_On";
    //BeamDoor
    public const string BEAMDOOR_HUMMING = "Energy_Humming";
    //Capsule
    public const string CAPSULE_UNCAPSULING = "CapsulePowerDown";
    //Saw Object
    public const string SAW_SOUND_LOOP = "Saw_Loop";
    //Drone
    public const string DRONE_LASER_SOUND = "Drone_Laser";
    public const string ALERT_SOUND = "Alert";
    //Computer
    public const string COMPUTER_SELECTMENU_SOUND_1 = "KeyClick";
    public const string COMPUTER_SELECTMENU_SOUND_2 = "ComputerButton";
    //LaserObject
    public const string LASER_HIT_SOUND = "";
    //HydraulicPress
    public const string HYDRAULICPRESS_START = "Hydraulic_Start";
    public const string HYDRAULICPRESS_LOOP = "Hydraulic_Loop";
    public const string HYDRAULICPRESS_END = "Hydraulic_End";
    public const string HYDRAULICPRESS_STEAM = "Steam";
    #endregion

    #region Death Sound
    public const string PLAYER_DEATH = "Splat";
    public const string PLAYER_SUICIDE_EXPLOSION = "Explosion";

    public static readonly IReadOnlyDictionary<DamageType, string> DeathSoundDictionary =
        new Dictionary<DamageType, string>
        {
            { DamageType.Default, PLAYER_DEATH },
            { DamageType.Fire, PLAYER_DEATH },
            { DamageType.Boom, PLAYER_SUICIDE_EXPLOSION },
            { DamageType.Electric, PLAYER_DEATH },
            { DamageType.Suicide, PLAYER_SUICIDE_EXPLOSION }
        };
    #endregion

    #region BGM
    public const string TITLE_SOUND = "Danya Vodovoz - High NRG (mp3cut.net)";
    public const string LOBBY_SOUND = "LobbyMusic_Onion";
    public const string TRACK_RACE_SOUND = "WBA Free Track - Race Against Sunset";
    public const string TRACK_LEGEND_SOUND = "WBA Free Track - Legend";
    public const string TRACK_HACKERS_SOUND = "WBA Free Track - Hackers";
    public const string TRACK_LASTSTOP_SOUND = "WBA Free Track - Last Stop";
    #endregion

    #endregion

    #endregion

    #region Achievement Id
    //Player
    public const string PLAYER_JUMPING_100 = "Player_Jumping_100";
    public const string PLAYER_DEATH_1 = "Player_Death_First";
    //Object
    public const string USE_PORTAL_1 = "Use_Portal_1";
    public const string USE_PORTAL_50 = "Use_Portal_50";

    #endregion

    #region ColorPicker
    public const string HUETEXTURE = "HueTexture";
    public const string SATVALTEXTURE = "SatValTexture";
    public const string OUTPUTTEXTURE = "OutputTexture";
    #endregion

    #region MapEditor
    public const string LOBBY = "Lobby";
    public const string SHADOW_PREFAB_PATH = "Prefabs/MapEditor/ShadowCaster";
    public const string TILEBASE_CABLE = "Prefabs/MapEditor/Tile/100";
    public const string TILEBASE_CHAIN = "Prefabs/MapEditor/Tile/101";
    public const string CAPSULE_OBJECT = "Prefabs/MapEditor/CapsulObject";
    public const string ACTIVATABLE_OBJECT_INDICATOR_VAR_1_Path = "Prefabs/MapEditor/ActivatableObject_Indicator/ActivatableObject_Indicator_var1";
    public const string ACTIVATABLE_OBJECT_INDICATOR_VAR_2_Path = "Prefabs/MapEditor/ActivatableObject_Indicator/ActivatableObject_Indicator_var2";
    public const string ACTIVATABLE_OBJECT_INDICATOR_VAR_2_Item = "Prefabs/MapEditor/ActivatableObject_Indicator/Item";
    #endregion
}
