using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public string id;
    public string templateID;
    public GameObject prefab;
    public RoomNodeTypeSO roomNodeType;
    
    //绝对位置
    public Vector2Int lowerBounds;
    public Vector2Int upperBounds;
    //相对位置  
    public Vector2Int templateLowerBounds;
    public Vector2Int templateUpperBounds;
    public Vector2Int[] spawnPositionArrary;
    
    
    
    public string parentRoomID;
    public List<string> childRoomIDList;

    public List<Doorway> doorWayList;

    public bool isPositioned = false;

    public InstantiatedRoom instantiatedRoom;

    public bool isLit = false;
    public bool isClearedOfEnemies = false;
    public bool isPreviouslyVisited = false;

    public List<SpawnableObjectByLevel<EnemyDetailsSO>> enemiesByLevelList;
    public List<RoomEnemySpawnParameters> roomLevelEnemySpawnParameterList;

    public Room()
    {
        childRoomIDList = new List<string>();
        doorWayList = new List<Doorway>();
    }

    public int GetNumberOfEnemiesToSpawn(DungeonLevelSO dungeonLevel)
    {
        foreach(RoomEnemySpawnParameters roomEnemySpawnParameter in roomLevelEnemySpawnParameterList)
        {
            if(dungeonLevel== roomEnemySpawnParameter.dungeonLevel)
            {
                return UnityEngine.Random.Range(roomEnemySpawnParameter.minTotalEnemiesToSpwan, roomEnemySpawnParameter.maxTotalEnemiesToSpwan);
            }
        }
        return 0;
    }

    public RoomEnemySpawnParameters GetRoomEnemySpawnParameters(DungeonLevelSO dungeonLevel)
    {
        foreach (RoomEnemySpawnParameters roomEnemySpawnParameter in roomLevelEnemySpawnParameterList)
        {
            if (dungeonLevel == roomEnemySpawnParameter.dungeonLevel)
            {
                return roomEnemySpawnParameter;
            }
        }
        return null;
    }
}
