using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public Projectile projectilePrefab;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TowerState towerState = TowerState.IDLE;

    [Header("Tower Stats")]
    [SerializeField] private float range = 2f;
    [SerializeField] private float fireInterval = 0.2f;
    [Header("Tower Projectile Stats")]
    [SerializeField] private float projectileSpeed = 30f;
    [SerializeField] private float projectileDamage = 4f;
    [SerializeField] private TowerProjectileAimingType towerProjectileAimingType = TowerProjectileAimingType.DIRECTED;
    [SerializeField] private Vector2 projectileSpawnRingBottomOffset = new Vector2(0f, -0.2f);
    [SerializeField] private float projectileSpawnRingRadius = 0.75f;

    float cooldown = 0f;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null) Debug.LogError("[Tower] No SpriteRenderer component found on this GameObject.");
    }

    void Update()
    {
        cooldown -= Time.deltaTime;

        Enemy target = FindFirstEnemy();

        if (target != null && cooldown <= 0f)
        {
            Fire(target);
            cooldown = fireInterval;
        }
    }

    private Enemy FindFirstEnemy()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None); // FindObjectsSortMode.None makes it faster and not obsolete

        Enemy first = null;
        float closestToGoal = Mathf.Infinity;

        foreach (Enemy e in enemies)
        {
            float distToTower = Vector3.Distance(transform.position, e.transform.position);

            if (distToTower > range) continue;

            float distToGoal = e.GetDistanceToGoal();

            if (distToGoal < closestToGoal)
            {
                closestToGoal = distToGoal;
                first = e;
            }
        }

        return first;
    }

    //polymorphism: Fire() will cause different behavior for different towers. One tower might fire
    //a projectile, another tower might increase the fire rate of a different tower, another tower
    //might lay spikes on the ground
    public event System.Action OnFire;

    void Fire(Enemy target)
    {
        OnFire?.Invoke();

        Vector3 spawnPos = CalculateSpawnPosition(target);
        Projectile proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        if (towerProjectileAimingType == TowerProjectileAimingType.HOMING)
        {
            proj.Initialize(projectileDamage, projectileSpeed, new HomingStrategy(proj, target));
        }
        else if (towerProjectileAimingType == TowerProjectileAimingType.DIRECTED)
        {
            proj.Initialize(projectileDamage, projectileSpeed, new DirectionalStrategy(proj, target, transform));
        }
    }

    /// <summary>
    /// For visual clarity, we set projeciles' spawn positions to be right under the gun nozzle. This method calculates the current position of the nozzle.
    /// </summary>
    private Vector3 CalculateSpawnPosition(Enemy target)
    {
        Vector3 bottomPos = transform.position + (Vector3)projectileSpawnRingBottomOffset;
        Vector3 ringCenter = bottomPos + (Vector3.up * projectileSpawnRingRadius);

        Vector3 targetDir = (target.transform.position - transform.position).normalized;

        return ringCenter + (targetDir * projectileSpawnRingRadius);
    }

    public float GetLookingDirection()
    {
        Enemy target = FindFirstEnemy();
        if (target == null) return 0f;

        Vector3 direction = target.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return (angle + 360f) % 360f; // Normalize to [0, 360)
    }

    public TowerState GetTowerState() { return towerState; }

    public void SetSprite(Sprite sprite) { if (spriteRenderer != null) spriteRenderer.sprite = sprite; }
}
