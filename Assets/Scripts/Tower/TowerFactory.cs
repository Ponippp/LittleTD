using System.Collections.Generic;
using UnityEngine;

public class TowerFactory : MonoBehaviour
{
    public static TowerFactory Instance { get; private set; }

    [Header("Tower Prefabs")]
    [SerializeField] private Tower gigaGatlingPrefab;
    // other tower prefabs would go here

    [Header("Tower Data (Defaults)")]
    [SerializeField] private TowerData gigaGatlingData;
    // other tower data would go here


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        //returns all unity objects of type tower, doesn't sort them to save time
        Tower[] towers = FindObjectsByType<Tower>(FindObjectsSortMode.None);
        foreach (Tower tower in towers)
        {
            if (tower != null && !tower.GetIsInitalized()) tower.Initialize(CreateTowerStats(GetDataByType(tower.GetTowerType())));
        }
    }

    public Tower CreateTower(TowerType type, Vector2 position)
    {
        Tower prefab = GetPrefabByType(type);
        TowerData data = GetDataByType(type);

        if (prefab == null || data == null)
        {
            Debug.LogError($"[TowerFactory] Missing prefab or data for {type}");
            return null;
        }

        Tower towerInstance = Instantiate(prefab, position, Quaternion.identity);

        towerInstance.Initialize(CreateTowerStats(data));

        return towerInstance;
    }

    public Tower.TowerStats CreateTowerStats(TowerData data)
    {
        Tower.TowerStats stats = new();
        stats.range.baseF = data.baseRange;
        stats.fireInterval.baseF = data.baseFireInterval;

        stats.aiming.type = data.towerAimingType;

        stats.projectile.speed.baseF = data.baseProjectileSpeed;
        stats.projectile.damage.baseF = data.baseProjectileDamage;
        stats.projectile.movementType = data.projectileMovementType;

        stats.visual.fireAnimationTime = data.fireAnimationTime;
        stats.visual.rotationSpeed.baseF = data.baseRotationSpeedIfSpinningRadians;
        stats.visual.projectileSpawnRingBottomOffset = data.projectileSpawnRingBottomOffset;
        stats.visual.projectileSpawnRingRadius = data.projectileSpawnRingRadius;

        stats.record.towerName = data.towerName;

        return stats;
    }

    public Tower GetPrefabByType(TowerType type)
    {
        //if tower type is giga gatling, ret gigagatling prefab, if it's any other type, ret null

        return type switch
        {
            TowerType.GIGA_GATLING => gigaGatlingPrefab,
            _ => null,
        };
    }

    public TowerData GetDataByType(TowerType type)
    {
        //if tower type is giga gatling, ret gigagatling data, if it's any other type, ret null

        return type switch
        {
            TowerType.GIGA_GATLING => gigaGatlingData,
            _ => null,
        };
    }

}