using System;
using UnityEditor.UI;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Type")]
    [SerializeField] private TowerType towerType;
    
    [Header("References")]
    [SerializeField] public Projectile projectilePrefab;
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

        public ProjectileStats projectileStats = new();
        [Serializable]
        public class ProjectileStats
        {
            public BaseBoostedFloat speed = new();
            public BaseBoostedFloat damage = new();
            public TowerProjectileAimingType aimingType = TowerProjectileAimingType.DIRECTED;
        }

        public VisualStats visualStats = new();
        [Serializable]
        public class VisualStats
        {
            public float fireAnimationTime = 0.1f;
            public Vector2 projectileSpawnRingBottomOffset = new Vector2(0f, -0.2f);
            public float projectileSpawnRingRadius = 0.75f;
        }
        public RecordStats recordStats = new();
        [Serializable]
        public class RecordStats
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
        stats.recordStats.isInitalized = true;
    }

    private void Update()
    {
        if (stats.fireCooldown > 0f) stats.fireCooldown -= Time.deltaTime;

        Enemy target = FindFirstEnemy();

        if (stats.recordStats.isInitalized && target != null && stats.fireCooldown <= 0f)
        {
            Fire(target);
            stats.fireCooldown = stats.fireInterval.BaseBoostedF;
        }
    }

    private Enemy FindFirstEnemy()
    {
        Collider2D[] colliderInRanges = Physics2D.OverlapCircleAll(transform.position, stats.range.BaseBoostedF, Utility.ENEMY__LAYERMASK);

        Enemy first = null;
        float closestToGoal = Mathf.Infinity;

        foreach (Collider2D col in colliderInRanges)
        {
            if (col.TryGetComponent<Enemy>(out var e))
            {
                float distToGoal = e.GetDistanceToGoal();

                if (distToGoal < closestToGoal)
                {
                    closestToGoal = distToGoal;
                    first = e;
                }
            }
        }

        return first;
    }

    void Fire(Enemy target)
    {
        OnFire?.Invoke();

        Vector3 spawnPos = CalculateSpawnPosition(target);
        Projectile proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        float dmg = stats.projectileStats.damage.BaseBoostedF;
        float speed = stats.projectileStats.speed.BaseBoostedF;
        TowerProjectileAimingType aimingType = stats.projectileStats.aimingType;

        if (aimingType == TowerProjectileAimingType.HOMING)
        {
            proj.Initialize(dmg, speed, new HomingStrategy(proj, target), RecordDamageDealt);
        }
        else if (aimingType == TowerProjectileAimingType.DIRECTED)
        {
            proj.Initialize(dmg, speed, new DirectionalStrategy(proj, target, transform), RecordDamageDealt);
        }

    }

    private Vector3 CalculateSpawnPosition(Enemy target)
    {
        Vector3 bottomPos = transform.position + (Vector3)stats.visualStats.projectileSpawnRingBottomOffset;
        Vector3 ringCenter = bottomPos + (Vector3.up * stats.visualStats.projectileSpawnRingRadius);

        Vector3 targetDir = (target.transform.position - transform.position).normalized;

        return ringCenter + (targetDir * stats.visualStats.projectileSpawnRingRadius);
    }

    public float GetLookingDirection()
    {
        Enemy target = FindFirstEnemy();
        if (target == null) return 0f;

        Vector3 direction = target.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return (angle + 360f) % 360f;
    }

    public TowerState GetTowerState() { return stats.towerState; }
    public TowerType GetTowerType() { return towerType; }
    public string GetTowerName() { return stats.recordStats.towerName; }
    public bool GetIsInitalized() { return stats.recordStats.isInitalized; }

    public void RecordDamageDealt(float damage) { stats.recordStats.totalDamageDealt += damage; }

    public void SetSprite(Sprite sprite) { if (spriteRenderer != null) spriteRenderer.sprite = sprite; }
    public void SetColor(Color color) { if (spriteRenderer != null) spriteRenderer.color = color; }
}
