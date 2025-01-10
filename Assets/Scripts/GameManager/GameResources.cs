using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResources : MonoBehaviour
{
    private static GameResources _instance;

    public static GameResources Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<GameResources>("GameResources");
            }
            return _instance;
        }
    }

    [SerializeField] private RoomNodeTypeListSO roomNodeTypeListSO;

    public RoomNodeTypeListSO GetRoomNodeTypeListSO()
    {
        return roomNodeTypeListSO;
    }

    public List<RoomNodeTypeSO> GetRoomNodeTypeList()
    {
        return roomNodeTypeListSO.list;
    }

    public Material dimmedMaterial;
}