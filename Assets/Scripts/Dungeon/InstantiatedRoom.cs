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


}
