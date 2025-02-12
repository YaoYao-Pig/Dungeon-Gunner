using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomEnemySpawnParameters
{
    public DungeonLevelSO dungeonLevel;
    public int minTotalEnemiesToSpwan;
    public int maxTotalEnemiesToSpwan;

    public int minConcurrentEnemies;
    public int maxConcurrentEnemies;

    public int minSpawnInterval;
    public int maxSpawnInterval;

}
