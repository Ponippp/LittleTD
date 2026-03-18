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

    public float range = 2f;
    public float inflictedDamage = 4f;
    public float fireRate = 1f;

    float cooldown = 0f;

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

            float distToGoal = e.DistanceToGoal();

            if (distToGoal < closestToGoal)
            {
                closestToGoal = distToGoal;
                first = e;
            }
        }

        return first;
    }

    void Fire(Enemy target)
    {
        Projectile proj = Instantiate(
            projectilePrefab,
            transform.position,
            Quaternion.identity
        );

        proj.target = target;
        proj.damage = inflictedDamage;
    }
}