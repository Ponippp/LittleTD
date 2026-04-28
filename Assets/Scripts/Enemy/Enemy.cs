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
        public int coinsDroppedOnKill = 0;

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
        //we're using trigger colliders (hitbox can pass through other colliders) instead of nontrigger colliders (hitbox is physical, so colliders will bump) b/c enemies will never bump into towers in the first place b/c of astar
        GetComponent<CircleCollider2D>().radius = stats.circleColliderRadius; //sets the enemy hitbox. 
    }

    private void SetupMovementStrategy()
    {
        if (stats.pathfinding.movementType == EnemyMovementType.FLYING) stats.pathfinding.movementStrategy = new FlyingStrategy(this);
        //FindAnyObjectByType<AStar>() searches the scene for the one astar algo object and passes 
        //it to ground strategy for use. On awake(), we could store the astar algo and later pass the 
        //single astar instance into gameManager and do GameManager.instance.AStar
        else if (stats.pathfinding.movementType == EnemyMovementType.GROUND) stats.pathfinding.movementStrategy = new GroundStrategy(this, FindAnyObjectByType<AStar>());
    }

    private void Update() //runs every frame
    {
        //each enemy instance has its own stats
        if (!stats.record.isInitialized || stats.pathfinding.movementStrategy == null || !GameManager.instance.GetGameActive()) return;
        stats.pathfinding.movementStrategy.Move();
        if (EnemyAtEndOfJourney())
        {
            GameManager.instance.TrySubtractLife(1);
            Destroy(gameObject);
        }
    }

    private bool EnemyAtEndOfJourney() => Vector3.Distance(transform.position, GameManager.instance.GetEnemyGoalPoint()) < 0.1f;

    private void OnDestroy()
    {
        if (stats.pathfinding.movementStrategy != null) stats.pathfinding.movementStrategy.Cleanup();
    }

    public void TakeDamage(float damage)
    {
        stats.health -= damage;
        if (stats.health <= 0)
        {
            GameManager.instance.AddCoins(stats.coinsDroppedOnKill);
            Destroy(gameObject);
        }
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
