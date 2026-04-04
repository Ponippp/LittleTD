// using UnityEngine;

// public class Tower : MonoBehaviour
// {
//     public float range = 2.0f; 
//     public float inflictedDamage = 4.0f; //4.0f

// //    public FireProjectile projectile;
// //     void Update()
// //     {
// //         if (Vector3.Distance(transform.position, target.transform.position) <= range)
// //         {
// //             projectile.Fire();
// //         }
// //     }
// }

using UnityEngine;

public class Tower : MonoBehaviour
{
    public Projectile projectilePrefab;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TowerState towerState = TowerState.IDLE;

    public float range = 2f;
    public float inflictedDamage = 4f;
    public float fireRate = 1f;

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
            cooldown = 1f / fireRate;
        }
    }

    Enemy FindFirstEnemy()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None); //FindObjectsSortMode.None makes it faster and not obsolete

        Enemy first = null;
        float closestToGoal = Mathf.Infinity;

        foreach (Enemy e in enemies)
        {
            float distToTower = Vector3.Distance(
                transform.position,
                e.transform.position
            );

            if (distToTower > range)
                continue;

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
        Projectile proj = Instantiate(
            projectilePrefab,
            transform.position,
            Quaternion.identity
        );

        proj.target = target;
        proj.damage = inflictedDamage;
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
