using UnityEngine;

public class FirstEnemyStrategy : IAimingStrategy
{
    public AimingResult UpdateAiming(Vector3 towerPosition, float range)
    {
        //OverlapCircleAll returns an array of all colliders that overlap with the tower's range circle (AKA all colliders in range)
        Collider2D[] colliderInRanges = Physics2D.OverlapCircleAll(towerPosition, range, Utility.ENEMY__LAYERMASK);

        Enemy first = null;
        float closestToGoal = Mathf.Infinity;

        foreach (Collider2D col in colliderInRanges)
        {
            //iterates thru all colliders in range and only considers the ones that are enemies 
            //(there are tower colliders an projectile colliders too)
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
            //have to call on result.enemy and result.targetPosition and result.shouldFire and result.lookingAngle because they are all public variables in AimingResult, which is a struct, so we cant use a constructor to set them
            //.enemy sepcifies which enemy to use to calculate the position to fire at
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
