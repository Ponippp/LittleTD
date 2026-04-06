using System.Collections.Generic;
using UnityEngine;

public class GroundStrategy : IMovementStrategy
{
    private List<Vector3> _path;
    private readonly AStar _astar;
    private readonly Enemy _enemy;

    public GroundStrategy(Enemy enemy, AStar astar)
    {
        _enemy = enemy;
        _astar = astar;
        _path = null;
        RegenerateAStarPath();
    }

    private void RegenerateAStarPath()
    {
        AStarNode startNode = _astar.GetGridNode(_enemy.GetSpawnPoint());
        AStarNode targetNode = _astar.GetGridNode(_enemy.GetGoalPoint());
        if (startNode == null || targetNode == null) return;
        _path = _astar.TryRunAStar(startNode, targetNode);
    }

    public void Move()
    {
        if (Vector3.Distance(_enemy.transform.position, _enemy.GetGoalPoint()) < 0.1f) return;
        if (_path == null) RegenerateAStarPath();

        if (Vector3.Distance(_enemy.transform.position, _path[^1]) < 0.1f) _path.RemoveAt(_path.Count - 1); 
        else {
            Vector3 direction = (_path[^1] - _enemy.transform.position).normalized;
            _enemy.transform.position += _enemy.GetSpeed() * Time.deltaTime * direction;
        }
    }
    public float GetDistanceToGoal() => Utility.CalculatePathLength(_path);

}
