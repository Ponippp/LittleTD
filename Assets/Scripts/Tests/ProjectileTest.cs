using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ProjectileTest
{
    // Projectile projectile;

    // A Test behaves as an ordinary method
    [Test]
    public void ProjectileTestSimplePasses()
    {
        // Use the Assert class to test conditions

        // verify projectile destroyed when it hits enemy (transform.position is from projectile.cs)
        // transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        // Assert.AreEqual(projectile.isNull, Vector3.Distance(transform.position, target.transform.position) < 0.1f);
        // Assert.AreEqual(!projectile.isNull, Vector3.Distance(transform.position, target.transform.position) >= 0.1f);

        // public void testProjectileConstructor() {
        //     assertEquals(Projectile.DEFAULT_INITIAL_SPEED, projectile.getSpeed());
        // }
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator ProjectileTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
