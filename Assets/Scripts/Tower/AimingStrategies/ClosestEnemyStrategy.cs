using UnityEngine;

public class ClosestEnemyStrategy : IAimingStrategy
{
    public AimingResult UpdateAiming(Vector3 towerPosition, float range)
    {
        Collider2D[] colliderInRanges = Physics2D.OverlapCircleAll(towerPosition, range, Utility.ENEMY__LAYERMASK);

        Enemy closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider2D col in colliderInRanges)
        {
            if (col.TryGetComponent<Enemy>(out var e))
            {
                float distToTower = Vector3.Distance(towerPosition, e.transform.position);

                if (distToTower < minDistance)
                {
                    minDistance = distToTower;
                    closest = e;
                }
            }
        }

        AimingResult result = new AimingResult();
        if (closest != null)
        {
            result.enemy = closest;
            result.targetPosition = closest.transform.position;
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
