using UnityEngine;

public class FlyingStrategy : IMovementStrategy
{
    private readonly Enemy _enemy;
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

    public float GetDistanceToGoal() => Vector2.Distance(_enemy.transform.position, _enemy.GetGoalPoint());

}
