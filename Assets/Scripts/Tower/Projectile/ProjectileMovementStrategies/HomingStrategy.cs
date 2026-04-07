using UnityEngine;

public class HomingStrategy : IProjectileMovementStrategy
{
    private readonly Projectile _projectile;
    private readonly Enemy _target;

    public HomingStrategy(Projectile projectile, Enemy target)
    {
        _projectile = projectile;
        _target = target;
    }

    public void Move()
    {
        if (_target == null)
        {
            _projectile.ResetAndEnqueueProjectile();
            return;
        }

        _projectile.transform.position = Vector3.MoveTowards(_projectile.transform.position, _target.transform.position, _projectile.GetSpeed() * Time.deltaTime);
    }
}
