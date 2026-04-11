using UnityEngine;

public class FirstEnemyStrategy : IAimingStrategy
{
    public AimingResult UpdateAiming(Vector3 towerPosition, float range)
    {
        Collider2D[] colliderInRanges = Physics2D.OverlapCircleAll(towerPosition, range, Utility.ENEMY__LAYERMASK);

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

        AimingResult result = new AimingResult();
        if (first != null)
        {
            result.enemy = first;
            result.targetPosition = first.transform.position;
            result.shouldFire = true;

            Vector3 direction = result.targetPosition - towerPosition;
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
