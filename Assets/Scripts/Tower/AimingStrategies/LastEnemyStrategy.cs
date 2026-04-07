using UnityEngine;

public class LastEnemyStrategy : IAimingStrategy
{
    public AimingResult UpdateAiming(Vector3 towerPosition, float range)
    {
        Collider2D[] colliderInRanges = Physics2D.OverlapCircleAll(towerPosition, range, Utility.ENEMY__LAYERMASK);

        Enemy last = null;
        float furthestFromGoal = -Mathf.Infinity;

        foreach (Collider2D col in colliderInRanges)
        {
            if (col.TryGetComponent<Enemy>(out var e))
            {
                float distToGoal = e.GetDistanceToGoal();

                if (distToGoal > furthestFromGoal)
                {
                    furthestFromGoal = distToGoal;
                    last = e;
                }
            }
        }

        AimingResult result = new AimingResult();
        if (last != null)
        {
            result.enemy = last;
            result.targetPosition = last.transform.position;
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
