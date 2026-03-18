using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    public Projectile projectilePrefab;
    public Enemy enemy;
    public Tower tower;

    public float fireRate = 1f;   // shots per second

    float cooldown = 0f;

    void Update()
    {
        if (enemy == null) return;

        //Count down timer
        cooldown -= Time.deltaTime;

        float distance = Vector3.Distance(
            tower.transform.position,
            enemy.transform.position
        );

        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     Fire();
        // }

        if (distance <= tower.range && cooldown <= 0f)
        {
            Fire();
            // Debug.Log("in range");
            cooldown = 1f / fireRate; //reset timer
        }
    }

    public void Fire()
    {
        Projectile newProjectile = Instantiate(
            projectilePrefab,
            transform.position,
            Quaternion.identity //instantiate w/ default rotation of 0, basically the 0 vector
        );

        newProjectile.target = enemy; //selects which enemy to fire at 
    }
}