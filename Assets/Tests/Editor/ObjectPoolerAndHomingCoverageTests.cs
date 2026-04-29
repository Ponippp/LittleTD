using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ObjectPoolerAndHomingCoverageTests
{
    [SetUp]
    public void SetUp()
    {
        ResetObjectPoolerState();
    }

    [TearDown]
    public void TearDown()
    {
        ResetObjectPoolerState();
    }

    private static void ResetObjectPoolerState()
    {
        ObjectPooler.poolLookup.Clear();
        ObjectPooler.poolDictionary.Clear();

        FieldInfo maxSizeField = typeof(ObjectPooler).GetField("poolMaxSizeDictionary", BindingFlags.Static | BindingFlags.NonPublic);
        var maxSizeDictionary = (Dictionary<string, int>)maxSizeField.GetValue(null);
        maxSizeDictionary.Clear();

        FieldInfo parentsField = typeof(ObjectPooler).GetField("poolParentObjects", BindingFlags.Static | BindingFlags.NonPublic);
        var parentsDictionary = (Dictionary<string, GameObject>)parentsField.GetValue(null);
        parentsDictionary.Clear();

        FieldInfo rootField = typeof(ObjectPooler).GetField("OBJECTPOOLS_ROOT", BindingFlags.Static | BindingFlags.NonPublic);
        rootField.SetValue(null, null);
    }

    [Test]
    public void SetupPool_Dequeue_Enqueue_TracksActiveCount()
    {
        string key = "Pool_Test";
        Projectile prefab = new GameObject("ProjectilePrefab").AddComponent<Projectile>();

        ObjectPooler.SetupPool(prefab, 2, key);
        Assert.IsTrue(ObjectPooler.poolDictionary.ContainsKey(key));
        Assert.AreEqual(2, ObjectPooler.poolDictionary[key].Count);

        Projectile instance = ObjectPooler.DequeueObject<Projectile>(key);
        instance.gameObject.SetActive(true);
        Assert.AreEqual(1, ObjectPooler.poolDictionary[key].Count);

        ObjectPooler.EnqueueObject(instance, key);
        Assert.AreEqual(2, ObjectPooler.poolDictionary[key].Count);
    }

    [Test]
    public void EnqueueNewInstance_WithEnqueueFalse_DoesNotAddToQueue()
    {
        string key = "Pool_NewInstance";
        Projectile prefab = new GameObject("ProjectilePrefab_New").AddComponent<Projectile>();
        ObjectPooler.SetupPool(prefab, 0, key);

        int before = ObjectPooler.poolDictionary[key].Count;
        Projectile instance = ObjectPooler.EnqueueNewInstance(prefab, key, false);

        Assert.IsNotNull(instance);
        Assert.AreEqual(before, ObjectPooler.poolDictionary[key].Count);
    }

    [Test]
    public void HomingStrategy_Move_WithTarget_MovesTowardTarget()
    {
        Projectile projectile = new GameObject("HomingProjectile").AddComponent<Projectile>();
        Enemy target = new GameObject("HomingTarget").AddComponent<Enemy>();

        projectile.transform.position = Vector3.zero;
        target.transform.position = new Vector3(5f, 0f, 0f);
        projectile.Initialize(1f, 100f, null);

        float distanceBefore = Vector3.Distance(projectile.transform.position, target.transform.position);
        new HomingStrategy(projectile, target).Move();
        float distanceAfter = Vector3.Distance(projectile.transform.position, target.transform.position);

        Assert.LessOrEqual(distanceAfter, distanceBefore);
    }

    [Test]
    public void HomingStrategy_Move_WithNullTarget_ResetsAndEnqueuesProjectile()
    {
        Projectile prefab = new GameObject("ProjectilePrefab_HomingNull").AddComponent<Projectile>();
        ObjectPooler.SetupPool(prefab, 0, Utility.PROJECTILE_OBJECTPOOL_NAME);

        Projectile projectile = ObjectPooler.EnqueueNewInstance(prefab, Utility.PROJECTILE_OBJECTPOOL_NAME, false);
        projectile.gameObject.SetActive(true);
        projectile.transform.position = new Vector3(3f, 4f, 0f);
        projectile.Initialize(9f, 11f, null);

        new HomingStrategy(projectile, null).Move();

        Assert.AreEqual(0f, projectile.damage);
        Assert.AreEqual(0f, projectile.speed);
        Assert.AreEqual(0, projectile.pierce);
        Assert.IsFalse(projectile.gameObject.activeSelf);
    }
}
