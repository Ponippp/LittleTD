using UnityEngine;

public class DirectionalStrategy : IProjectileMovementStrategy
{
    private readonly Projectile _projectile;
    private readonly Vector3 _direction;

    public DirectionalStrategy(Projectile projectile, Vector3 direction)
    {
        _projectile = projectile;
        _direction = direction;
    }

    public void Move()
    {
        _projectile.transform.position += _direction * _projectile.GetSpeed() * Time.deltaTime; //v=d/t, so d=vt

    }
}
