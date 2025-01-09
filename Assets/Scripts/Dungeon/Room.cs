using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    private string _id;
    private string templateID;
    [SerializeField] private GameObject prefab;
    private RoomNodeTypeSO roomNodeTypeSO;
    
    private Vector2Int lowerBounds;
    private Vector2Int upperBounds;
    private Vector2Int templateLowerBounds;
    private Vector2Int templateUpperBounds;
    private Vector2Int[] spawnPositionArrary;
    
    
    
    private string parentRoomID;
    private List<string> childRoomIDList;

    private List<Doorway> doorWayList;

    private bool isPositioned = false;

    private InstantiatedRoom instantiatedRoom;

    public bool isLit = false;
    private bool isClearedOfEnemies = false;
    private bool isPreviouslyVisited = false;

    public Room()
    {
        childRoomIDList = new List<string>();
        doorWayList = new List<Doorway>();
    }
}
