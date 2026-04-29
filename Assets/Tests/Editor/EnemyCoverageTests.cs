using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class EnemyCoverageTests
{
    private class FakeEnemyMovementStrategy : IEnemyMovementStrategy
    {
        private readonly float _distanceToGoal;

        public FakeEnemyMovementStrategy(float distanceToGoal)
        {
            _distanceToGoal = distanceToGoal;
        }

        public void Move() { }
        public float GetDistanceToGoal() => _distanceToGoal;
        public void Cleanup() { }
    }

    private static void SetPrivateField<T>(object instance, string fieldName, T value)
    {
        FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field, $"Field '{fieldName}' not found");
        field.SetValue(instance, value);
    }

    [Test]
    public void EnemyFactory_GetDataByType_ReturnsConfiguredAsset()
    {
        EnemyFactory factory = new GameObject("EnemyFactory_Test").AddComponent<EnemyFactory>();
        EnemyData mite = ScriptableObject.CreateInstance<EnemyData>();
        EnemyData cromenockle = ScriptableObject.CreateInstance<EnemyData>();
        EnemyData blart = ScriptableObject.CreateInstance<EnemyData>();
        EnemyData peter = ScriptableObject.CreateInstance<EnemyData>();

        SetPrivateField(factory, "miteData", mite);
        SetPrivateField(factory, "cromenockleData", cromenockle);
        SetPrivateField(factory, "blartData", blart);
        SetPrivateField(factory, "peterData", peter);

        Assert.AreSame(mite, factory.GetDataByType(EnemyType.MITE));
        Assert.AreSame(cromenockle, factory.GetDataByType(EnemyType.CROMENOCKLE));
        Assert.AreSame(blart, factory.GetDataByType(EnemyType.BLART));
        Assert.AreSame(peter, factory.GetDataByType(EnemyType.PETER));
    }

    [Test]
    public void EnemyFactory_CreateEnemyStats_MapsFields()
    {
        EnemyFactory factory = new GameObject("EnemyFactory_Map_Test").AddComponent<EnemyFactory>();
        EnemyData data = ScriptableObject.CreateInstance<EnemyData>();
        data.enemyName = "MapEnemy";
        data.enemyType = EnemyType.PETER;
        data.health = 123f;
        data.speed = 4.5f;
        data.circleColliderRadius = 0.45f;
        data.animationSpeedPercentage = 175;
        data.movementType = EnemyMovementType.FLYING;
        data.coinsDroppedOnKill = 33;

        Enemy.EnemyStats stats = factory.CreateEnemyStats(data);

        Assert.AreEqual("MapEnemy", stats.enemyName);
        Assert.AreEqual(123f, stats.health);
        Assert.AreEqual(4.5f, stats.speed);
        Assert.AreEqual(0.45f, stats.circleColliderRadius);
        Assert.AreEqual(175, stats.animationSpeedPercentage);
        Assert.AreEqual(EnemyMovementType.FLYING, stats.pathfinding.movementType);
        Assert.AreEqual(33, stats.coinsDroppedOnKill);
        Assert.AreEqual(EnemyType.PETER, stats.record.enemyType);
        Assert.IsFalse(stats.record.isInitialized);
    }

    [Test]
    public void Enemy_GetDistanceToGoal_UsesMovementStrategy()
    {
        Enemy enemy = new GameObject("Enemy_Distance_Test").AddComponent<Enemy>();
        enemy.SetMovementStrategy(new FakeEnemyMovementStrategy(42f));

        Assert.AreEqual(42f, enemy.GetDistanceToGoal());
    }

    [Test]
    public void Enemy_GetDistanceToGoal_NoMovementStrategy_ReturnsZero()
    {
        Enemy enemy = new GameObject("Enemy_NoStrategy_Test").AddComponent<Enemy>();
        enemy.SetMovementStrategy(null);

        Assert.AreEqual(0f, enemy.GetDistanceToGoal());
    }

    [Test]
    public void Enemy_TakeDamage_ReducesHealth_WhenStillAlive()
    {
        Enemy enemy = new GameObject("Enemy_TakeDamage_Test").AddComponent<Enemy>();
        var stats = new Enemy.EnemyStats
        {
            health = 10f
        };
        SetPrivateField(enemy, "stats", stats);

        enemy.TakeDamage(3f);

        Assert.AreEqual(7f, enemy.GetHealth());
    }
}
