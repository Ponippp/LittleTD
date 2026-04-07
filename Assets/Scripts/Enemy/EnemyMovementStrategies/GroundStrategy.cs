using System.Collections.Generic;
using UnityEngine;

public class GroundStrategy : IEnemyMovementStrategy
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
        EventsManager.instance.gameEvents.OnTowerGridUpdated += RegenerateAStarPath;
    }

    private void RegenerateAStarPath()
    {
        AStarNode startNode = _astar.GetGridNode(_enemy.transform.position);
        AStarNode targetNode = _astar.GetGridNode(_enemy.GetGoalPoint());
        if (startNode == null || targetNode == null) return;
        _path = _astar.TryRunAStar(startNode, targetNode);
        if (_path != null && _path.Count > 0) _path.RemoveAt(_path.Count - 1);
    }

    public void Move()
    {
        Vector3 goalPoint = _enemy.GetGoalPoint();
        if (Vector3.Distance(_enemy.transform.position, goalPoint) < 0.1f) return;

        if (_path == null || _path.Count == 0)
        {
            Vector3 direction = (goalPoint - _enemy.transform.position).normalized;
            _enemy.transform.position += _enemy.GetSpeed() * Time.deltaTime * direction;
            return;
        }

        // Standard A* Movement
        Vector3 currentTarget = _path[^1];
        if (Vector3.Distance(_enemy.transform.position, currentTarget) < 0.1f)
        {
            _path.RemoveAt(_path.Count - 1);
        }
        else
        {
            Vector3 direction = (currentTarget - _enemy.transform.position).normalized;
            _enemy.transform.position += _enemy.GetSpeed() * Time.deltaTime * direction;
        }
    }

    public void Cleanup()
    {
        EventsManager.instance.gameEvents.OnTowerGridUpdated -= RegenerateAStarPath;
    }

    public float GetDistanceToGoal() => Utility.CalculatePathLength(_path);

}
