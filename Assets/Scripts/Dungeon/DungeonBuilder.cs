using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonobehaviour<DungeonBuilder>
{
    public Dictionary<string, Room> dungeonBuilderRoomDictionary = new Dictionary<string, Room>();
    private Dictionary<string, RoomTemplateSO> roomTemplateDictionary = new Dictionary<string, RoomTemplateSO>();
    private List<RoomTemplateSO> roomTemplateList = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    private bool dungeonBuildSuccessful;


    protected override void Awake()
    {
        base.Awake();

        LoadRoomNodeTypeList();


    }

    private void OnEnable()
    {
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 0f);
    }

    private void OnDisable()
    {
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
    }

    private void LoadRoomNodeTypeList()
    {
        roomNodeTypeList = GameResources.Instance.GetRoomNodeTypeListSO();
    }

    //利用LevelSO创建地牢
    public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel)
    {
        roomTemplateList = currentDungeonLevel.roomTemplateList;

        LoadRoomTemplatesIntoDictionary();


        dungeonBuildSuccessful = false;
        int dungeonBuildAttempts = 0;
        while (!dungeonBuildSuccessful && dungeonBuildAttempts < Settings.maxDungeonBuildAttempts)
        {
            dungeonBuildAttempts++;

            //进行一次尝试，随机选择一个roomNodeGraph模板 
            RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraphList);

            int dungeonRebuildAttemptsForNodeGraph = 0;


            dungeonBuildSuccessful = false;
            while (!dungeonBuildSuccessful && dungeonRebuildAttemptsForNodeGraph <= Settings.maxDungeonRebuildAttemptsForRoomGraph)
            {
                //清楚已经生成的地图
                ClearDungeon();

                dungeonRebuildAttemptsForNodeGraph++;

                dungeonBuildSuccessful = AttemptToBuildRandomDungeon(roomNodeGraph);
            }

            if (dungeonBuildSuccessful)
            {
                InstantiateRoomGameobjects();
            }
            //Debug.Log(dungeonRebuildAttemptsForNodeGraph);
        }
        return dungeonBuildSuccessful;
    }

    private void LoadRoomTemplatesIntoDictionary()
    {
        roomTemplateDictionary.Clear();
        foreach(var roomTemplate in roomTemplateList)
        {
            if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid))
            {
                roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
            }
            else
            {
                Debug.Log("Duplicate Room Template Key in " + roomTemplateList);
            }
        }
    }

    private RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphSOList)
    {
        if (roomNodeGraphSOList.Count > 0)
        {
            return roomNodeGraphSOList[UnityEngine.Random.Range(0, roomNodeGraphSOList.Count)];
        }
        else
        {
            Debug.Log("No room node graphs in list");
            return null;
        }
    }

    private void ClearDungeon()
    {
        if (dungeonBuilderRoomDictionary.Count > 0)
        {
            foreach(KeyValuePair<string,Room> keyValuePair in dungeonBuilderRoomDictionary)
            {
                Room room = keyValuePair.Value;
                if (room.instantiatedRoom != null)
                {
                    Destroy(room.instantiatedRoom.gameObject);
                }
            }
            dungeonBuilderRoomDictionary.Clear();
        }
    }

    private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
    {
        Queue<RoomNodeSO> openRoomNodeQueue = new Queue<RoomNodeSO>();
        RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));
        if(entranceNode!=null)
            openRoomNodeQueue.Enqueue(entranceNode);
        else
        {
            Debug.Log("No Entrance Node");
            return false;
        }

        bool noRoomOverlaps = true;
        noRoomOverlaps=ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue,noRoomOverlaps);

        //NodeGraph的所有节点都被处理了并且没有遮挡
        if (openRoomNodeQueue.Count == 0 && noRoomOverlaps)
        {
            return true;
        }

        return false;
    }

    private bool ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph,Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps)
    {
        //其实就是个树的层次遍历
        while (openRoomNodeQueue.Count > 0 && noRoomOverlaps==true)
        {
            RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();

            foreach(RoomNodeSO childNode in roomNodeGraph.GetChildRoomNodes(roomNode))
            {
                openRoomNodeQueue.Enqueue(childNode);
            }

            if (roomNode.roomType.isEntrance)
            {
                RoomTemplateSO roomTemplate = GetRandomRoomTempalte(roomNode.roomType);

                Room room = CreateRoomFromTemplate(roomTemplate, roomNode);

                room.isPositioned = true;

                dungeonBuilderRoomDictionary.Add(room.id, room);
            }
            else
            {
                Room parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];
                noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);
            }
        }

        return noRoomOverlaps;
    }

    //roomNode 是将要插入的新的roomNode
    /// <summary>
    /// 1. 根据父节点的类型和方向确定子节点的模板
    /// 2. 用模板对应创建Room类
    /// 3. PlaceTheRoom看是否能成功放置
    /// </summary>
    /// <param name="roomNode"></param>
    /// <param name="parentRoom"></param>
    /// <returns></returns>
    private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom)
    {
        bool roomOverlaps = true;

        while (roomOverlaps)
        {
            List<Doorway> unconnectedAvailableParentDoorways = GetUnconnectedAvailableDoorways(parentRoom.doorWayList).ToList();

            if (unconnectedAvailableParentDoorways.Count == 0)
            {
                return false;
            }

            Doorway doorwayParent = unconnectedAvailableParentDoorways[Random.Range(0, unconnectedAvailableParentDoorways.Count)];
            
            //随机选择了一个南北的doorway，那么就要匹配一个南北走向的走廊
            RoomTemplateSO roomTemplate = GetRandomRoomTempalteConsistentWithParent(roomNode, doorwayParent);

            Room room = CreateRoomFromTemplate(roomTemplate, roomNode);

            if (PlaceTheRoom(parentRoom, doorwayParent, room))
            {
                roomOverlaps = false;

                room.isPositioned = true;

                dungeonBuilderRoomDictionary.Add(room.id, room);
            }
            else
            {
                roomOverlaps = true;
            }
        }

        return true;
    }

    private bool PlaceTheRoom(Room parentRoom, Doorway doorwayParent, Room room)
    {
        //门洞对齐
        Doorway doorway = GetOppositeDoorway(doorwayParent, room.doorWayList);

        if (doorway == null)
        {
            doorwayParent.isUnavailable = true;
            return false;
        }

        //parentDoorway的绝对位置
        Vector2Int parentDoorwayposition = parentRoom.lowerBounds + doorwayParent.position - parentRoom.templateLowerBounds;

        Vector2Int adjustment = Vector2Int.zero;

        switch (doorway.orientation)
        {
            case Orientation.north:
                adjustment = new Vector2Int(0, -1);
                break;
            case Orientation.east:
                adjustment = new Vector2Int(-1, 0);
                break;
            case Orientation.south:
                adjustment = new Vector2Int(0, 1);
                break;
            case Orientation.west:
                adjustment = new Vector2Int(1, 0);
                break;
            case Orientation.none:
                break;
            default:
                break;
        }

        room.lowerBounds = parentDoorwayposition + adjustment + room.templateLowerBounds - doorway.position;
        room.upperBounds = room.lowerBounds + room.templateUpperBounds - room.templateLowerBounds;

        //check重叠
        Room overlappingRoom = CheckForRoomOverlap(room);

        if (overlappingRoom == null)
        {
            doorwayParent.isConnected = true;
            doorwayParent.isUnavailable = true;

            doorway.isConnected = true;
            doorway.isUnavailable = true;
            return true;
        }
        doorwayParent.isUnavailable = true;
        return false;
    }

    private Room CheckForRoomOverlap(Room roomToTest)
    {
        foreach(var kv in dungeonBuilderRoomDictionary)
        {
            Room room = kv.Value;
            if(room.id== roomToTest.id|| !room.isPositioned)
            {
                continue;
            }

            if (IsOverLappingRoom(roomToTest, room))
            {
                return room;
            }
        }
        return null;
    }

    private bool IsOverLappingRoom(Room roomToTest, Room room)
    {
        bool isOverlappingX= IsOverLappingInterval(roomToTest.lowerBounds.x, roomToTest.upperBounds.x, room.lowerBounds.x, room.upperBounds.x);
        bool isOverlappingY = IsOverLappingInterval(roomToTest.lowerBounds.y, roomToTest.upperBounds.y, room.lowerBounds.y, room.upperBounds.y);
        if (isOverlappingX && isOverlappingY)
        {
            return true;
        }
        return false;
    }

    private bool IsOverLappingInterval(int imin1,int imax1,int imin2,int imax2)
    {
        if (Mathf.Max(imin1, imin2) <= Mathf.Min(imax1, imax2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private Doorway GetOppositeDoorway(Doorway doorwayParent, List<Doorway> doorWayList)
    {
        foreach(Doorway doorway in doorWayList)
        {
            if (doorwayParent.orientation == Orientation.east && doorway.orientation == Orientation.west)
            {
                return doorway;
            }
            else if (doorwayParent.orientation == Orientation.west && doorway.orientation == Orientation.east)
            {
                return doorway;
            }

            else if (doorwayParent.orientation == Orientation.north && doorway.orientation == Orientation.south)
            {
                return doorway;
            }

            else if (doorwayParent.orientation == Orientation.south && doorway.orientation == Orientation.north)
            {
                return doorway;
            }

        }
        return null;
    }

    private RoomTemplateSO GetRandomRoomTempalteConsistentWithParent(RoomNodeSO roomNode, Doorway doorwayParent)
    {
        RoomTemplateSO roomTemplate = null;
        if (roomNode.roomType.isCorridor)
        {
            switch (doorwayParent.orientation)
            {
                case Orientation.north:
                case Orientation.south:
                    roomTemplate = GetRandomRoomTempalte(roomNodeTypeList.list.Find(x => x.isCorridorNS));
                    break;
                case Orientation.east:
                case Orientation.west:
                    roomTemplate = GetRandomRoomTempalte(roomNodeTypeList.list.Find(x => x.isCorridorEW));
                    break;
                case Orientation.none:
                    break;
                default:
                    break;
            }
        }
        else
        {
            roomTemplate = GetRandomRoomTempalte(roomNode.roomType);
        }
        return roomTemplate;
    }

    private IEnumerable<Doorway> GetUnconnectedAvailableDoorways(List<Doorway> doorWayList)
    {
        foreach(Doorway doorway in doorWayList)
        {
            if (!doorway.isConnected && !doorway.isUnavailable)
                yield return doorway;
        }
    }



    //实例化
    private void InstantiateRoomGameobjects()
    {
        foreach(var kv in dungeonBuilderRoomDictionary)
        {
            Room room = kv.Value;

            Vector3 roomPosition = new Vector3(room.lowerBounds.x - room.templateLowerBounds.x, room.lowerBounds.y - room.templateLowerBounds.y, 0f);

            GameObject roomGameobject = Instantiate(room.prefab, roomPosition, Quaternion.identity, transform);

            InstantiatedRoom instantiatedRoom = roomGameobject.GetComponentInChildren<InstantiatedRoom>();

            instantiatedRoom.room = room;
            instantiatedRoom.Initialise(roomGameobject);

            room.instantiatedRoom = instantiatedRoom;
        }
    }

    private RoomTemplateSO GetRandomRoomTempalte(RoomNodeTypeSO nodeType)
    {
        List<RoomTemplateSO> matchingRoomTemplateList =new List<RoomTemplateSO>();

        foreach(RoomTemplateSO roomTemplate in roomTemplateList)
        {
            if (roomTemplate.roomNodeType == nodeType)
            {
                matchingRoomTemplateList.Add(roomTemplate);
            }
        }

        if (matchingRoomTemplateList.Count == 0)
        {
            return null;
        }

        return matchingRoomTemplateList[Random.Range(0, matchingRoomTemplateList.Count)];
    }

    private Room CreateRoomFromTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
    {

        Room room = new Room();
        room.templateID = roomTemplate.guid;
        room.id = roomNode.id;
        room.prefab = roomTemplate.prefab;
        room.roomNodeType = roomTemplate.roomNodeType;
        room.spawnPositionArrary = roomTemplate.spawnPositionArray;
        room.lowerBounds = roomTemplate.lowerBounds;
        room.upperBounds = roomTemplate.upperBounds;
        room.templateLowerBounds = roomTemplate.lowerBounds;
        room.templateUpperBounds = roomTemplate.upperBounds;


        room.childRoomIDList = CoryStringList(roomNode.childRoomNodeIDList);

        room.doorWayList = CopyDoorwayList(roomTemplate.doorwayList);

        //is Entrance
        if (roomNode.parentRoomNodeIDList.Count == 0)
        {
            room.parentRoomID = "";
            room.isPreviouslyVisited = true;
            GameManager.Instance.SetCurrentRoom(room);
        }
        else
        {
            room.parentRoomID = roomNode.parentRoomNodeIDList[0];
        }

        return room;

    }

    private List<Doorway> CopyDoorwayList(List<Doorway> oldDoorwayList)
    {
        List<Doorway> newDoorwayList = new List<Doorway>();
        foreach(Doorway doorway in oldDoorwayList)
        {
            Doorway newDoorway = new Doorway();
            newDoorway.position = doorway.position;
            newDoorway.orientation = doorway.orientation;
            newDoorway.doorPrefab = doorway.doorPrefab;
            newDoorway.isConnected = doorway.isConnected;
            newDoorway.isUnavailable = doorway.isUnavailable;
            newDoorway.doorwayStartCopyPosition = doorway.doorwayStartCopyPosition;
            newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;
            newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;
            newDoorwayList.Add(newDoorway);
        }
        return newDoorwayList;
    }

    private List<string> CoryStringList(List<string> oldStringList)
    {
        List<string> newStringList = new List<string>();
        foreach(string stringValue in oldStringList)
        {
            newStringList.Add(stringValue);
        }
        return newStringList;

    }

    public RoomTemplateSO GetRoomTemplate(string roomTemplateID)
    {
        if(roomTemplateDictionary.TryGetValue(roomTemplateID,out RoomTemplateSO roomTemplate))
        {
            return roomTemplate;
        }
        return null;
    }


    public Room GetRoomByRoomID(string roomID)
    {
        if (dungeonBuilderRoomDictionary.TryGetValue(roomID, out Room room))
        {
            return room;
        }
        return null;
    }
}
