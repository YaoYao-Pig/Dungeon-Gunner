using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private GUIStyle selectedNodeStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeSO preRoomNode = null;
    private RoomNodeSO currentRoomNode = null;

    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding=25;
    private const int nodeBorder = 12;

    private float connectingLineWidth=3f;

    public EventHandler OnChangedSelectedRoomNode;

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



    [MenuItem("Room Node Graph Editor",menuItem ="Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    //这个属性可以让我们在Assets当中,双击的时候触发这个回调
    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID,int line)
    {
        Debug.Log("OnOpenAsset");
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



    private void DrawNodeConnections()
    {
        foreach(var node in currentRoomNodeGraph.roomNodeList)
        {
            foreach(var childNodeID in node.childRoomNodeIDList)
            {
                if(currentRoomNodeGraph.roomNodeDictionary.TryGetValue(childNodeID,out RoomNodeSO childNode))
                {


                    //绘制箭头
                    Vector2 dicStart2End = (childNode.rect.center - node.rect.center).normalized;
                    Vector2 middlePoint =  (node.rect.center + childNode.rect.center)/2f;
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
        foreach(var node in currentRoomNodeGraph.roomNodeList)
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
                currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNode(currentRoomNode.id);
                currentRoomNode.AddParentRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
            
            }
            ClearLineDrag();
        }
    }
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        if (currentEvent.button == 1)
        {
            ProcessRightMouseDragEvent(currentEvent);
        }
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
        menu.ShowAsContext();
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
