using NUnit.Framework;

public class UtilityClassesCoverageTests
{
    [Test]
    public void BaseBoostedFloat_ComputesAndResets()
    {
        var value = new BaseBoostedFloat
        {
            baseF = 10f,
            boostF = 5f,
            multiplierF = 2f
        };

        Assert.AreEqual(30f, value.BaseBoostedF);

        value.multiplierDefaultIs0F = true;
        value.ResetReboostiplier();

        Assert.AreEqual(0f, value.boostF);
        Assert.AreEqual(0f, value.multiplierF);
    }

    [Test]
    public void BaseBoostedInt_ComputesAndClampsAtZero()
    {
        var value = new BaseBoostedInt
        {
            baseI = 4,
            boostI = 2,
            multiplierI = 3
        };
        Assert.AreEqual(18, value.BaseBoostedI);

        value.boostI = -10;
        Assert.AreEqual(0, value.BaseBoostedI);

        value.ResetReboostiplier();
        Assert.AreEqual(0, value.boostI);
        Assert.AreEqual(1, value.multiplierI);
    }

    [Test]
    public void BaseBoostedFloat_ImplementsIBaseReboost()
    {
        var value = new BaseBoostedFloat();
        Assert.IsInstanceOf<IBaseReboost>(value);
    }

    [Test]
    public void BaseBoostedInt_ImplementsIBaseReboost()
    {
        var value = new BaseBoostedInt();
        Assert.IsInstanceOf<IBaseReboost>(value);
    }
}
