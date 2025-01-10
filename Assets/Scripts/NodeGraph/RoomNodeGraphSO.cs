using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="RoomNodeGraph",menuName ="Scriptable Objects/Dungeon/Room Node Graph")]
public class RoomNodeGraphSO : ScriptableObject
{
    public RoomNodeTypeListSO roomNodeTypeList;
    public List<RoomNodeSO> roomNodeList = new List<RoomNodeSO>();
    public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>();

    private void Awake()
    {
        LoadRoomNodeDictionary();
    }

    public void LoadRoomNodeDictionary()
    {
        roomNodeDictionary.Clear();
       foreach(var node in roomNodeList)
        {
            roomNodeDictionary.Add(node.id,node);
        }
    }

    


    public RoomNodeSO GetRoomNode(RoomNodeTypeSO nodeTypeSO)
    {
        foreach(RoomNodeSO node in roomNodeList)
        {
            if (node.roomType == nodeTypeSO)
            {
                return node;
            }
        }
        return null;
    }

    public RoomNodeSO GetRoomNode(string roomID)
    {
        if (roomNodeDictionary.TryGetValue(roomID, out RoomNodeSO result))
        {
            return result;
        }
        return null;
    }


    //定义函数迭代器（不是协程）
    public IEnumerable<RoomNodeSO> GetChildRoomNodes(RoomNodeSO parentRoomNode)
    {
        foreach(string childeNodeID in parentRoomNode.childRoomNodeIDList)
        {
            yield return GetRoomNode(childeNodeID);
        }
    }


#if UNITY_EDITOR




    private void OnValidate()
    {
        LoadRoomNodeDictionary();
    }
    [HideInInspector] public RoomNodeSO roomNodeToDrawLineFrom = null;
    [HideInInspector] public Vector2 linePosition;
/*    [HideInInspector] public RoomNodeSO roomNodeToDrawLineTo = null;*/

    public void SetNodeToDrawConnectionLineFrom(RoomNodeSO fromNode,Vector2 position)
    {
        roomNodeToDrawLineFrom = fromNode;
        linePosition = position;
    }




    public bool RemoveRoomNode(string nodeID)
    {
        if (roomNodeDictionary.TryGetValue(nodeID,out RoomNodeSO roomNode))
        {
            roomNodeDictionary.Remove(nodeID);
            roomNodeList.Remove(roomNode);

        }
        return false;
    }
#endif
}
