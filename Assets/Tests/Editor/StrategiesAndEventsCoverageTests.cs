using System.Reflection;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class StrategiesAndEventsCoverageTests
{
    private readonly List<GameObject> _createdObjects = new();

    private class FakeEnemyMovementStrategy : IEnemyMovementStrategy
    {
        private readonly float _distance;
        public FakeEnemyMovementStrategy(float distance) { _distance = distance; }
        public void Move() { }
        public float GetDistanceToGoal() => _distance;
        public void Cleanup() { }
    }

    private static void InvokeAwake<T>() where T : MonoBehaviour
    {
        MethodInfo awake = typeof(T).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(awake, $"Could not find private Awake on {typeof(T).Name}");
        T component = Object.FindAnyObjectByType<T>();
        Assert.IsNotNull(component);
        awake.Invoke(component, null);
    }

    private static Enemy CreateEnemy(Vector3 position, float health = 50f, float distanceToGoal = 10f)
    {
        GameObject go = new GameObject("Enemy_Test");
        go.transform.position = position;
        go.AddComponent<CircleCollider2D>();
        Enemy enemy = go.AddComponent<Enemy>();
        enemy.SetMovementStrategy(new FakeEnemyMovementStrategy(distanceToGoal));

        FieldInfo statsField = typeof(Enemy).GetField("stats", BindingFlags.Instance | BindingFlags.NonPublic);
        Enemy.EnemyStats stats = (Enemy.EnemyStats)statsField.GetValue(enemy);
        stats.health = health;
        statsField.SetValue(enemy, stats);

        return enemy;
    }

    [SetUp]
    public void SetUp()
    {
        foreach (Enemy enemy in Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            Object.DestroyImmediate(enemy.gameObject);
        }
    }

    [TearDown]
    public void TearDown()
    {
        foreach (GameObject go in _createdObjects)
        {
            if (go != null) Object.DestroyImmediate(go);
        }
        _createdObjects.Clear();

        foreach (Enemy enemy in Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            Object.DestroyImmediate(enemy.gameObject);
        }
    }

    [Test]
    public void GameEvents_InvokesSubscribers()
    {
        GameEvents events = new GameEvents();
        int coins = -1;
        int lives = -1;
        int wave = -1;
        bool gridUpdated = false;
        bool gameOver = false;
        Tower selectedTower = null;

        events.OnCoinsUpdated += c => coins = c;
        events.OnLivesUpdated += l => lives = l;
        events.OnWaveUpdated += w => wave = w;
        events.OnTowerGridUpdated += () => gridUpdated = true;
        events.OnToggleGameOverText += () => gameOver = true;
        events.OnTowerSelected += t => selectedTower = t;

        GameObject towerObject = new GameObject("Tower_Event_Test");
        _createdObjects.Add(towerObject);
        Tower tower = towerObject.AddComponent<Tower>();
        events.CoinsUpdated(5);
        events.LivesUpdated(3);
        events.WaveUpdated(2);
        events.TowerGridUpdated();
        events.ToggleGameOverText();
        events.TowerSelected(tower);

        Assert.AreEqual(5, coins);
        Assert.AreEqual(3, lives);
        Assert.AreEqual(2, wave);
        Assert.IsTrue(gridUpdated);
        Assert.IsTrue(gameOver);
        Assert.AreSame(tower, selectedTower);
    }

    [Test]
    public void GameEvents_SetupNewAStarGrid_InvokesSubscriberWithArgs()
    {
        GameEvents events = new GameEvents();
        int receivedHeight = -1;
        int receivedWidth = -1;
        Vector3 receivedOffset = Vector3.zero;
        UnityEngine.Tilemaps.Tilemap receivedTilemap = null;
        UnityEngine.Tilemaps.Tilemap expectedTilemap = new GameObject("SetupGridTilemap").AddComponent<UnityEngine.Tilemaps.Tilemap>();
        _createdObjects.Add(expectedTilemap.gameObject);

        events.OnSetupNewAStarGrid += (height, width, offset, tilemap) =>
        {
            receivedHeight = height;
            receivedWidth = width;
            receivedOffset = offset;
            receivedTilemap = tilemap;
        };

        events.SetupNewAStarGrid(6, 8, new Vector3(1f, 2f, 0f), expectedTilemap);

        Assert.AreEqual(6, receivedHeight);
        Assert.AreEqual(8, receivedWidth);
        Assert.AreEqual(new Vector3(1f, 2f, 0f), receivedOffset);
        Assert.AreSame(expectedTilemap, receivedTilemap);
    }

    [Test]
    public void FirstAndLastEnemyStrategies_SelectByDistanceToGoal()
    {
        Utility.ENEMY__LAYERMASK = ~0;
        Enemy nearGoal = CreateEnemy(new Vector3(1, 0, 0), distanceToGoal: 1f);
        Enemy farGoal = CreateEnemy(new Vector3(2, 0, 0), distanceToGoal: 20f);
        _createdObjects.Add(nearGoal.gameObject);
        _createdObjects.Add(farGoal.gameObject);

        var firstResult = new FirstEnemyStrategy().UpdateAiming(Vector3.zero, 10f);
        var lastResult = new LastEnemyStrategy().UpdateAiming(Vector3.zero, 10f);

        Assert.AreSame(nearGoal, firstResult.enemy);
        Assert.IsTrue(firstResult.shouldFire);
        Assert.AreSame(farGoal, lastResult.enemy);
        Assert.IsTrue(lastResult.shouldFire);
    }

    [Test]
    public void ClosestWeakestStrongestStrategies_SelectExpectedEnemy()
    {
        Utility.ENEMY__LAYERMASK = ~0;
        Enemy closeLowHp = CreateEnemy(new Vector3(1, 0, 0), health: 5f);
        Enemy farHighHp = CreateEnemy(new Vector3(4, 0, 0), health: 30f);
        _createdObjects.Add(closeLowHp.gameObject);
        _createdObjects.Add(farHighHp.gameObject);

        var closest = new ClosestEnemyStrategy().UpdateAiming(Vector3.zero, 10f);
        var weakest = new WeakestEnemyStrategy().UpdateAiming(Vector3.zero, 10f);
        var strongest = new StrongestEnemyStrategy().UpdateAiming(Vector3.zero, 10f);

        Assert.AreSame(closeLowHp, closest.enemy);
        Assert.AreSame(closeLowHp, weakest.enemy);
        Assert.AreSame(farHighHp, strongest.enemy);
    }

    [Test]
    public void SpinAndDirectionalStrategies_ProduceAimingAndMovement()
    {
        AimingResult spin = new SpinStrategy(360f).UpdateAiming(Vector3.zero, 5f);
        Assert.IsTrue(spin.shouldFire);
        Assert.IsNull(spin.enemy);
        Assert.AreEqual(10f, spin.targetPosition.magnitude, 0.001f);

        GameObject projectileObject = new GameObject("Directional_Projectile_Test");
        _createdObjects.Add(projectileObject);
        Projectile projectile = projectileObject.AddComponent<Projectile>();
        projectile.speed = 10f;
        Vector3 before = projectile.transform.position;
        new DirectionalStrategy(projectile, Vector3.right).Move();
        Assert.GreaterOrEqual(projectile.transform.position.x, before.x);
    }

    [Test]
    public void GameEvents_MethodsWithoutSubscribers_DoNotThrow()
    {
        GameEvents events = new GameEvents();
        Assert.DoesNotThrow(() => events.CoinsUpdated(1));
        Assert.DoesNotThrow(() => events.LivesUpdated(1));
        Assert.DoesNotThrow(() => events.WaveUpdated(1));
        Assert.DoesNotThrow(() => events.TowerGridUpdated());
        Assert.DoesNotThrow(() => events.ToggleGameOverText());
    }

}
