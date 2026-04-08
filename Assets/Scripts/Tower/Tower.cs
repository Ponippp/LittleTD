using System;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Type")]
    [SerializeField] private TowerType towerType;

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Runtime Stats (Auto-filled by Factory/Configure)")]
    [SerializeField] private TowerStats stats = new();
    [Serializable]
    public class TowerStats
    {
        public BaseBoostedFloat range = new();
        public BaseBoostedFloat fireInterval = new();
        public TowerState towerState = TowerState.IDLE;
        public float fireCooldown = 0f;
        public Aiming aiming = new();
        [Serializable]
        public class Aiming
        {
            public IAimingStrategy strategy;
            public AimingResult currentResult;
            public TowerAimingType type = TowerAimingType.FIRST;
        }
        public Projectile projectile = new();
        [Serializable]
        public class Projectile
        {
            public BaseBoostedFloat speed = new();
            public BaseBoostedFloat damage = new();
            public ProjectileMovementType movementType = ProjectileMovementType.DIRECTED;
        }

        public Visual visual = new();
        [Serializable]
        public class Visual
        {
            public float fireAnimationTime = 0.1f;
            public BaseBoostedFloat rotationSpeed = new();
            public Vector2 projectileSpawnRingBottomOffset = new Vector2(0f, -0.2f);
            public float projectileSpawnRingRadius = 0.75f;
        }
        public Record record = new();
        [Serializable]
        public class Record
        {
            public float totalDamageDealt = 0;
            public string towerName = "";
            public bool isInitalized = false;
        }
    }
    public event Action OnFire;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null) Debug.LogError("[Tower] No SpriteRenderer component found on this GameObject.");
    }

    public void Initialize(TowerStats newStats)
    {
        stats = newStats;
        stats.fireCooldown = 0f;

        stats.aiming.strategy = stats.aiming.type switch
        {
            TowerAimingType.FIRST => new FirstEnemyStrategy(),
            TowerAimingType.CLOSEST => new ClosestEnemyStrategy(),
            TowerAimingType.STRONGEST => new StrongestEnemyStrategy(),
            TowerAimingType.LAST => new LastEnemyStrategy(),
            TowerAimingType.WEAKEST => new WeakestEnemyStrategy(),
            TowerAimingType.SPIN => new SpinStrategy(stats.visual.rotationSpeed.BaseBoostedF),
            _ => new FirstEnemyStrategy(),
        };

        stats.record.isInitalized = true;
    }

    private void Update()
    {
        if (!stats.record.isInitalized) return;
        UpdateAiming();
    }

    private void UpdateAiming()
    {
        if (stats.fireCooldown > 0f) stats.fireCooldown -= Time.deltaTime;

        stats.aiming.currentResult = stats.aiming.strategy.UpdateAiming(transform.position, stats.range.BaseBoostedF);

        if (stats.aiming.currentResult.shouldFire && stats.fireCooldown <= 0f)
        {
            Fire(stats.aiming.currentResult);
            stats.fireCooldown = stats.fireInterval.BaseBoostedF;
        }
    }

    private void Fire(AimingResult result)
    {
        OnFire?.Invoke();

        //purely for art; we want to spawn the projectile on the gun nozzle, and the gun nozzle is a different size+shape for different towers
        Vector3 spawnPos = CalculateProjectileSpawnPosition(result.targetPosition);
        Projectile proj = ObjectPooler.DequeueObject<Projectile>(Utility.PROJECTILE_OBJECTPOOL_NAME);
        proj.gameObject.SetActive(true);
        proj.transform.position = spawnPos;

        float dmg = stats.projectile.damage.BaseBoostedF;
        float speed = stats.projectile.speed.BaseBoostedF;
        ProjectileMovementType movementType = stats.projectile.movementType;

        if (movementType == ProjectileMovementType.HOMING && result.enemy != null)
        {
            proj.Initialize(dmg, speed, new HomingStrategy(proj, result.enemy), RecordDamageDealt);
        }
        else
        {
            proj.Initialize(dmg, speed, new DirectionalStrategy(proj, result.targetPosition, transform), RecordDamageDealt);
        }
    }

    private Vector3 CalculateProjectileSpawnPosition(Vector3 target)
    {
        Vector3 bottomPos = transform.position + (Vector3)stats.visual.projectileSpawnRingBottomOffset;
        Vector3 ringCenter = bottomPos + (Vector3.up * stats.visual.projectileSpawnRingRadius);

        Vector3 targetDir = (target - transform.position).normalized;

        return ringCenter + (targetDir * stats.visual.projectileSpawnRingRadius);
    }

    public float GetLookingDirection() => stats.aiming.currentResult.lookingAngle;

    public TowerState GetTowerState() { return stats.towerState; }
    public TowerType GetTowerType() { return towerType; }
    public string GetTowerName() { return stats.record.towerName; }
    public bool GetIsInitalized() { return stats.record.isInitalized; }

    public void RecordDamageDealt(float damage) { stats.record.totalDamageDealt += damage; }

    public void SetSprite(Sprite sprite) { if (spriteRenderer != null) spriteRenderer.sprite = sprite; }
    public void SetColor(Color color) { if (spriteRenderer != null) spriteRenderer.color = color; }
}
