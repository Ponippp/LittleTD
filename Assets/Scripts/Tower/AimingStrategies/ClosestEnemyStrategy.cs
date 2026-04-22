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
            if (col.TryGetComponent<Enemy>(out var e)) //out var e sets enemy to be e iff valid, otherwise it doesnt enter the if statement
            {
                float distToTower = Vector3.Distance(towerPosition, e.transform.position);

                if (distToTower < minDistance)
                {
                    minDistance = distToTower;
                    closest = e;
                }
            }
        }

        AimingResult result = new AimingResult(); //aiming result in IAimingStrategy

        if (closest != null) //if there is a closest enemy

        {
            result.enemy = closest; //sets the enemy in the aiming result to the closest enemy in ClosestEnemyStrategy

            result.targetPosition = closest.transform.position;
            result.shouldFire = true;

            //takes direction vector (which points from tower to enemy) and takes the arctan of it
            //to get the angle in rads, converts to degrees so sprites can use it
            Vector2 direction = result.targetPosition - towerPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            result.lookingAngle = (angle + 360f) % 360f;
        }
        else
        {
            result.shouldFire = false;
        }

        return result; //above, we modified result, so we return it here. otherwise the modifications would do nothing

    }
}
