using System.Collections.Generic;
using UnityEngine;

public class TowerFactory : MonoBehaviour // not static due to needing start and awake for test purposes
{
    public static TowerFactory Instance { get; private set; }

    // other tower prefabs would go here

    [Header("Tower Data (Defaults)")]
    [SerializeField] private TowerData gigaGatlingData;
    [SerializeField] private TowerData officeChairData;
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
            if (tower != null && !tower.GetIsInitalized()) //isInit is a private var, needs a getter
            {
                //we have preset towers in the scene by default for testing. Initialize()
                //gives them stats. by default in unity, all stats are 0, so it would have 0 range
                //and 0 cooldown and 0 chill, but this gives them dmg, range, etc
                tower.Initialize(CreateTowerStats(GetDataByType(tower.GetTowerType())));
                Utility.SnapToTileCenter(tower.transform);
            }

        }
    }

    public Tower CreateTower(TowerType type, Vector2 position)
    {
        Tower prefab = GameManager.instance.GetTowerPrefab().GetComponent<Tower>();
        TowerData data = GetDataByType(type);

        if (prefab == null || data == null)
        {
            Debug.LogError($"[TowerFactory] Missing prefab or data for {type}");
            return null;
        }
        //we've alr initialized preset towers for testing, but if the user wanst to add more,
        //we instantiate and initialize them here
        Tower towerInstance = Instantiate(prefab, position, Quaternion.identity);

        towerInstance.Initialize(CreateTowerStats(data));

        return towerInstance;
    }

    public Tower.TowerStats CreateTowerStats(TowerData data)
    {
        Tower.TowerStats stats = new();
        //baseF is a custom float data type. with a normal float, if we wanted to boost the range by 
        //20, we would do float+=20. If we applied several boosts and wanted to reset it back to the 
        //default, it would not be easy. With baseF, the boosts are applied on top of the baseF, so 
        //to reset the boosts, we just set baseF back to the default value.
        stats.range.baseF = data.baseRange; 
        stats.fireInterval.baseF = data.baseFireInterval;
        stats.baseBulletSpreadAngle.baseF = data.baseBulletSpreadAngle;
        stats.projectilesFiredWithEachShot.baseI = data.baseProjectilesFiredWithEachShot;
        stats.baseReleaseTimeBetweenEachProjectileInBurst.baseF = data.baseReleaseTimeBetweenEachProjectileInBurst;

        stats.aiming.type = data.towerAimingType;
        // stats.aiming.aimingWindowWhereTowerCanShootAtEnemyRadians.baseF = data.baseAimingWindowWhereTowerCanShootAtEnemyRadians;
        stats.aiming.swivelSpeed.baseF = data.baseTowerSwivelSpeed;

        stats.projectile.speed.baseF = data.baseProjectileSpeed;
        stats.projectile.damage.baseF = data.baseProjectileDamage;
        stats.projectile.movementType = data.projectileMovementType;

        stats.visual.fireAnimationTime = data.fireAnimationTime;
        stats.visual.projectileSpawnRingBottomOffset = data.projectileSpawnRingBottomOffset;
        stats.visual.projectileSpawnRingRadius = data.projectileSpawnRingRadius;

        stats.record.towerName = data.towerName;
        stats.record.towerType = data.towerType;
        stats.record.towerDescription = data.towerDescription;
        stats.record.baseTowerCost.baseF = data.baseTowerCost;

        return stats;
    }

    public TowerData GetDataByType(TowerType type)
    {
        //if tower type is giga gatling, ret gigagatling data, if it's any other type, ret null
        return type switch
        {
            TowerType.GIGA_GATLING => gigaGatlingData,
            TowerType.OFFICE_CHAIR => officeChairData,
            _ => null,
        };
    }

}