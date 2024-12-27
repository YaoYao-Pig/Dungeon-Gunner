using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomNodeSO : ScriptableObject
{
    public string id;
    public List<string> parentRoomNodeIDList = new List<string>();
    public List<string> childRoomNodeIDList = new List<string>();
    public RoomNodeGraphSO parentGraph;
    public RoomNodeTypeSO roomType;
#if UNITY_EDITOR
    public Rect rect;
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
#endif



}
