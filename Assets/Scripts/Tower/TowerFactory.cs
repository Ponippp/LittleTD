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
            if (tower != null && !tower.GetIsInitalized()) tower.Initialize(CreateTowerData(GetDataByType(tower.GetTowerType())));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            CreateTower(TowerType.GIGA_GATLING, new Vector2(2, -3));
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

        towerInstance.Initialize(CreateTowerData(data));

        return towerInstance;
    }

    private Tower.TowerStats CreateTowerData(TowerData data)
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
        return stats;
    }

    private Tower GetPrefabByType(TowerType type)
    {
        switch (type)
        {
            case TowerType.GIGA_GATLING:
                return gigaGatlingPrefab;
            default:
                return null;
        }
    }

    private TowerData GetDataByType(TowerType type)
    {
        switch (type)
        {
            case TowerType.GIGA_GATLING:
                return gigaGatlingData;
            default:
                return null;
        }
    }
}