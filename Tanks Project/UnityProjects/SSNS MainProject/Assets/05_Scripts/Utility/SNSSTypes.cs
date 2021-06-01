using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SNSSTypes
{
    public enum PlayerRole
    {
        Gunner,
        Pilot,
        None
    };

    public enum AreaType
    {
        Tutorial,
        Regular,
        BossShield,
        BossAttack
    }

    public enum GameState
    {
        MAIN_MENU,
        WARPING,
        BATTLE,
        BATTLE_END,
        RESPAWN,
        PAUSE,
        GAME_OVER,
        VICTORY,
        CREDITS
    }

    public enum DreadnovaState
    {
        SHIELD_STAGE,
        FINAL_STAGE
    }

    public enum AreaState
    {
        BATTLE,
        APPLYING_REWARD,
        TRANSITION_BEGIN,
        TRANSITION_END
    }

    public enum FadeType
    {
        NO_FADE,
        HALF_FADE,
        FULL_FADE
    }

    public enum OpenMenuType
    {
        MENU,
        NODE_MAP
    }

    public enum OwnerType
    {
        AI,
        MC,
        NONE
    }

    //RegularShot: Normal
    //EnergyShot: Regular shot, but for shields
    //ChargeShot: Charges over course of 1.5 seconds, then fires
    //Missiles: Spawns many misiles to hit many enemies (think whistlin birds)
    //LaserShot: Long, straight beam, near instant, aoe from origin

    public enum WeaponType { Regular, Energy, Laser, Charged, Missiles, End }

    public enum EnemyType { FIGHTER, CHARGER, SWARMER, CRUISER, CARGO }

    public enum MusicTrackType { MENU, NON_COMBAT, COMBAT }

    public enum NotificationType { MAJOR, MINOR }

    public enum VideoType { VICTORY, GAME_OVER }
}
