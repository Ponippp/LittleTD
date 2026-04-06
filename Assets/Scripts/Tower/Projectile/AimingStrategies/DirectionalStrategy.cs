using UnityEngine;

public class DirectionalStrategy : IAimingStrategy
{
    private readonly Projectile _projectile;
    private readonly Vector3 _direction;

    public DirectionalStrategy(Projectile projectile, Enemy target, Transform towerTransform)
    {
        _projectile = projectile;
        _direction = (target.transform.position - towerTransform.position).normalized;
    }

    public void Move()
    {
        _projectile.transform.position += _direction * _projectile.GetSpeed() * Time.deltaTime;
    }
}
