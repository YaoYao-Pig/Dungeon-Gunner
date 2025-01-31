using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings 
{

    #region UNITS
    public const float pixelsPerUnit = 16f;
    public const float tileSizePixels = 16f;
    #endregion UNITS

    #region DUNGEON BULD SETTINGS
    public const int maxDungeonRebuildAttemptsForRoomGraph = 1000;
    public const int maxDungeonBuildAttempts = 10;
    #endregion DUNGEON BULD SETTINGS

    #region ROOM_Settings
    public const float fadeInTime = 0.5f;
    public const int maxChildCorridors = 3;
    public const int doorUnlockDelay = 1;
    #endregion ROOM_Settings

    #region ANIMATOR PARAMETERS
    public static int aimUp = Animator.StringToHash("aimUp");
    public static int aimDown = Animator.StringToHash("aimDown");
    public static int aimUpRight = Animator.StringToHash("aimUpRight");
    public static int aimUpLeft = Animator.StringToHash("aimUpLeft");
    public static int aimRight = Animator.StringToHash("aimRight");
    public static int aimLeft = Animator.StringToHash("aimLeft");
    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isMoving = Animator.StringToHash("isMoving");

    public static int rollUp = Animator.StringToHash("rollUp");
    public static int rollRight = Animator.StringToHash("rollRight");
    public static int rollLeft = Animator.StringToHash("rollLeft");
    public static int rollDown = Animator.StringToHash("rollDown");
    public static float baseSpeedForPlayerAnimations = 8f;
    public static float baseSpeedForEnemyAnimations = 3f;

    public static int open = Animator.StringToHash("open");

    public static String stateDestroyed = "Destroyed";
    public static int destroy = Animator.StringToHash("destroy");
    #endregion ANIMATOR PARAMETERS


    #region GAMEOBJECT TAGS
    public const string playerTag = "Player";
    public const string playerWeapon = "playerWeapon";
    #endregion


    #region FIRING CONTROL
    public const float useAimAngleDistance = 3.5f;

    #endregion

    #region UI PARAMETERS
    public const float uiAmmoIconSpacing = 4f;
    public const float uiHeartSpacing = 16f;

    #endregion

    #region Enemy PARAMETERS
    public const int defaultEnemyHealth = 20;
    #endregion

    #region PathFinding
    public const int defaultAStarMovementPenalty = 40;
    public const int preferredAStarMovementPenalty = 20;
    public const int targetFrameRateToSpreadPathFindingOver = 60;
    public const float playerMoveDistanceToRebuildPath = 3f;
    public const float enemyPathRebuildCooldown = 2f;

    #endregion

    #region Contact Damage
    public const float contactDamageCollisionResetDelay = 0.5f;
    #endregion
}
