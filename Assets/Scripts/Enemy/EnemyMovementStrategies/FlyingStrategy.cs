using UnityEngine;

public class FlyingStrategy : IEnemyMovementStrategy
{
    private readonly Enemy _enemy; //readonly: once you set the enemy in the constructor, then you can't set enemy to be another enemy instance, but you can modify the exsiting enemy instance
    public FlyingStrategy(Enemy enemy)
    {
        _enemy = enemy;
    }
    public void Move()
    {
        _enemy.transform.position = Vector3.MoveTowards(
            _enemy.transform.position, 
            _enemy.GetGoalPoint(), 
            _enemy.GetSpeed() * Time.deltaTime
        );
    }

    public void Cleanup()
    {
        // No cleanup needed for flying strategy b/c we don't need to unsubscribe from Astar
    }

    public float GetDistanceToGoal() => Vector2.Distance(_enemy.transform.position, _enemy.GetGoalPoint());

}
