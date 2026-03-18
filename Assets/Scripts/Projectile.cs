//create reusable projectile object to hit the enemies while they move from A to B
//consider using object pooling

using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Enemy target;
    // public Vector3 targetPoint; //do I need to do Enemy.getLocation() and calculate where the enemy will be based on projectile speed and enemy speed and initial locations of each?
    public float speed = 10.0f; //10.0f
    public float damage;
   
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.transform.position) < 0.1f)
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}

