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
        //whenever OnTowerGridUpdated is called, RegenerateAStarPath will be called
        EventsManager.instance.gameEvents.OnTowerGridUpdated += RegenerateAStarPath;
    }

    private void RegenerateAStarPath()
    {
        AStarNode startNode = _astar.GetGridNode(_enemy.transform.position);
        AStarNode targetNode = _astar.GetGridNode(_enemy.GetGoalPoint());
        if (startNode == null || targetNode == null) return; //don't think targetNode would be null, but may as well check just in case
        _path = _astar.TryRunAStar(startNode, targetNode);
        //the waypoints are stored in path from goal to start, so we need to remove the last element of the list (which removes the start waypoint)
        //this is because the enemy is not directly on the start node (we estimate it), so if we left the start node in, the enemy would glitch back for half a step before going to the rest of the waypoints
        if (_path != null && _path.Count > 0) _path.RemoveAt(_path.Count - 1); //removeAt: builtin that removes the object at the passed index
    }

    public void Move()
    {
        Vector3 goalPoint = _enemy.GetGoalPoint();
        if (Vector3.Distance(_enemy.transform.position, goalPoint) < 0.1f) return;

        if (_path == null || _path.Count == 0)
        {
            Vector3 direction = (goalPoint - _enemy.transform.position).normalized; 
            //direction vector has a magnitude and a adirection, so we use .normalized
            //to cancel the magnitude out, leaving us with just a direction.
            //this is needed b/c if direction had a magnitude, then the next line would move different enemies 
            //at different speeds, with enemies farther from the goal moving faster than enemies closer to the goal
            _enemy.transform.position += _enemy.GetSpeed() * Time.deltaTime * direction;
            return;
        }

        // Standard A* Movement
        Vector3 currentTarget = _path[^1]; //^1 is C# syntax for "the last element of the list"
        if (Vector3.Distance(_enemy.transform.position, currentTarget) < 0.1f)
        {
            _path.RemoveAt(_path.Count - 1);
        }
        else
        {
            Vector3 direction = (currentTarget - _enemy.transform.position).normalized;
            //_enemy.transform.position is a vector, we're adding another vector to it 
            _enemy.transform.position += _enemy.GetSpeed() * Time.deltaTime * direction;
        }
    }

    public void Cleanup()
    {
        EventsManager.instance.gameEvents.OnTowerGridUpdated -= RegenerateAStarPath; // Unsubscribe from the event when the strategy is cleaned up to prevent memory leaks and unintended behavior.
    }

    public float GetDistanceToGoal() => Utility.CalculatePathLength(_path);

}
