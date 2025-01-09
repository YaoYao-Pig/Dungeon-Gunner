using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject
{
    [Space(10)]
    [Header("BASIC LEVEL DETAILS")]
    [Tooltip("The name of level")]
    public string levelName;

    [Space(10)]
    [Header("ROOM TEMPLATES FOR LEVEL")]
    public List<RoomTemplateSO> roomTemplateList;

    [Space(10)]
    [Header("ROOM NODE GRAPHS FOR LEVEL")]
    public List<RoomNodeGraphSO> roomNodeGraphList;


    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName);
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomTemplateList), roomTemplateList))
        {
            return;
        }

        bool isEWCorridor = false;
        bool isNSCorridor = false;
        bool isEntrance = false;

        foreach(RoomTemplateSO roomTemplateSO in roomTemplateList)
        {
            if (roomTemplateSO == null)
            {
                return;
            }
            if (roomTemplateSO.roomNodeType.isCorridorEW)
            {
                isEWCorridor = true;
            }
            if (roomTemplateSO.roomNodeType.isCorridorNS)
            {
                isNSCorridor = true;
            }

            if (roomTemplateSO.roomNodeType.isEntrance)
            {
                isEntrance = true;
            }
        }

        if (isEWCorridor == false)
        {
            Debug.Log("In " + this.name.ToString() + " : No E/W Corridor Room Type Specified");
        }
        if (isNSCorridor == false)
        {
            Debug.Log("In " + this.name.ToString() + " : No N/S Corridor Room Type Specified");
        }
        if (isEntrance == false)
        {
            Debug.Log("In " + this.name.ToString() + " : No Entrance Corridor Room Type Specified");
        }

        foreach(RoomNodeGraphSO roomNodeGraph in roomNodeGraphList)
        {
            if (roomNodeGraph == null) return;

            foreach (RoomNodeSO roomNodeSO in roomNodeGraph.roomNodeList)
            {
                if (roomNodeSO == null) continue;

                if (roomNodeSO.roomType.isEntrance || roomNodeSO.roomType.isCorridor || roomNodeSO.roomType.isCorridorEW || roomNodeSO.roomType.isCorridorNS ||
                    roomNodeSO.roomType.isNone)
                    continue;
                bool isRoomNodeTypeFound = false;
                foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
                {
                    if (roomTemplateSO == null)
                        continue;
                    if (roomTemplateSO.roomNodeType == roomNodeSO.roomType)
                    {
                        isRoomNodeTypeFound = true;
                        break;
                    }
                }

                if (isRoomNodeTypeFound == false)
                {
                    Debug.Log("In " + this.name.ToString() + " : No room template " + roomNodeSO.roomType.name.ToString() + " found for node graph "
                        + roomNodeGraph.name.ToString());
                }

            }
        }

    }



#endif
    #endregion Validation



}
