using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="RoomNodeType_",menuName = "Scriptable Objects/Dungeon/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{
    public string roomNodeTypeName;

    #region Header
    [Header("Only faly the RoomNodeTypes that should be visible in the editor")]
    #endregion Header
    public bool displayeInNodeGraphEditor = true;
    #region Header
    #endregion Header
    public bool isCorridor;
    #region Header
    #endregion Header
    public bool isCorridorNS;
    #region Header
    #endregion Header
    public bool isCorridorEW;
    #region Header
    #endregion Header
    public bool isEntrance;
    #region Header
    #endregion Header
    public bool isBossRoom;
    #region Header
    #endregion Header
    public bool isNone;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(roomNodeTypeName), roomNodeTypeName);
    }
#endif
    #endregion Validation
}
