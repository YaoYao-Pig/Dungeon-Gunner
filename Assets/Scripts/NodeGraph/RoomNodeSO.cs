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
        int selected = GameResources.Instance.GetRoomNodeTypeList().FindIndex(x => x == roomType);
        //ÏÂÀ­Ñ¡Ôñ
        int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());
        roomType = GameResources.Instance.GetRoomNodeTypeList()[selection];
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(this);
        }
        GUILayout.EndArea();


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

    private void ProcessLeftMouseDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;
        DragNode(currentEvent.delta);
        GUI.changed = true;
    }

    private void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }
    private void ProcessMouseUpEvent(Event currentEvent)
    {
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
#endif



}
