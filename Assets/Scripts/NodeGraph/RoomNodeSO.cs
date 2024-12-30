using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class RoomNodeSO : ScriptableObject
{
    public string id;
    public List<string> parentRoomNodeIDList = new List<string>();
    public List<string> childRoomNodeIDList = new List<string>();
    public RoomNodeGraphSO parentGraph;
    public RoomNodeTypeSO roomType;
#if UNITY_EDITOR
    public Rect rect;
    public bool isLeftClickDragging = false;
    public bool isSelected = false;
    public void Initialize(Rect rect,RoomNodeGraphSO parentGraph,RoomNodeTypeSO roomType)
    {
        this.rect = rect;
        this.id = GUID.Generate().ToString();
        this.parentGraph = parentGraph;
        this.roomType = roomType;
        this.name = "Room Node";

    }

    public void Draw(GUIStyle nodeStyle)
    {
        GUILayout.BeginArea(rect, nodeStyle);
        EditorGUI.BeginChangeCheck();

        if (parentRoomNodeIDList.Count > 0 || roomType.isEntrance)
        {
            EditorGUILayout.LabelField(roomType.roomNodeTypeName);
        }
        else
        {
            int selected = GameResources.Instance.GetRoomNodeTypeList().FindIndex(x => x == roomType);
            //下拉选择
            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());
            roomType = GameResources.Instance.GetRoomNodeTypeList()[selection];


            if(!CheckSelectionChangeIsValid(GameResources.Instance.GetRoomNodeTypeList()[selected], GameResources.Instance.GetRoomNodeTypeList()[selection]))
            {

                if (childRoomNodeIDList.Count != 0)
                {
                    for (int i = childRoomNodeIDList.Count - 1; i >= 0; i--)
                    {
                        RoomNodeSO connectedNode = parentGraph.GetRoomNode(childRoomNodeIDList[i]);
                        if (connectedNode != null)
                        {
                            RemoveChildID(connectedNode.id);
                            
                            connectedNode.RemoveParentID(id);
                        }
                    }

                }
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(this);
        }
        GUILayout.EndArea();


    }

    private bool CheckSelectionChangeIsValid(RoomNodeTypeSO pre,RoomNodeTypeSO last)
    {
        if (pre.isCorridor && !last.isCorridor)
        {
            return false;
        }
        if (!pre.isCorridor && last.isCorridor)
        {
            return false;
        }
        if (!pre.isBossRoom && last.isBossRoom)
        {
            return false;
        }
        return true;
    }

    private string[] GetRoomNodeTypesToDisplay()
    {
        List<string> result = new List<string>();
        List<RoomNodeTypeSO> roomNodeTypeSOList = GameResources.Instance.GetRoomNodeTypeList();
        for(int i = 0; i < roomNodeTypeSOList.Count; ++i)
        {
            result.Add(roomNodeTypeSOList[i].roomNodeTypeName);
        }
        return result.ToArray();
    }

    public void ProcessEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
        }
    }

    private void ProcessMouseDragEvent(Event currentEvent)
    {
        if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent); 
        }

    }

    private void ProcessRightMouseDownEvent(Event currentEvent)
    {
        Debug.Log("Right on Room");
        parentGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
    }

    private void ProcessLeftMouseDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;
        DragNode(currentEvent.delta);
        GUI.changed = true;
    }

    public void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        Debug.Log("up");
        if (currentEvent.button == 0)
        {
            ProcessLeftMouseUpEvent(currentEvent);
        }
    }

    private void ProcessLeftMouseUpEvent(Event currentEvent)
    {
        if (isLeftClickDragging)
        {
            isLeftClickDragging = false;
        }
    }

    private void ProcessMouseDownEvent(Event currentEvent)
    {
        //left mouse button
        if (currentEvent.button == 0)
        {
            ProcessLeftMouseDownEvent(currentEvent);
        }
        else if (currentEvent.button == 1)
        {
            ProcessRightMouseDownEvent(currentEvent);
        }
    }


    private void ProcessLeftMouseDownEvent(Event currentEvent)
    {
        Selection.activeObject = this;
        isSelected = !isSelected;
    }
    
    public void ResetSelected()
    {
        if (isSelected == true)
        {
            isSelected = false;
        }
    }




    public bool AddChildRoomNode(string childID)
    {
        if (isChildRoomAddValid(childID))
        {
            childRoomNodeIDList.Add(childID);
            return true;
        }
        return false;
    }

    private bool isChildRoomAddValid(string childID)
    {
        RoomNodeSO childNode = parentGraph.GetRoomNode(childID);

        if (childNode == null) return false;

        //Boos Room;
        bool isConnectedBossodeAlready = false;
        if (childNode.roomType.isBossRoom)
        {
            foreach(RoomNodeSO roomNode in parentGraph.roomNodeList)
            {
                if (roomNode.roomType.isBossRoom && roomNode.parentRoomNodeIDList.Count > 0)
                {
                    isConnectedBossodeAlready = true;
                }
            }
        }
        if (childNode.roomType.isBossRoom && isConnectedBossodeAlready)
        {
            return false;
        }

        if (childNode.roomType.isNone)
        {
            return false;

        }

        //父子环
        if (parentRoomNodeIDList.Contains(childID))
        {
            return false;
        }
        //自环
        if (id == childID)
        {
            return false;
        }
        //不重复包含
        if (childRoomNodeIDList.Contains(childID))
        {
            return false;
        }




        if (childNode.parentRoomNodeIDList.Count ==1)
        {
            return false;
        }

        //不允许两个走廊相连
        if (childNode.roomType.isCorridor && roomType.isCorridor)
        {
            return false;
        }
        //不允许两个房间相连
        if (!childNode.roomType.isCorridor && !roomType.isCorridor)
        {
            return false;
        }

        //链接最大值
        if (childNode.roomType.isCorridor && childRoomNodeIDList.Count >= Settings.maxChildCorridors)
        {
            return false;
        }

        if (childNode.roomType.isEntrance)
        {
            return false;
        }

        if (!childNode.roomType.isCorridor && childRoomNodeIDList.Count > 0)
        {
            return false;
        }



        return true;
    }

    public bool AddParentRoomNode(string parentID)
    {
        parentRoomNodeIDList.Add(parentID);
        return true;
    }

    public bool RemoveChildID(string childID)
    {
        if (childRoomNodeIDList.Contains(childID))
        {
            childRoomNodeIDList.Remove(childID);
            return true;
        }
        return false;
    }

    public bool RemoveParentID(string parentID)
    {
        if (parentRoomNodeIDList.Contains(parentID))
        {
            parentRoomNodeIDList.Remove(parentID);
            return true;
        }
        return false;
    }
#endif



}
