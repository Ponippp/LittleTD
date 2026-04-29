using NUnit.Framework;

public class AStarNodeTests
{
    [Test]
    public void Constructor_SetsDefaults()
    {
        var node = new AStarNode(2, 5, AStarState.UNTESTED);

        Assert.AreEqual(2, node.x);
        Assert.AreEqual(5, node.y);
        Assert.AreEqual(AStarState.UNTESTED, node.aStarState);
        Assert.IsNull(node.parent);
        Assert.AreEqual(0, node.g_cost);
        Assert.AreEqual(0, node.h_cost);
    }

    [Test]
    public void FCost_ReturnsSumOfGAndH()
    {
        var node = new AStarNode(0, 0, AStarState.OPEN)
        {
            g_cost = 12,
            h_cost = 8
        };

        Assert.AreEqual(20, node.F_cost());
    }
}
