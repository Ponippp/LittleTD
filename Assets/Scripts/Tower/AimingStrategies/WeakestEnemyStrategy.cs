using UnityEngine;

public class WeakestEnemyStrategy : IAimingStrategy
{
    public AimingResult UpdateAiming(Vector3 towerPosition, float range)
    {
        Collider2D[] colliderInRanges = Physics2D.OverlapCircleAll(towerPosition, range, Utility.ENEMY__LAYERMASK);

        Enemy weakest = null;
        float minHealth = Mathf.Infinity;

        foreach (Collider2D col in colliderInRanges)
        {
            if (col.TryGetComponent<Enemy>(out var e))
            {
                float health = e.GetHealth();

                if (health < minHealth)
                {
                    minHealth = health;
                    weakest = e;
                }
            }
        }

        AimingResult result = new AimingResult();
        if (weakest != null)
        {
            result.enemy = weakest;
            result.targetPosition = weakest.transform.position;
            result.shouldFire = true;

            Vector2 direction = result.targetPosition - towerPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            result.lookingAngle = (angle + 360f) % 360f;
        }
        else
        {
            result.shouldFire = false;
        }

        return result;
    }
}
