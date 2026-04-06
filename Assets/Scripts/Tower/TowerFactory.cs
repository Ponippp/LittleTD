using System.Collections.Generic;
using UnityEngine;

public class TowerFactory : MonoBehaviour
{
    public static TowerFactory Instance { get; private set; }

    [Header("Tower Prefabs")]
    [SerializeField] private Tower gigaGatlingPrefab;

    [Header("Tower Data (Defaults)")]
    [SerializeField] private TowerData gigaGatlingData;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
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

        stats.projectileStats.speed.baseF = data.baseProjectileSpeed;
        stats.projectileStats.damage.baseF = data.baseProjectileDamage;
        stats.projectileStats.aimingType = data.aimingType;

        stats.visualStats.fireAnimationTime = data.fireAnimationTime;
        stats.visualStats.projectileSpawnRingBottomOffset = data.projectileSpawnRingBottomOffset;
        stats.visualStats.projectileSpawnRingRadius = data.projectileSpawnRingRadius;

        stats.recordStats.towerName = data.towerName;

        return stats;
    }

    public Tower GetPrefabByType(TowerType type)
    {
        return type switch
        {
            TowerType.GIGA_GATLING => gigaGatlingPrefab,
            _ => null,
        };
    }

    public TowerData GetDataByType(TowerType type)
    {
        return type switch
        {
            TowerType.GIGA_GATLING => gigaGatlingData,
            _ => null,
        };
    }

}