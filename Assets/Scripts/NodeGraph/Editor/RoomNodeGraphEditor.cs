using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeSO preRoomNode = null;
    private RoomNodeSO currentRoomNode = null;

    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding=25;
    private const int nodeBorder = 12;

    public EventHandler OnChangedSelectedRoomNode;

    private void OnEnable()
    {
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
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
            ProcessEvents(Event.current);

            DrawRoomNodes();
        }

        if (GUI.changed)
        {
            Repaint();
        }
    }

    private void ProcessEvents(Event currentEvent)
    {
        preRoomNode = currentRoomNode;
        currentRoomNode = RaycastMouseOverRoomNode(currentEvent);




        ProcessRoomNodeGraphEvents(currentEvent);
        if (currentRoomNode != null)
        {
/*            if (preRoomNode!=null&&preRoomNode != currentRoomNode)
            {
                preRoomNode.ResetSelected();
            }*/
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
        if (currentRoomNode != null) return;
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            default:
                break;
        }
    }




    private void ProcessMouseDownEvent(Event currentEvent)
    {
        //Debug.Log("ProcessMouseDownEvent");
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
    }

    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();
        //GenericMenu.MenuFunction2 menuFunction = o => CreateRoomNode(o, roomNodeTypeList.list.Find(x => x.isNone));
        
        menu.AddItem(new GUIContent("Create Room Node"), false, o => CreateRoomNode(o, 
            GameResources.Instance.GetRoomNodeTypeList().Find(x => x.isNone)), 
            mousePosition);
        menu.ShowAsContext();
    }


    private void CreateRoomNode(object mousePositionObject,RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();
        currentRoomNodeGraph.roomNodeList.Add(roomNode);
        roomNode.Initialize(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph,roomNodeType);
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);
        AssetDatabase.SaveAssets();
    }

    private void DrawRoomNodes()
    {
        foreach(RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.Draw(roomNodeStyle);
        }
    }

}
