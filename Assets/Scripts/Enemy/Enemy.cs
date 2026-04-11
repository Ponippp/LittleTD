using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyStats stats = new();

    [Serializable]
    public class EnemyStats
    {
        public string enemyName = "DefaultName";
        public float health = 50f;
        public float speed = 1f;
        public float circleColliderRadius = 0.3f;
        public int animationSpeedPercentage = 100;
        public Pathfinding pathfinding = new();
        public Record record = new();

        [Serializable]
        public class Pathfinding
        {
            public Vector3 spawnPoint;
            public Vector3 goalPoint;
            public EnemyMovementType movementType = EnemyMovementType.GROUND;
            public IEnemyMovementStrategy movementStrategy;
        }

        [Serializable]
        public class Record
        {
            public EnemyType enemyType;
            public bool isInitialized;
        }
    }

    public void Initialize(EnemyStats newStats, Vector3 spawnWorldPosition)
    {
        stats = newStats;
        stats.pathfinding.goalPoint = GameManager.instance.GetEnemyGoalPoint();
        stats.pathfinding.spawnPoint = spawnWorldPosition;
        transform.position = spawnWorldPosition;
        SetupMovementStrategy();
        gameObject.name = stats.enemyName;
        stats.record.isInitialized = true;
        GetComponent<CircleCollider2D>().radius = stats.circleColliderRadius;
    }

    private void SetupMovementStrategy()
    {
        if (stats.pathfinding.movementType == EnemyMovementType.FLYING) stats.pathfinding.movementStrategy = new FlyingStrategy(this);
        else if (stats.pathfinding.movementType == EnemyMovementType.GROUND) stats.pathfinding.movementStrategy = new GroundStrategy(this, FindAnyObjectByType<AStar>());
    }

    private void Update()
    {
        if (!stats.record.isInitialized || stats.pathfinding.movementStrategy == null) return;
        stats.pathfinding.movementStrategy.Move();
    }

    private void OnDestroy()
    {
        if (stats.pathfinding.movementStrategy != null) stats.pathfinding.movementStrategy.Cleanup();
    }

    public void TakeDamage(float damage)
    {
        stats.health -= damage;
        if (stats.health <= 0) Destroy(gameObject);
    }

    public void SetMovementStrategy(IEnemyMovementStrategy movementStrategy) => stats.pathfinding.movementStrategy = movementStrategy;

    public bool GetIsInitialized() => stats.record.isInitialized;
    public EnemyType GetEnemyType() => stats.record.enemyType;
    public float GetDistanceToGoal() => stats.pathfinding.movementStrategy != null ? stats.pathfinding.movementStrategy.GetDistanceToGoal() : 0f;
    public float GetHealth() => stats.health;
    public float GetSpeed() => stats.speed;
    public string GetName() => stats.enemyName;
    public int GetAnimationSpeedPercentage() => stats.animationSpeedPercentage;
    public Vector3 GetGoalPoint() => stats.pathfinding.goalPoint;
    public Vector3 GetSpawnPoint() => stats.pathfinding.spawnPoint;
}
