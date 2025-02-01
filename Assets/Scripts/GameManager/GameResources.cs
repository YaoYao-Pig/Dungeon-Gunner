using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Tilemaps;
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
    public CurrentPlayerSO CurrentPlayerSO;

    public RoomNodeTypeListSO GetRoomNodeTypeListSO()
    {
        return roomNodeTypeListSO;
    }

    public List<RoomNodeTypeSO> GetRoomNodeTypeList()
    {
        return roomNodeTypeListSO.list;
    }

    public Material dimmedMaterial;
    public Material litMaterial;

    public Shader variableLitShader;

    public Shader materializeShader;
    [Header("UI")]
    public GameObject ammoIconPrefab;
    public GameObject heartPrefab;

    [Header("Audio Sounds")]
    public AudioMixerGroup soundsMasterMixerGroup;

    public SoundEffectSO doorOpenCloseSoundEffect;

    [Header("Special Tile")]
    public TileBase[] enemyUnwalkableCollisionTilesArray;

    public TileBase preferredEnemyPathTile;

    [Header("Chest")]
    public SoundEffectSO chestOpen;
    public SoundEffectSO healthPickup;
    public SoundEffectSO weaponPickup;
    public SoundEffectSO ammoPickup;

    public GameObject chestItemPrefab;
    public Sprite heartIcon;
    public Sprite bulletIcon;
}