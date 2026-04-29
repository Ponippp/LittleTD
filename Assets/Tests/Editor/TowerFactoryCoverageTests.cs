using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class TowerFactoryCoverageTests
{
    private static void SetPrivateField<T>(object instance, string fieldName, T value)
    {
        FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field, $"Field '{fieldName}' not found");
        field.SetValue(instance, value);
    }

    [Test]
    public void GetDataByType_ReturnsConfiguredData()
    {
        TowerFactory factory = new GameObject("TowerFactory_Test").AddComponent<TowerFactory>();

        TowerData giga = ScriptableObject.CreateInstance<TowerData>();
        TowerData office = ScriptableObject.CreateInstance<TowerData>();
        TowerData juicy = ScriptableObject.CreateInstance<TowerData>();

        SetPrivateField(factory, "gigaGatlingData", giga);
        SetPrivateField(factory, "officeChairData", office);
        SetPrivateField(factory, "juicyRagLauncherData", juicy);

        Assert.AreSame(giga, factory.GetDataByType(TowerType.GIGA_GATLING));
        Assert.AreSame(office, factory.GetDataByType(TowerType.OFFICE_CHAIR));
        Assert.AreSame(juicy, factory.GetDataByType(TowerType.JUICY_RAG_LAUNCHER));
    }

    [Test]
    public void CreateTowerStats_MapsDataFieldsToRuntimeStats()
    {
        TowerFactory factory = new GameObject("TowerFactory_Map_Test").AddComponent<TowerFactory>();
        TowerData data = ScriptableObject.CreateInstance<TowerData>();

        data.towerName = "MapTest";
        data.towerDescription = "desc";
        data.towerType = TowerType.OFFICE_CHAIR;
        data.baseTowerCost = 777;
        data.baseRange = 9f;
        data.baseFireInterval = 1.2f;
        data.baseBulletSpreadAngle = 7f;
        data.baseProjectilesFiredWithEachShot = 3;
        data.baseReleaseTimeBetweenEachProjectileInBurst = 0.25f;
        data.towerAimingType = TowerAimingType.CLOSEST;
        data.baseTowerSwivelSpeed = 222f;
        data.baseProjectileSpeed = 13f;
        data.baseProjectileDamage = 21f;
        data.projectileMovementType = ProjectileMovementType.HOMING;
        data.fireAnimationTime = 0.33f;
        data.projectileSpawnRingBottomOffset = new Vector2(1f, -1f);
        data.projectileSpawnRingRadius = 1.5f;

        Tower.TowerStats stats = factory.CreateTowerStats(data);

        Assert.AreEqual(9f, stats.range.baseF);
        Assert.AreEqual(1.2f, stats.fireInterval.baseF);
        Assert.AreEqual(7f, stats.baseBulletSpreadAngle.baseF);
        Assert.AreEqual(3, stats.projectilesFiredWithEachShot.baseI);
        Assert.AreEqual(0.25f, stats.baseReleaseTimeBetweenEachProjectileInBurst.baseF);
        Assert.AreEqual(TowerAimingType.CLOSEST, stats.aiming.type);
        Assert.AreEqual(222f, stats.aiming.swivelSpeed.baseF);
        Assert.AreEqual(13f, stats.projectile.speed.baseF);
        Assert.AreEqual(21f, stats.projectile.damage.baseF);
        Assert.AreEqual(ProjectileMovementType.HOMING, stats.projectile.movementType);
        Assert.AreEqual(0.33f, stats.visual.fireAnimationTime);
        Assert.AreEqual(new Vector2(1f, -1f), stats.visual.projectileSpawnRingBottomOffset);
        Assert.AreEqual(1.5f, stats.visual.projectileSpawnRingRadius);
        Assert.AreEqual("MapTest", stats.record.towerName);
        Assert.AreEqual("desc", stats.record.towerDescription);
        Assert.AreEqual(TowerType.OFFICE_CHAIR, stats.record.towerType);
        Assert.AreEqual(777f, stats.record.baseTowerCost.baseF);
    }
}
