using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemySpawner : SingletonMonobehaviour<EnemySpawner>
{
    private int enemiesToSpawn;
    private int currentEnemyCount;
    private int enemiesSpawnedSoFar;
    private int enemyMaxConcurrentSpawnNumber;

    private Room currentRoom;
    private RoomEnemySpawnParameters roomEnemySpawnParameters;

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        enemiesSpawnedSoFar = 0;
        currentEnemyCount = 0;
        currentRoom = roomChangedEventArgs.room;

        if(currentRoom.roomNodeType.isCorridorEW|| currentRoom.roomNodeType.isCorridorNS|| currentRoom.roomNodeType.isEntrance)
        {
            return;
        }
        if (currentRoom.isClearedOfEnemies) return;

        enemiesToSpawn = currentRoom.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentDungeonLevel());

        roomEnemySpawnParameters = currentRoom.GetRoomEnemySpawnParameters(GameManager.Instance.GetCurrentDungeonLevel());

        if (enemiesToSpawn == 0)
        {
            currentRoom.isClearedOfEnemies = true;
            return;
        }
        enemyMaxConcurrentSpawnNumber = GetConcurrentEnemies();

        currentRoom.instantiatedRoom.LockDoors();
        SpawnEnemies();
    }

    private int GetConcurrentEnemies()
    {
        return (UnityEngine.Random.Range(roomEnemySpawnParameters.minConcurrentEnemies, roomEnemySpawnParameters.maxConcurrentEnemies));
    }

    private void SpawnEnemies()
    {
        if (GameManager.Instance.gameState == GameState.playingLevel)
        {
            GameManager.Instance.previousGameState = GameState.playingLevel;
            GameManager.Instance.gameState = GameState.engagingEnemies;
        }
        StartCoroutine(SpawnEnemiesRoutine());
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        Grid grid = currentRoom.instantiatedRoom.grid;
        RandomSpawnableObject<EnemyDetailsSO> randomEnemyHelperClass = new RandomSpawnableObject<EnemyDetailsSO>(currentRoom.enemiesByLevelList);

        if (currentRoom.spawnPositionArrary.Length > 0)
        {
            for(int i = 0; i < enemiesToSpawn; ++i)
            {
                //类似一个锁，持续检查currentEnemyCount是否到达同时允许的上限
                while (currentEnemyCount >= enemyMaxConcurrentSpawnNumber)
                {
                    yield return null;
                }
                Vector3Int cellPosition = (Vector3Int)currentRoom.spawnPositionArrary[UnityEngine.Random.Range(0, currentRoom.spawnPositionArrary.Length)];

                CreateEnemy(randomEnemyHelperClass.GetItem(), grid.CellToWorld(cellPosition));
                yield return new WaitForSeconds(GetEnemySpawnInterval());
            }

        }
    }



    private void CreateEnemy(EnemyDetailsSO enemyDetail, Vector3 worldPosition)
    {
        enemiesSpawnedSoFar++;
        currentEnemyCount++;

        DungeonLevelSO currentDungeonLevel = GameManager.Instance.GetCurrentDungeonLevel();

        GameObject enemy = Instantiate(enemyDetail.enemyPrefab, worldPosition, Quaternion.identity, transform);

        enemy.GetComponent<Enemy>().EnemyInitialization(enemyDetail, enemiesSpawnedSoFar, currentDungeonLevel);
        enemy.GetComponent<DestroyEvent>().OnDestroyed += EnemySpawner_OnDestroyed;
    }


    private float GetEnemySpawnInterval()
    {
        return UnityEngine.Random.Range(roomEnemySpawnParameters.minSpawnInterval, roomEnemySpawnParameters.maxSpawnInterval);
    }

    private void EnemySpawner_OnDestroyed(DestroyEvent destroyEvent,DestroyEventArgs destroyEventArgs)
    {
        destroyEvent.OnDestroyed -= EnemySpawner_OnDestroyed;
        currentEnemyCount--;

        if (currentEnemyCount <= 0 && enemiesSpawnedSoFar == enemiesToSpawn)
        {
            currentRoom.isClearedOfEnemies = true;

            if (GameManager.Instance.gameState == GameState.engagingEnemies)
            {
                GameManager.Instance.gameState = GameState.playingLevel;
                GameManager.Instance.previousGameState = GameState.engagingEnemies;
            }
            else if (GameManager.Instance.gameState == GameState.engagingBoss)
            {
                GameManager.Instance.gameState = GameState.bossStage;
                GameManager.Instance.previousGameState = GameState.engagingBoss;
            }
            currentRoom.instantiatedRoom.UnlockDoors(Settings.doorUnlockDelay);

            //当有Enemy被打败-> [触发事件]检测是否是最后一个-> [触发事件]检测是否所有普通房间都被探索完毕
            StaticEventHandler.CallRoomEnemiesDefeatedEvent(currentRoom);
        }
    }

}
