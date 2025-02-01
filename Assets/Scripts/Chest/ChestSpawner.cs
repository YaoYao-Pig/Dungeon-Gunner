using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSpawner : MonoBehaviour
{
    [System.Serializable]
    private struct RangeByLevel
    {
        public DungeonLevelSO dungeonLevel;
        [Range(0, 100)] public int min;
        [Range(0, 100)] public int max;
    }
    #region Header CHEST PREFAB
    [Space(10)]
    [Header("CHEST PREFAB")]
    #endregion

    #region Tooltip
    [Tooltip("Populate with the chest prefab")]
    #endregion

    [SerializeField] private GameObject chestPrefab;
    #region Header CHEST SPAWN CHANCE
    [Space(10)]
    [Header("CHEST SPAWN CHANCE")]
    #endregion

    #region Tooltip
    [Tooltip("The minimum probability for spawning a chest")]
    #endregion
    [SerializeField] [Range(0, 100)] private int chestSpawnChanceMin;

    #region Tooltip
    [Tooltip("The maximum probability for spawning a chest")]
    #endregion
    [SerializeField] [Range(0, 100)] private int chestSpawnChanceMax;
    #region Tooltip
    [Tooltip("You can override the chest spawn chance by dungeon level")]
    #endregion
    [SerializeField] private List<RangeByLevel> chestSpawnChanceByLevelList;
    #region Header CHEST SPAWN DETAILS
    [Space(10)]
    [Header("CHEST SPAWN DETAILS")]
    #endregion

    [SerializeField] private ChestSpawnEvent chestSpawnEvent;
    [SerializeField] private ChestSpawnPosition chestSpawnPosition;
    #region Tooltip
    [Tooltip("The minimum number of items to spawn (note that a maximum of 1 of each type of ammo, health, and weapon will be spawned)")]
    #endregion
    [SerializeField] [Range(0, 3)] private int numberOfItemsToSpawnMin;

    #region Tooltip
    [Tooltip("The maximum number of items to spawn (note that a maximum of 1 of each type of ammo, health, and weapon will be spawned)")]
    #endregion
    [SerializeField] [Range(0, 3)] private int numberOfItemsToSpawnMax;
    #region Header CHEST CONTENT DETAILS
    [Space(10)]
    [Header("CHEST CONTENT DETAILS")]
    #endregion

    #region Tooltip
    [Tooltip("The weapons to spawn for each dungeon level and their spawn ratios")]
    #endregion
    [SerializeField] private List<SpawnableObjectByLevel<WeaponDetailsSO>> weaponSpawnByLevelList;
    #region Tooltip
    [Tooltip("The range of health to spawn for each level")]
    #endregion
    [SerializeField] private List<RangeByLevel> healthSpawnByLevelList;
    [SerializeField] private List<RangeByLevel> ammoSpawnByLevelList;
    private bool chestSpawned = false;
    private Room chestRoom;
    private void OnEnable()
    {
        // Subscribe to room changed event
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;

        // Subscribe to room enemies defeated event
        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
    }
    private void OnDisable()
    {
        // Unsubscribe from room changed event
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;

        // Unsubscribe from room enemies defeated event
        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
    }
    /// <summary>
    /// Handle room changed event
    /// </summary>
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        // Get the room the chest is in if we don't already have it
        if (chestRoom == null)
        {
            chestRoom = GetComponentInParent<InstantiatedRoom>().room;
        }

        // If the chest is spawned on room entry then spawn chest
        if (!chestSpawned && chestSpawnEvent == ChestSpawnEvent.onRoomEntry && chestRoom == roomChangedEventArgs.room)
        {
            SpawnChest();
        }
    }
    /// <summary>
    /// Handle room enemies defeated event
    /// </summary>
    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs)
    {
        // Get the room the chest is in if we don't already have it
        if (chestRoom == null)
        {
            chestRoom = GetComponentInParent<InstantiatedRoom>().room;
        }

        // If the chest is spawned when enemies are defeated and the chest is in the room that the enemies have been defeated
        if (!chestSpawned && chestSpawnEvent == ChestSpawnEvent.onEnemiesDefeated && chestRoom == roomEnemiesDefeatedArgs.room)
        {
            SpawnChest();
        }
    }
    private void SpawnChest()
    {
        chestSpawned = true;

        // Should chest be spawned based on specified chance? If not return.
        if (!RandomSpawnChest()) return;

        // Get Number Of Ammo, Health, & Weapon Items To Spawn (max 1 of each)
        GetItemsToSpawn(out int ammoNum, out int healthNum, out int weaponNum);

        // Instantiate chest
        GameObject chestGameObject = Instantiate(chestPrefab, this.transform);

        // Position chest
        if (chestSpawnPosition == ChestSpawnPosition.atSpawnerPosition)
        {
            chestGameObject.transform.position = this.transform.position;
        }
        else if (chestSpawnPosition == ChestSpawnPosition.onPlayerPosition)
        {
            // Get nearest spawn position to player
            Vector3 spawnPosition = HelperUtilities.GetSwapnPositionNearestToPlayer(GameManager.Instance.GetPlayer().transform.position);

            // Calculate some random variation
            Vector3 variation = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);

            chestGameObject.transform.position = spawnPosition + variation;
        }

        // Get Chest component
        Chest chest = chestGameObject.GetComponent<Chest>();

        // Initialize chest
        if (chestSpawnEvent == ChestSpawnEvent.onRoomEntry)
        {
            // Don't use materialize effect
            chest.Initialize(false, GetHealthPercentToSpawn(healthNum), GetWeaponDetailsToSpawn(weaponNum), GetAmmoPercentToSpawn(ammoNum));
        }
        else
        {
            // Use materialize effect
            chest.Initialize(true, GetHealthPercentToSpawn(healthNum), GetWeaponDetailsToSpawn(weaponNum), GetAmmoPercentToSpawn(ammoNum));
        }


    }

    private bool RandomSpawnChest()
    {
        // 计算基础概率
        int chancePercent = Random.Range(chestSpawnChanceMin, chestSpawnChanceMax + 1);

        // 检查当前等级是否有自定义的掉落概率
        foreach (RangeByLevel rangeByLevel in chestSpawnChanceByLevelList)
        {
            if (rangeByLevel.dungeonLevel == GameManager.Instance.GetCurrentDungeonLevel())
            {
                chancePercent = Random.Range(rangeByLevel.min, rangeByLevel.max + 1);
                break;
            }
        }

        // 生成一个 1~100 的随机数
        int randomPercent = Random.Range(1, 100 + 1);

        // 判断宝箱是否生成
        if (randomPercent <= chancePercent)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void GetItemsToSpawn(out int ammo, out int health, out int weapons)
    {
        // 初始化掉落物数量
        ammo = 0;
        health = 0;
        weapons = 0;

        // 确定本次掉落的物品总数
        int numberOfItemsToSpawn = Random.Range(numberOfItemsToSpawnMin, numberOfItemsToSpawnMax + 1);

        int choice;

        // 生成 1 个掉落物
        if (numberOfItemsToSpawn == 1)
        {
            choice = Random.Range(0, 3);
            if (choice == 0) { weapons++; return; }
            if (choice == 1) { ammo++; return; }
            if (choice == 2) { health++; return; }
            return;
        }
        // 生成 2 个掉落物
        else if (numberOfItemsToSpawn == 2)
        {
            choice = Random.Range(0, 3);
            if (choice == 0) { weapons++; ammo++; return; }
            if (choice == 1) { ammo++; health++; return; }
            if (choice == 2) { health++; weapons++; return; }
        }
        // 生成 3 个掉落物（最大）
        else if (numberOfItemsToSpawn >= 3)
        {
            weapons++;
            ammo++;
            health++;
            return;
        }
    }

    /// <summary>
    /// Get ammo percent to spawn
    /// </summary>
    private int GetAmmoPercentToSpawn(int ammoNumber)
    {
        if (ammoNumber == 0) return 0;

        // 获取当前关卡的弹药掉落概率范围
        foreach (RangeByLevel spawnPercentByLevel in ammoSpawnByLevelList)
        {
            if (spawnPercentByLevel.dungeonLevel == GameManager.Instance.GetCurrentDungeonLevel())
            {
                return Random.Range(spawnPercentByLevel.min, spawnPercentByLevel.max);
            }
        }

        return 0;
    }

    /// <summary>
    /// Get the weapon details to spawn - return null if no weapon is to be spawned or the player already has the weapon
    /// </summary>
    private WeaponDetailsSO GetWeaponDetailsToSpawn(int weaponNumber)
    {
        if (weaponNumber == 0) return null;

        // Create an instance of the class used to select a random item from a list based on the
        // relative 'ratios' of the items specified
        RandomSpawnableObject<WeaponDetailsSO> weaponRandom = new RandomSpawnableObject<WeaponDetailsSO>(weaponSpawnByLevelList);

        WeaponDetailsSO weaponDetails = weaponRandom.GetItem();

        return weaponDetails;
    }

    /// <summary>
    /// Get health percent to spawn
    /// </summary>
    private int GetHealthPercentToSpawn(int healthNumber)
    {
        if (healthNumber == 0) return 0;

        // Get ammo spawn percent range for level
        foreach (RangeByLevel spawnPercentByLevel in healthSpawnByLevelList)
        {
            if (spawnPercentByLevel.dungeonLevel == GameManager.Instance.GetCurrentDungeonLevel())
            {
                return Random.Range(spawnPercentByLevel.min, spawnPercentByLevel.max);
            }
        }

        return 0;
    }

}
