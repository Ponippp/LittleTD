using UnityEngine;

public class DirectionalStrategy : IProjectileMovementStrategy
{
    private readonly Projectile _projectile;
    private readonly Vector3 _direction;

    public DirectionalStrategy(Projectile projectile, Vector3 target, Transform towerTransform)
    {
        _projectile = projectile;
        _direction = (target - towerTransform.position).normalized;
    }

    public void Move()
    {
        _projectile.transform.position += _direction * _projectile.GetSpeed() * Time.deltaTime;
    }
}
