// using UnityEngine;

// public class FireProjectile : MonoBehaviour
// {
//     public Projectile projectilePrefab;
//     // public Enemy enemy;
//     public Tower tower;

//     public float fireRate = 1f;   // shots per second

//     float cooldown = 0f;

//     void Update()
//     {
//         //Count down timer
//         cooldown -= Time.deltaTime;
//         Enemy target = FindFirstEnemy();

//         if (target == null) return;

//         float distanceFromTower = Vector3.Distance(
//             tower.transform.position,
//             enemy.transform.position
//         );

        

//         if (distanceFromTower <= tower.range && cooldown <= 0f)
//         {
//             Fire(target);
//             // Debug.Log("in range");
//             cooldown = 1f / fireRate; //reset timer
//         }
//     }

//     Enemy FindFirstEnemy()
//         {
//             Enemy[] enemies = FindObjectsOfType<Enemy>();

//             Enemy first = null;
//             float closestToGoal = Mathf.Infinity;

//             foreach (Enemy enemy in enemies)
//             {
//                 float distanceToTower = Vector3.Distance(
//                     transform.position,
//                     enemy.transform.position
//                 );

//                 if (distanceToTower > range)
//                     continue;

//                 float distanceToGoal = enemy.DistanceToGoal();

//                 if (distanceToGoal < closestToGoal)
//                 {
//                     closestToGoal = distanceToGoal;
//                     first = enemy;
//                 }
//             }

//             return first;
//         }

//     public void Fire(Enemy target)
//     {
//         Projectile newProjectile = Instantiate(
//             projectilePrefab,
//             transform.position,
//             Quaternion.identity //instantiate w/ default rotation of 0, basically the 0 vector
//         );

//         newProjectile.target = target; //selects which enemy to fire at
//         newProjectile.damage = tower.inflictedDamage; 
//     }
// }