  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings 
{

    #region DUNGEON BULD SETTINGS
    public const int maxDungeonRebuildAttemptsForRoomGraph = 1000;
    public const int maxDungeonBuildAttempts = 10;
    #endregion DUNGEON BULD SETTINGS

    #region room_Settings
    public const int maxChildCorridors = 3;
    #endregion room_Settings

    #region ANIMATOR PARAMETERS
    public static int aimUp = Animator.StringToHash("aimUp");
    public static int aimDown = Animator.StringToHash("aimDown");
    public static int aimUpRight = Animator.StringToHash("aimUpRight");
    public static int aimUpLeft = Animator.StringToHash("aimUpLeft");
    public static int aimRight = Animator.StringToHash("aimRight");
    public static int aimLeft = Animator.StringToHash("aimLeft");
    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isMoving = Animator.StringToHash("isMoving");

    #endregion ANIMATOR PARAMETERS
}
