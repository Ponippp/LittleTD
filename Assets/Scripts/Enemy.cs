using UnityEngine;
public class Enemy : MonoBehaviour
{
    public float health = 10f;
    public Vector3 finishPoint;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
            Destroy(gameObject);
    }
    public float DistanceToGoal()
    {
        return Vector3.Distance(transform.position, finishPoint);
    }
}
    