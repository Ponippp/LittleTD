using NUnit.Framework;
using System.Reflection;
using UnityEngine;

public class ProjectileCoverageTests
{
    private static void InvokeUpdate(Projectile projectile)
    {
        MethodInfo update = typeof(Projectile).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(update, "Could not find private Update on Projectile");
        update.Invoke(projectile, null);
    }

    private class FakeProjectileStrategy : IProjectileMovementStrategy
    {
        private readonly Projectile _projectile;
        private readonly Vector3 _delta;

        public FakeProjectileStrategy(Projectile projectile, Vector3 delta)
        {
            _projectile = projectile;
            _delta = delta;
        }

        public void Move()
        {
            _projectile.transform.position += _delta;
        }
    }

    [Test]
    public void Initialize_SetsProjectileRuntimeFields()
    {
        Projectile projectile = new GameObject("Projectile_Init_Test").AddComponent<Projectile>();

        projectile.Initialize(8f, 14f, null);

        Assert.AreEqual(8f, projectile.damage);
        Assert.AreEqual(14f, projectile.speed);
        Assert.AreEqual(1, projectile.pierce);
        Assert.AreEqual(14f, projectile.GetSpeed());
    }

    [Test]
    public void Update_WithNullStrategy_DoesNothing()
    {
        Projectile projectile = new GameObject("Projectile_NullStrategy_Test").AddComponent<Projectile>();
        projectile.transform.position = new Vector3(2f, 3f, 0f);

        projectile.Initialize(1f, 2f, null);
        InvokeUpdate(projectile);

        Assert.AreEqual(new Vector3(2f, 3f, 0f), projectile.transform.position);
    }

    [Test]
    public void Update_WithStrategy_MovesAndRotatesProjectile()
    {
        Projectile projectile = new GameObject("Projectile_Move_Test").AddComponent<Projectile>();
        projectile.transform.position = Vector3.zero;
        Vector3 movement = new Vector3(1f, 1f, 0f);

        projectile.Initialize(1f, 10f, new FakeProjectileStrategy(projectile, movement));
        InvokeUpdate(projectile);

        Assert.Greater(projectile.transform.position.magnitude, 0.0001f);

        Vector3 expectedUp = -movement.normalized;
        Assert.AreEqual(expectedUp.x, projectile.transform.up.x, 0.0001f);
        Assert.AreEqual(expectedUp.y, projectile.transform.up.y, 0.0001f);
    }
}
