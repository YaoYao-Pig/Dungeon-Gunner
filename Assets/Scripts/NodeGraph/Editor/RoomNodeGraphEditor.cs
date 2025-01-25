using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.Collections.Generic;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private GUIStyle selectedNodeStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeSO preRoomNode = null;
    private RoomNodeSO currentRoomNode = null;

    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    private float connectingLineWidth = 3f;

    public EventHandler OnChangedSelectedRoomNode;

    private Vector2 graphOffset;
    private Vector2 graphDrag;

    private const float gridLarge = 100f;
    private const float gridSmall = 25f;

    private void OnEnable()
    {
        Selection.selectionChanged += Selection_SelectionChanged;
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        selectedNodeStyle.normal.textColor = Color.gray;
        selectedNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        selectedNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= Selection_SelectionChanged;
    }



    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    //这个属性可以让我们在Assets当中,双击的时候触发这个回调
    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;
        if (roomNodeGraph != null)
        {
            OpenWindow();
            currentRoomNodeGraph = roomNodeGraph;
            return true;
        }
        return false;
    }


    private void OnGUI()
    {
        if (currentRoomNodeGraph != null)
        {
            DrawBackgroundGrid(gridLarge, 0.2f, Color.white);
            DrawBackgroundGrid(gridSmall, 0.1f, Color.white);
            DrawNodeConnections();
            DrawDraggingLine();
            ProcessEvents(Event.current);

            DrawRoomNodes();
        }

        if (GUI.changed)
        {
            Repaint();
        }
    }


    private void DrawBackgroundGrid(float gridSize, float lineWidth, Color color)
    {
        int verticalLineCount=Mathf.CeilToInt((position.width+gridSize)/gridSize);
        int horizontalLineCount = Mathf.CeilToInt((position.height + gridSize) / gridSize);

        float gridOpacity = .2f;
        Handles.color = new Color(color.r, color.g,color.b, gridOpacity);

        graphOffset += graphDrag * 0.5f;
        Vector3 gridOffset = new Vector3(graphOffset.x % gridSize, graphOffset.y % gridSize, 0);


        for(int i = 0; i < verticalLineCount; ++i)
        {
            Handles.DrawLine(new Vector3(gridSize * i, -gridSize, 0f) + gridOffset, new Vector3(gridSize * i, position.height + gridSize, 0f) + gridOffset);
        }

        for (int i = 0; i < horizontalLineCount; ++i)
        {
            Handles.DrawLine(new Vector3(-gridSize, gridSize * i, 0f) + gridOffset, new Vector3(position.width + gridSize,gridSize * i, 0f) + gridOffset);
        }
        Handles.color = Color.white;
    }
    private void DrawNodeConnections()
    {
        foreach (var node in currentRoomNodeGraph.roomNodeList)
        {
            foreach (var childNodeID in node.childRoomNodeIDList)
            {
                RoomNodeSO childNode = currentRoomNodeGraph.GetRoomNode(childNodeID);
                if (childNode)
                {


                    //绘制箭头
                    Vector2 dicStart2End = (childNode.rect.center - node.rect.center).normalized;
                    Vector2 middlePoint = (node.rect.center + childNode.rect.center) / 2f;
                    //垂直向量
                    float connectionArrowSize = 6f;
                    Vector2 verticalVector = new Vector2(-dicStart2End.y, dicStart2End.x).normalized;
                    Vector2 aboveArrowPoint = middlePoint + verticalVector * connectionArrowSize;
                    Vector2 belowArrowPoint = middlePoint - verticalVector * connectionArrowSize;
                    Vector2 frontArrowPoint = middlePoint + dicStart2End * connectionArrowSize;
                    Handles.DrawBezier(aboveArrowPoint, frontArrowPoint,
                        aboveArrowPoint, frontArrowPoint, Color.white, null, connectingLineWidth);

                    Handles.DrawBezier(belowArrowPoint, frontArrowPoint,
                        belowArrowPoint, frontArrowPoint, Color.white, null, connectingLineWidth);

                    Handles.DrawBezier(node.rect.center, childNode.rect.center,
                        node.rect.center, childNode.rect.center, Color.white, null, connectingLineWidth);
                    GUI.changed = true;
                }

            }
        }
    }

    private void DrawDraggingLine()
    {
        if (currentRoomNodeGraph.linePosition != Vector2.zero)
        {
            Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition,
                currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition,
                Color.white, null, connectingLineWidth);
        }
    }
    private void ProcessEvents(Event currentEvent)
    {
        graphDrag = Vector2.zero;



        preRoomNode = currentRoomNode;
        currentRoomNode = RaycastMouseOverRoomNode(currentEvent);


        if (currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {

            ProcessRoomNodeGraphEvents(currentEvent);

        }
        else
        {
            currentRoomNode.ProcessEvents(currentEvent);
        }

    }




    private RoomNodeSO RaycastMouseOverRoomNode(Event currentEvent)
    {
        foreach (var node in currentRoomNodeGraph.roomNodeList)
        {
            if (node.rect.Contains(currentEvent.mousePosition))
            {
                return node;
            }
        }
        return null;
    }


    private void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            default:
                break;
        }
    }

    private void ProcessMouseUpEvent(Event currentEvent)
    {
        if (currentEvent.button == 1)
        {
            ProcessRightMouseUpEvent(currentEvent);
        }
    }

    private void ProcessRightMouseUpEvent(Event currentEvent)
    {
        if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            if (currentRoomNode != null)
            {
                if (currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNode(currentRoomNode.id))
                {
                    currentRoomNode.AddParentRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
                }
            }
            ClearLineDrag();
        }
    }
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent);
        }
        if (currentEvent.button == 1)
        {
            ProcessRightMouseDragEvent(currentEvent);
        }
    }

    private void ProcessLeftMouseDragEvent(Event currentEvent)
    {
        graphDrag = currentEvent.delta;
        foreach(var node in currentRoomNodeGraph.roomNodeList)
        {
            node.DragNode(graphDrag);
        }
        GUI.changed = true;
    }
    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }

    private void DragConnectingLine(Vector2 delta)
    {
        currentRoomNodeGraph.linePosition += delta;
    }

    private void ProcessMouseDownEvent(Event currentEvent)
    {

        if (currentEvent.button == 0)
        {
            ClearLineDrag();
            ClearAllSelected();
        }
        //Debug.Log("ProcessMouseDownEvent");
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
    }
    private void ClearAllSelected()
    {
        foreach(var node in currentRoomNodeGraph.roomNodeList)
        {
            node.ResetSelected();
        }
    }

    #region ContextMenu
    private void ShowContextMenu(Vector2 mousePosition)
    {
        if (currentRoomNodeGraph.roomNodeList.Count == 0)
        {
            CreateEntranceRoomNode(new Vector2(20, 20));
        }
        GenericMenu menu = new GenericMenu();
        //GenericMenu.MenuFunction2 menuFunction = o => CreateRoomNode(o, roomNodeTypeList.list.Find(x => x.isNone));
        
        menu.AddItem(new GUIContent("Create Room Node"), false, o => CreateRoomNode(o, 
            GameResources.Instance.GetRoomNodeTypeList().Find(x => x.isNone)), 
            mousePosition);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Select All Room Node"), false, SelectedAllRoomNode);
        menu.AddItem(new GUIContent("UnSelect All Room Node"), false, UnSelectedAllRoomNode);

        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Selected Connection"), false, DeleteSelectedConnection);
        menu.AddItem(new GUIContent("Delete Seletected Room Node"), false, DeleteSelectedRoomNode);
        menu.ShowAsContext();
    }

    private void SelectedAllRoomNode()
    {

        foreach(var node in currentRoomNodeGraph.roomNodeList)
        {
            node.isSelected = true;
        }
        GUI.changed = true;
    }

    private void UnSelectedAllRoomNode()
    {
        foreach (var node in currentRoomNodeGraph.roomNodeList)
        {
            node.isSelected = false;
        }
        GUI.changed = true;
    }


    private void DeleteSelectedConnection()
    {
        foreach(var node in currentRoomNodeGraph.roomNodeList)
        {
            if (node.isSelected&&node.childRoomNodeIDList.Count!=0)
            {
                for(int i = node.childRoomNodeIDList.Count - 1; i >= 0; i--)
                {
                    RoomNodeSO connectedNode = currentRoomNodeGraph.GetRoomNode(node.childRoomNodeIDList[i]);
                    if (connectedNode != null && connectedNode.isSelected)
                    {
                        node.RemoveChildID(connectedNode.id);
                        connectedNode.RemoveParentID(node.id);
                    }
                }

            }
        }
        ClearAllSelected();
    }

    private void DeleteSelectedRoomNode()
    {
        Queue<RoomNodeSO> roomNodeDeletionQueue = new Queue<RoomNodeSO>();
        foreach (var node in currentRoomNodeGraph.roomNodeList)
        {
            if (node.isSelected&&!node.roomType.isEntrance)
            {
                roomNodeDeletionQueue.Enqueue(node);
                for (int i = node.parentRoomNodeIDList.Count - 1; i >= 0; i--)
                {
                    RoomNodeSO parentNode = currentRoomNodeGraph.GetRoomNode(node.parentRoomNodeIDList[i]);
                    if (parentNode != null)
                    {
                        parentNode.RemoveChildID(node.id);
                    }
                }

                for (int i = node.childRoomNodeIDList.Count - 1; i >= 0; i--)
                {
                    RoomNodeSO childeNode = currentRoomNodeGraph.GetRoomNode(node.childRoomNodeIDList[i]);
                    if (childeNode != null)
                    {
                        childeNode.RemoveParentID(node.id);
                    }
                }
            }
        }

        while (roomNodeDeletionQueue.Count > 0)
        {
            var node = roomNodeDeletionQueue.Dequeue();
            currentRoomNodeGraph.RemoveRoomNode(node.id);

            DestroyImmediate(node, true);

            AssetDatabase.SaveAssets();
        }
    }
    private void CreateRoomNode(object mousePositionObject,RoomNodeTypeSO roomNodeType)
    {

        Vector2 position = (Vector2)mousePositionObject;

        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();
        currentRoomNodeGraph.roomNodeList.Add(roomNode);
        roomNode.Initialize(new Rect(position, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph,roomNodeType);
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);
        AssetDatabase.SaveAssets();

        currentRoomNodeGraph.LoadRoomNodeDictionary();
    }

    #endregion ContextMenu

    private void CreateEntranceRoomNode(Vector2 entrancePosition)
    {
        CreateRoomNode(entrancePosition, GameResources.Instance.GetRoomNodeTypeList().Find(x => x.isEntrance));
    }

    private void DrawRoomNodes()
    {
        foreach(RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
                roomNode.Draw(selectedNodeStyle);
            roomNode.Draw(roomNodeStyle);
        }
    }

    private void ClearLineDrag()
    {
        currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }

    private void Selection_SelectionChanged()
    {
        RoomNodeGraphSO roomNodeGraphSO = Selection.activeObject as RoomNodeGraphSO;
        if (roomNodeGraphSO != null)
        {
            currentRoomNodeGraph = roomNodeGraphSO;
            GUI.changed = true;
        }
    }



}
