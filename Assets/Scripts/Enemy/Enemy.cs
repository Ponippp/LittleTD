using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    [SerializeField] private string enemyName = "DefaultEnemy";
    [SerializeField] private float health = 10f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private Vector3 spawnPoint;
    [SerializeField] private Vector3 goalPoint;
    [SerializeField] private EnemyMovementType movementType = EnemyMovementType.GROUND;
    private IEnemyMovementStrategy _movementStrategy;

    private void Start()
    {
        goalPoint = GameManager.instance.GetEnemyGoalPoint();
        spawnPoint = GameManager.instance.GetEnemySpawnPoint();
        transform.position = spawnPoint;
        if (movementType == EnemyMovementType.FLYING) _movementStrategy = new FlyingStrategy(this);
        else if (movementType == EnemyMovementType.GROUND) _movementStrategy = new GroundStrategy(this, FindAnyObjectByType<AStar>());
    }

    private void Update()
    {
        _movementStrategy.Move();
    }

    private void OnDestroy()
    {
        if (_movementStrategy != null) _movementStrategy.Cleanup();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
            Destroy(gameObject);
    }
    public void SetMovementStrategy(IEnemyMovementStrategy movementStrategy) => _movementStrategy = movementStrategy;
    
    public float GetDistanceToGoal() => _movementStrategy.GetDistanceToGoal();
    public float GetHealth() => health;
    public float GetSpeed() => speed;
    public string GetName() => enemyName;
    public Vector3 GetGoalPoint() => goalPoint;
    public Vector3 GetSpawnPoint() => spawnPoint;


}
