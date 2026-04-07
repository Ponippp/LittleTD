using UnityEngine;

public class StrongestEnemyStrategy : IAimingStrategy
{
    public AimingResult UpdateAiming(Vector3 towerPosition, float range)
    {
        Collider2D[] colliderInRanges = Physics2D.OverlapCircleAll(towerPosition, range, Utility.ENEMY__LAYERMASK);

        Enemy strongest = null;
        float maxHealth = -Mathf.Infinity;

        foreach (Collider2D col in colliderInRanges)
        {
            if (col.TryGetComponent<Enemy>(out var e))
            {
                float health = e.GetHealth();

                if (health > maxHealth)
                {
                    maxHealth = health;
                    strongest = e;
                }
            }
        }

        AimingResult result = new AimingResult();
        if (strongest != null)
        {
            result.enemy = strongest;
            result.targetPosition = strongest.transform.position;
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
