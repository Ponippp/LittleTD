using NUnit.Framework;

public class EnumTests
{
    [Test]
    public void EnemyType_ContainsExpectedEntries()
    {
        Assert.AreEqual(4, System.Enum.GetValues(typeof(EnemyType)).Length);
        Assert.IsTrue(System.Enum.IsDefined(typeof(EnemyType), EnemyType.MITE));
        Assert.IsTrue(System.Enum.IsDefined(typeof(EnemyType), EnemyType.CROMENOCKLE));
        Assert.IsTrue(System.Enum.IsDefined(typeof(EnemyType), EnemyType.BLART));
        Assert.IsTrue(System.Enum.IsDefined(typeof(EnemyType), EnemyType.PETER));
    }

    [Test]
    public void TowerAimingType_ContainsSpin()
    {
        Assert.IsTrue(System.Enum.IsDefined(typeof(TowerAimingType), TowerAimingType.SPIN));
    }

    [Test]
    public void ProjectileMovementType_ValuesAreStable()
    {
        Assert.AreEqual(0, (int)ProjectileMovementType.DIRECTED);
        Assert.AreEqual(1, (int)ProjectileMovementType.HOMING);
    }
}
