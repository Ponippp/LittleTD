using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TowerTest
{
    // Tower tower;

    // A Test behaves as an ordinary method
    [Test]
    public void TowerTestSimplePasses()
    {
        // Use the Assert class to test conditions

        // public void testTowerConstructor() {
        //     assertEquals("Cannon", tower.getType());
        //     assertEquals(Tower.DEFAULT_INITIAL_DAMAGE, tower.getDamage());
        // }
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator TowerTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
