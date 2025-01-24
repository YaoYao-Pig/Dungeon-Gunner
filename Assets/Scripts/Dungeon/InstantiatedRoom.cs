using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
    [HideInInspector] public Room room;
    [HideInInspector] public Grid grid;
    [HideInInspector] public Tilemap groundTilemap;
    [HideInInspector] public Tilemap decoration1Tilemap;
    [HideInInspector] public Tilemap decoration2Tilemap;
    [HideInInspector] public Tilemap frontTilemap;
    [HideInInspector] public Tilemap collisionTilemap;
    [HideInInspector] public Tilemap minimapTilemap;
    [HideInInspector] public Bounds roomColliderBounds;

    [HideInInspector] public int[,] aStarMovementPenalty;

    private BoxCollider2D boxCollider2D;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        roomColliderBounds = boxCollider2D.bounds;
        

    }

    public void Initialise(GameObject roomGameObjectPrefab)
    {
        PopulateTilemapMemberVariables(roomGameObjectPrefab);
        BlockOffUnuseDoorWays();

        AddObstaclesAndPreferredParts();

        AddDoorsToRooms();

        DisableCollisionTilemapRenderer();
    }



    private void BlockOffUnuseDoorWays()
    {
        foreach(Doorway doorway in room.doorWayList)
        {
            if (doorway.isConnected) continue;
            if(collisionTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(collisionTilemap, doorway);
            }

            if (minimapTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(minimapTilemap, doorway);
            }
            if (groundTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(groundTilemap, doorway);
            }
            if (decoration1Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(decoration1Tilemap, doorway);
            }
            if (decoration2Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(decoration2Tilemap, doorway);
            }
            if (frontTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(frontTilemap, doorway);
            }

        }
    }


    private void BlockADoorwayOnTilemapLayer(Tilemap tilemap,Doorway doorway)
    {
        switch (doorway.orientation)
        {
            case Orientation.north:
            case Orientation.south:
                BlockDoorwayHorizontally(tilemap, doorway);
                break;
            case Orientation.east:
            case Orientation.west:
                BlockDoorwayVertically(tilemap, doorway);
                break;
            case Orientation.none:
                break;
        }
    }



    private void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;
        for(int x = 0; x < doorway.doorwayCopyTileWidth; ++x)
        {
            for(int y = 0; y < doorway.doorwayCopyTileHeight; ++y)
            {
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + x, startPosition.y - y, 0));
                tilemap.SetTile(new Vector3Int(startPosition.x + 1 + x, startPosition.y - y, 0), tilemap.GetTile(new Vector3Int(startPosition.x + x, startPosition.y - y, 0)));
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + 1 + x, startPosition.y - y, 0), transformMatrix);
            }
        }
    }

    private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;
        for (int y = 0; y < doorway.doorwayCopyTileHeight; ++y)
        {
            for (int x = 0; x < doorway.doorwayCopyTileWidth; ++x)
            {
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + x, startPosition.y - y, 0));
                tilemap.SetTile(new Vector3Int(startPosition.x + x, startPosition.y - 1 - y, 0), tilemap.GetTile(new Vector3Int(startPosition.x + x, startPosition.y - y, 0)));
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x  + x, startPosition.y - 1 - y, 0), transformMatrix);
            }
        }
    }
    private void PopulateTilemapMemberVariables(GameObject roomGameObjectPrefab)
    {
        grid = roomGameObjectPrefab.GetComponentInChildren<Grid>();
        Tilemap[] tilemaps = roomGameObjectPrefab.GetComponentsInChildren<Tilemap>();
        foreach(Tilemap tilemap in tilemaps)
        {
            
            switch (tilemap.gameObject.tag)
            {
                case "groundTilemap":
                    groundTilemap = tilemap;
                    break;
                case "decoration1Tilemap":
                    decoration1Tilemap = tilemap;
                    break;
                case "decoration2Tilemap":
                    decoration2Tilemap = tilemap;
                    break;
                case "frontTilemap":
                    frontTilemap = tilemap;
                    break;
                case "collisionTilemap":
                    collisionTilemap = tilemap;
                    break;
                case "minimapTilemap":
                    minimapTilemap = tilemap;
                    break;
                default:
                    break;
            }
        }
    }
    private void DisableCollisionTilemapRenderer()
    {
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }


    private void AddObstaclesAndPreferredParts()
    {
        aStarMovementPenalty = new int[room.templateUpperBounds.x - room.templateLowerBounds.x + 1,
                                    room.templateUpperBounds.y - room.templateLowerBounds.y + 1];
        for(int x=0;x<(room.templateUpperBounds.x - room.templateLowerBounds.x + 1); x++)
        {
            for(int y=0;y<(room.templateUpperBounds.y - room.templateLowerBounds.y + 1); ++y)
            {
                aStarMovementPenalty[x, y] = Settings.defaultAStarMovementPenalty;

                TileBase tile = collisionTilemap.GetTile(new Vector3Int(x + room.templateLowerBounds.x, y + room.templateLowerBounds.y, 0));

                foreach(TileBase collisionTile in GameResources.Instance.enemyUnwalkableCollisionTilesArray)
                {
                    if (tile == collisionTile)
                    {
                        aStarMovementPenalty[x, y] = 0;
                        break;
                    }
                }

                if (tile == GameResources.Instance.preferredEnemyPathTile)
                {
                    aStarMovementPenalty[x, y] = Settings.preferredAStarMovementPenalty;
                }
            }
        }
    }

    private void AddDoorsToRooms()
    {
        if (room.roomNodeType.isCorridorEW==true || room.roomNodeType.isCorridorNS==true) return;

        foreach(Doorway doorway in room.doorWayList)
        {
            if (doorway.doorPrefab!=null&&doorway.isConnected)
            {
                float tileDistance = Settings.tileSizePixels / Settings.pixelsPerUnit;
                GameObject door = null;

                if (doorway.orientation == Orientation.north)
                {
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f, doorway.position.y + tileDistance, 0f);
                }
                else if (doorway.orientation == Orientation.south)
                {
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f, doorway.position.y, 0f);
                }
                else if (doorway.orientation == Orientation.east)
                {
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance, doorway.position.y + tileDistance*1.25f, 0f);
                }
                else if (doorway.orientation == Orientation.west)
                {
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x, doorway.position.y + tileDistance, 0f);
                }

                Door doorComponent = door.GetComponent<Door>();
                if (room.roomNodeType.isBossRoom)
                {
                    doorComponent.isBossRoomDoor = true;
                    doorComponent.LockDoor();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == Settings.playerTag&&room != GameManager.Instance.GetCurrentRoom())
        {
            this.room.isPreviouslyVisited = true;
            StaticEventHandler.CallRoomChangedEvent(room);
        }
        
    }

}
