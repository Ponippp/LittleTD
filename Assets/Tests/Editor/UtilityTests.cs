using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class UtilityTests
{
    [Test]
    public void CalculatePathLength_NullPath_ReturnsZero()
    {
        float length = Utility.CalculatePathLength(null);
        Assert.AreEqual(0f, length);
    }

    [Test]
    public void CalculatePathLength_EmptyPath_ReturnsZero()
    {
        float length = Utility.CalculatePathLength(new List<Vector3>());
        Assert.AreEqual(0f, length);
    }

    [Test]
    public void CalculatePathLength_MultiplePoints_ReturnsSumOfSegments()
    {
        var path = new List<Vector3>
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(3f, 4f, 0f)
        };

        float length = Utility.CalculatePathLength(path);

        Assert.AreEqual(7f, length, 0.0001f);
    }

    [Test]
    public void RandomAngleOffset_PreservesMagnitude()
    {
        Vector2 direction = new Vector2(3f, 4f);
        Vector2 rotated = Utility.RandomAngleOffset(direction, 35f);

        Assert.AreEqual(direction.magnitude, rotated.magnitude, 0.0001f);
    }
}
