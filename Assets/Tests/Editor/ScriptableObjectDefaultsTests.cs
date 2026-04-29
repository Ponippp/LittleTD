using NUnit.Framework;
using UnityEngine;

public class ScriptableObjectDefaultsTests
{
    [Test]
    public void EnemyData_DefaultValues_AreSane()
    {
        EnemyData data = ScriptableObject.CreateInstance<EnemyData>();

        Assert.AreEqual("DefaultName", data.enemyName);
        Assert.AreEqual(50f, data.health);
        Assert.AreEqual(1f, data.speed);
        Assert.AreEqual(0.3f, data.circleColliderRadius);
        Assert.AreEqual(100, data.animationSpeedPercentage);
        Assert.AreEqual(EnemyMovementType.GROUND, data.movementType);
        Assert.AreEqual(50, data.coinsDroppedOnKill);
    }

    [Test]
    public void ProjectileData_DefaultValues_AreSane()
    {
        ProjectileData data = ScriptableObject.CreateInstance<ProjectileData>();

        Assert.AreEqual("DefaultProjectile", data.projectileName);
        Assert.AreEqual(15f, data.baseProjectileSpeed);
        Assert.AreEqual(3f, data.baseProjectileDamage);
        Assert.AreEqual(ProjectileMovementType.DIRECTED, data.projectileMovementType);
        Assert.AreEqual(0.2f, data.baseProjectileColliderRadius);
        Assert.AreEqual(1, data.baseProjectilePierce);
        Assert.AreEqual(Vector3.one, data.spriteTransformSize);
        Assert.AreEqual(0f, data.baseProjectileAOERadiusOnHit);
    }

    [Test]
    public void WaveData_DefaultValues_AreSane()
    {
        WaveData data = ScriptableObject.CreateInstance<WaveData>();

        Assert.AreEqual(1f, data.sendInterval);
        Assert.AreEqual(20, data.enemiesInWave);
        Assert.AreEqual(EnemyType.MITE, data.enemyType);
    }

    [Test]
    public void TowerData_DefaultValues_AreSane()
    {
        TowerData data = ScriptableObject.CreateInstance<TowerData>();

        Assert.AreEqual(650, data.baseTowerCost);
        Assert.AreEqual(2f, data.baseRange);
        Assert.AreEqual(0.2f, data.baseFireInterval);
        Assert.AreEqual(0f, data.baseBulletSpreadAngle);
        Assert.AreEqual(1, data.baseProjectilesFiredWithEachShot);
        Assert.AreEqual(0.05f, data.baseReleaseTimeBetweenEachProjectileInBurst);
        Assert.AreEqual(TowerAimingType.FIRST, data.towerAimingType);
        Assert.AreEqual(180f, data.baseTowerSwivelSpeed);
        Assert.AreEqual(30f, data.baseProjectileSpeed);
        Assert.AreEqual(4f, data.baseProjectileDamage);
        Assert.AreEqual(ProjectileMovementType.DIRECTED, data.projectileMovementType);
        Assert.AreEqual(0.1f, data.fireAnimationTime);
        Assert.AreEqual(new Vector2(0f, -0.2f), data.projectileSpawnRingBottomOffset);
        Assert.AreEqual(0.75f, data.projectileSpawnRingRadius);
    }
}
