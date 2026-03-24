using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyTest
{
    // Enemy enemy;

    // A Test behaves as an ordinary method
    [Test]
    public void EnemyTestSimplePasses()
    {
        // Use the Assert class to test conditions

        // Assert.AreEqual(enemy.isNull, enemy.health<=0);

        // public void testEnemyConstructor() {
        //     assertEquals("Enemy1", enemy.getName());
        //     assertEquals(Enemy.DEFAULT_INITIAL_HEALTH, enemy.getHealth());
        // }
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator EnemyTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
