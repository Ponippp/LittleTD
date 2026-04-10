using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStar : MonoBehaviour
{

    //-+- -+- -+- -+- -+- -+- -+- -+- -+- -+- -+- -+- -+- VARIABLES -+- -+- -+- -+- -+- -+- -+- -+- -+- -+- -+- -+- -+-

    //serialize field private encapsulates data but creates fields in unity we can modify
    [Header("NODES")]
    [SerializeField] private AStarNode[,] _allNodes;
    [SerializeField] private HashSet<AStarNode> _OPEN_nodes;
    [SerializeField] private HashSet<AStarNode> _CLOSED_nodes;
    [SerializeField] private AStarNode _startNode;
    [SerializeField] private AStarNode _targetNode;
    [SerializeField] private AStarNode _currentNode;
    [SerializeField] private List<AStarNode> _neighborNodes;
    //===================================================================================================================
    [Header("NODE GRID")]
    [SerializeField] private int _nodeGridHeight;
    [SerializeField] private int _nodeGridWidth;
    [SerializeField] private Vector3 _gridOriginPosition;
    //===================================================================================================================
    [Header("ASTAR ALGORITHM MODS")]
    [SerializeField] private int _costOfDiagonal;
    [SerializeField] private int _costOfHorizontal;
    [SerializeField] private int _weightOfGCost = 1;
    [SerializeField] private int _weightOfHCost = 1;
    [SerializeField] private bool _diagonalMovementDisabled;
    //===================================================================================================================
    [Header("ASTAR DEBUG PREFERENCES")]
    [SerializeField] private bool _debuggingActive;
    [SerializeField] private bool _runAStarInstantly;
    [SerializeField] private int _maxIterationsAllowed = 10000;
    [SerializeField] private float _eachNeighborCheckDelay;
    [SerializeField] private float _eachCheckDelay;
    [SerializeField] private float _debugLineDuration = 100;
    [SerializeField] private int _currentIterationCount = 0;
    //===================================================================================================================
    [Header("REFERENCES")]
    [SerializeField] private Tilemap _floor;

    //-+- -+- -+- -+- -+- -+- -+- -+- -+- -+- -+- -+- BASE FUNCTIONS -+- -+- -+- -+- -+- -+- -+- -+- -+- -+- -+- -+- -+-

    private void OnEnable()
    {
        //+= subscribes a func to the event, -= unsubscribes
        //OnSetupNewAStarGrid is an action in gameEvents class, and the one instance of gameEvents is in EventsManager
        EventsManager.instance.gameEvents.OnSetupNewAStarGrid += SetupAStar;
    }
    private void OnDisable()
    {
        EventsManager.instance.gameEvents.OnSetupNewAStarGrid -= SetupAStar;
    }


    //-+- -+- -+- -+- -+- -+- -+- -+- -+- -+- -+- -+- CLASS FUNCTIONS -+- -+- -+- -+- -+- -+- -+- -+- -+- -+- -+- -+- -+-

    public void SetupAStar(int height, int width, Vector3 origin, Tilemap floor)
    {
        _floor = floor;
        _nodeGridHeight = height;
        _nodeGridWidth = width;
        _gridOriginPosition = origin;
        _allNodes = new AStarNode[_nodeGridWidth, _nodeGridHeight];
        _OPEN_nodes = new HashSet<AStarNode>();
        _CLOSED_nodes = new HashSet<AStarNode>();
        _neighborNodes = new List<AStarNode>();
        for (int x = 0; x < _nodeGridWidth; x++)
        {
            for (int y = 0; y < _nodeGridHeight; y++)
            {
                _allNodes[x, y] = new AStarNode(x, y, AStarState.UNTESTED);
                MakeTraversableIfTileNotNull(_allNodes[x, y], _floor);

                if (_debuggingActive)
                {
                    _allNodes[x, y].f_cost_text = Utility.CreateWorldText((_allNodes[x, y].x + _allNodes[x, y].y).ToString(), null, _allNodes[x, y].F_cost().ToString(), GetWorldPos(x, y) + new Vector3(1, 1) * .5f, 225, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center, 0, .015f);
                    _allNodes[x, y].g_cost_text = Utility.CreateWorldText((_allNodes[x, y].x + _allNodes[x, y].y).ToString(), null, _allNodes[x, y].g_cost.ToString(), GetWorldPos(x, y) + new Vector3(.3f, .7f), 150, Color.white, TextAnchor.LowerRight, TextAlignment.Center, 0, 0.01f);
                    _allNodes[x, y].h_cost_text = Utility.CreateWorldText((_allNodes[x, y].x + _allNodes[x, y].y).ToString(), null, _allNodes[x, y].h_cost.ToString(), GetWorldPos(x, y) + new Vector3(1, 1) * .7f, 150, Color.white, TextAnchor.LowerLeft, TextAlignment.Center, 0, 0.01f);
                    if (_allNodes[x, y].aStarState == AStarState.PERMA_UNTRAVERSABLE) SetNodeDebugTextColor(_allNodes[x, y], Color.black);
                    if (_allNodes[x, y].aStarState == AStarState.CURRENTLY_UNTRAVERSABLE) SetNodeDebugTextColor(_allNodes[x, y], Color.black);
                    if (_allNodes[x, y].aStarState == AStarState.UNTESTED) SetNodeDebugTextColor(_allNodes[x, y], Color.white);
                    Debug.DrawLine(GetWorldPos(x, y), GetWorldPos(x, y + 1), Color.white, _debugLineDuration);
                    Debug.DrawLine(GetWorldPos(x, y), GetWorldPos(x + 1, y), Color.white, _debugLineDuration);
                }
            }
        }
        if (_debuggingActive)
        {
            Debug.DrawLine(GetWorldPos(0, _nodeGridHeight), GetWorldPos(_nodeGridWidth, _nodeGridHeight), Color.white, _debugLineDuration);
            Debug.DrawLine(GetWorldPos(_nodeGridWidth, 0), GetWorldPos(_nodeGridWidth, _nodeGridHeight), Color.white, _debugLineDuration);
        }
    }

    //===================================================================================================================
    // RUN ASTAR
    //===================================================================================================================
    public List<Vector3> TryRunAStar(AStarNode startNode, AStarNode targetNode)
    {
        List<Vector3> ret = new(); //new() is shorthand for new List<Vector3>()
        Physics2D.SyncTransforms(); //Physics2D runs on a set frame rate, but game runs on a modifiable frame rate. If we set game frame rate to be 2x Physics2D frame rate, we could be caught out of sync. This safeguards that
        ResetAStar();
        if (_runAStarInstantly) ret = RunAStar(startNode, targetNode);
        return ret;
    }
    private List<Vector3> RunAStar(AStarNode startNode, AStarNode targetNode)
    {
        // GUARD
        if (targetNode == null) { Debug.Log("target null"); return null; }
        if (startNode == null) { Debug.Log("startNode null"); return null; }
        
        // VARS
        _currentIterationCount = 0;
        _startNode = startNode;
        _targetNode = targetNode;

        // REMOVED AS WE ALREADY DO THIS IN ENEMY AND TOWER PLACEMENT CODE
        // SETUP START NODE
        // Important: check if start itself is blocked by a tower/wall
        // MakeTraversableIfTileNotNull(startNode, _floor);
        // if (startNode.aStarState == AStarState.CURRENTLY_UNTRAVERSABLE || startNode.aStarState == AStarState.PERMA_UNTRAVERSABLE)
        // {
        //     return null;
        // }

        _startNode.g_cost = _weightOfGCost * GetNodeG_Cost(startNode, startNode);
        _startNode.h_cost = _weightOfHCost * GetNodeH_Cost(startNode, targetNode);
        if (_debuggingActive) UpdateNodeDebugText(_startNode);
        _OPEN_nodes.Add(_startNode);
        // RUN A* 
        while (true)
        {
            // GUARD
            _currentIterationCount++;
            if (_currentIterationCount > _maxIterationsAllowed) { Debug.Log("Exceeded maximum iterations"); return null; }
            // GET NEW CURRENT NODE
            _currentNode = GetNodeWithLowestF_Cost(_OPEN_nodes);
            if (_currentNode == null) { return null; } // Debug.Log($"nodeHashSet is empty on iteration: {_currentIterationCount}");
            // UPDATE CURRENT NODE
            RemoveNodeFromOpenAddToClosed(_currentNode);
            // TRY RETURN
            if (_currentNode == _targetNode)
            {
                if (_debuggingActive) LightUpPath(GetQuickestPath(_currentNode));
                return GetQuickestPositionPath(_currentNode);
            }
            // GET NEIGHBORS
            _neighborNodes = GetNeighborsOfNode(_currentNode);
            if (_neighborNodes == null) { Debug.Log("Node was found to be null when tried get neighbors"); return null; }
            // PROCESS NEIGHBORS
            foreach (AStarNode neighbor in _neighborNodes)
            {
                // SKIP BAD NODES
                if (neighbor.aStarState == AStarState.CLOSED || neighbor.aStarState == AStarState.PERMA_UNTRAVERSABLE) continue;
                // DYNAMICALLY TRY CHANGE NODE STATE
                MakeTraversableIfTileNotNull(neighbor, _floor);
                // SKIP BAD NODES
                if (neighbor.aStarState == AStarState.CURRENTLY_UNTRAVERSABLE) continue;
                // TRY UPDATE NEIGHBORS
                int newGCostToNeighbor = _currentNode.g_cost + GetNodeG_Cost(neighbor, _currentNode);
                // if (neighbor.aStarState != AStarState.OPEN || PathToNeighborIsShorter(_currentNode, neighbor, _targetNode))
                if (neighbor.aStarState != AStarState.OPEN || newGCostToNeighbor < neighbor.g_cost)
                {
                    // UPDATE NODE COSTS
                    // neighbor.g_cost = _weightOfGCost * (_currentNode.g_cost + GetNodeG_Cost(neighbor, _currentNode));
                    neighbor.g_cost = newGCostToNeighbor;
                    neighbor.h_cost = _weightOfHCost * GetNodeH_Cost(neighbor, _targetNode);
                    if (_debuggingActive) UpdateNodeDebugText(neighbor);
                    // SET NEW PARENT
                    neighbor.parent = _currentNode;
                    // NEIGHBOR TO OPEN IF NOT ALREADY
                    if (neighbor.aStarState != AStarState.OPEN)
                    {
                        neighbor.aStarState = AStarState.OPEN;
                        _OPEN_nodes.Add(neighbor);
                    }
                }
            }
        }
    }

    //===================================================================================================================
    // GET POSITIONS
    //===================================================================================================================
    private Vector3 GetWorldPos(int x, int y)
    {
        return new Vector3(x, y) + _gridOriginPosition;
    }
    // private Vector3 GetWorldPos(AStarNode node)
    // {
    //     return GetWorldPos(node.x, node.y);
    // }
    private Vector3 GetWorldPos(AStarNode node)
    {
        return new Vector3(node.x + _gridOriginPosition.x, node.y + _gridOriginPosition.y, 0f);
    }
    private Vector3Int GetIntWorldPos(AStarNode node)
    {
        int x = Mathf.FloorToInt(node.x + _gridOriginPosition.x);
        int y = Mathf.FloorToInt(node.y + _gridOriginPosition.y);
        return new Vector3Int(x, y, 0); // Assuming z = 0 for 2D tilemaps
    }

    //===================================================================================================================
    // GET NODES
    //===================================================================================================================
    private AStarNode GetNodeWithLowestF_Cost(HashSet<AStarNode> nodeHashSet)
    {
        if (nodeHashSet == null || nodeHashSet.Count == 0) return null;
        AStarNode lowestF_Cost = null;
        // FIND LOWEST F COST
        foreach (AStarNode node in nodeHashSet)
        {
            if (lowestF_Cost == null || node.F_cost() <= lowestF_Cost.F_cost()) lowestF_Cost = node;
        }
        return lowestF_Cost;
    }
    private List<AStarNode> GetNeighborsOfNode(AStarNode node)
    {
        if (node == null) return null;
        // VARS
        List<AStarNode> neighbors = new List<AStarNode>();
        AStarNode nextToBeAdded;
        // ADD NEIGHBORS
        nextToBeAdded = GetGridNode(node.x, node.y + 1); // UP
        if (nextToBeAdded != null) neighbors.Add(nextToBeAdded);
        nextToBeAdded = GetGridNode(node.x + 1, node.y); // RIGHT
        if (nextToBeAdded != null) neighbors.Add(nextToBeAdded);
        nextToBeAdded = GetGridNode(node.x, node.y - 1); // DOWN
        if (nextToBeAdded != null) neighbors.Add(nextToBeAdded);
        nextToBeAdded = GetGridNode(node.x - 1, node.y); // LEFT
        if (nextToBeAdded != null) neighbors.Add(nextToBeAdded);
        if (!_diagonalMovementDisabled)
        {
            nextToBeAdded = GetGridNode(node.x - 1, node.y - 1); // DOWN LEFT
            if (nextToBeAdded != null) neighbors.Add(nextToBeAdded);
            nextToBeAdded = GetGridNode(node.x + 1, node.y - 1); // DOWN RIGHT
            if (nextToBeAdded != null) neighbors.Add(nextToBeAdded);
            nextToBeAdded = GetGridNode(node.x - 1, node.y + 1); // UP LEFT
            if (nextToBeAdded != null) neighbors.Add(nextToBeAdded);
            nextToBeAdded = GetGridNode(node.x + 1, node.y + 1); // UP RIGHT
            if (nextToBeAdded != null) neighbors.Add(nextToBeAdded);
        }

        return neighbors;
    }
    public AStarNode GetGridNode(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < _nodeGridWidth && y < _nodeGridHeight)
        {
            return _allNodes[x, y]; //return node (x, y) in the list _allNodes
        }
        return null;
    }
    public AStarNode GetGridNode(Vector3 worldPos)
    {
        int x, y;
        GetXY(worldPos, out x, out y);
        return GetGridNode(x, y);
    }

    //===================================================================================================================
    // MISC
    //===================================================================================================================
    private void GetXY(Vector3 worldPos, out int x, out int y) //out type in params is so we dont have to return a tuple; any changes made to x and y in this func propagate and are returned whne we use out data type
    {
        //Floor(5.7)=5.0, but FloorToInt(5.7)=5
        //by default, (0,0) in world grid is bottom left of our main camera because our grid is automatically made in only +x and +y directions.
        //Because of this, we subtract gridOriginPosition to make (0,0) the center of our main camera
        //.x and .y are methods of the Vector3 class that return the respective component of the vector
        x = Mathf.FloorToInt((worldPos - _gridOriginPosition).x);
        y = Mathf.FloorToInt((worldPos - _gridOriginPosition).y);
    }
    private void RemoveNodeFromOpenAddToClosed(AStarNode node)
    {
        if (node == null) return;
        _OPEN_nodes.Remove(node);
        _CLOSED_nodes.Add(node);
        node.aStarState = AStarState.CLOSED;
        if (_debuggingActive) SetNodeDebugTextColor(node, Color.gray);
    }
    private bool PathToNeighborIsShorter(AStarNode current, AStarNode neighbor, AStarNode targetNode)
    {
        if (neighbor.F_cost() > current.g_cost + GetNodeG_Cost(neighbor, current) + GetNodeH_Cost(neighbor, targetNode)) return true;
        return false;
    }
    private int GetNodeG_Cost(AStarNode node, AStarNode startNode)
    {
        if (node == null || startNode == null) return -1;

        int distanceX = Math.Abs(node.x - startNode.x);
        int distanceY = Math.Abs(node.y - startNode.y);

        int diagonalMoves = Math.Min(distanceX, distanceY); // MINIMUM NUMBER = MAX DIAG MOVES
        int horizontalMoves = Math.Abs(distanceX - distanceY); // LEFTOVER MOVES 
        //diagonal functionality deprecated, still works, but we could modify this to make it simpler
        return diagonalMoves * _costOfDiagonal + horizontalMoves * _costOfHorizontal;
    }
    private int GetNodeH_Cost(AStarNode node, AStarNode endNode)
    {
        if (node == null || endNode == null) return -1;

        int distanceX = Math.Abs(node.x - endNode.x);
        int distanceY = Math.Abs(node.y - endNode.y);

        int diagonalMoves = Math.Min(distanceX, distanceY); // MINIMUM NUMBER = MAX DIAG MOVES
        int horizontalMoves = Math.Abs(distanceX - distanceY); // LEFTOVER MOVES 
        //diagonal functionality deprecated, still works, but we could modify this to make it simpler
        return diagonalMoves * _costOfDiagonal + horizontalMoves * _costOfHorizontal;
    }

    //===================================================================================================================
    // RESET
    //===================================================================================================================
    private void ResetAStar()
    {
        _currentIterationCount = 0;
        for (int x = 0; x < _nodeGridWidth; x++)
        {
            for (int y = 0; y < _nodeGridHeight; y++)
            {
                if (_allNodes[x, y] != null)
                {
                    if (_allNodes[x, y].aStarState != AStarState.PERMA_UNTRAVERSABLE)
                    {
                        _allNodes[x, y].aStarState = AStarState.UNTESTED;
                        _allNodes[x, y].g_cost = 0;
                        _allNodes[x, y].h_cost = 0;
                        _allNodes[x, y].parent = null;
                        if (_debuggingActive)
                        {
                            if (_allNodes[x, y].f_cost_text != null) _allNodes[x, y].f_cost_text.text = "0";
                            if (_allNodes[x, y].g_cost_text != null) _allNodes[x, y].g_cost_text.text = "0";
                            if (_allNodes[x, y].h_cost_text != null) _allNodes[x, y].h_cost_text.text = "0";
                            SetNodeDebugTextColor(_allNodes[x, y], Color.white);
                        }
                    }
                }
            }
        }
        _OPEN_nodes?.Clear(); //?.Clear() means only run Clear() if the thing before the . is not null
        _CLOSED_nodes?.Clear();
        _currentNode = null;
        _neighborNodes?.Clear();
    }

    //===================================================================================================================
    // SET NODES
    //===================================================================================================================
    private void SetStartingNode(AStarNode node)
    {
        // GUARD
        if (node == null || node.aStarState == AStarState.PERMA_UNTRAVERSABLE || node.aStarState == AStarState.CURRENTLY_UNTRAVERSABLE) return;
        // RESET PREVIOUS STARTNODE COLOR
        if (_startNode != null) if (_debuggingActive) SetNodeDebugTextColor(_startNode, Color.white);
        // SET NEW STARTNODE COLOR
        if (_debuggingActive) SetNodeDebugTextColor(node, Color.blue);
        _startNode = node;
    }
    public void SetStartingNode(Vector3 pos)
    {
        SetStartingNode(GetGridNode(pos));
    }
    private void SetTargetNode(AStarNode node)
    {
        // GUARD
        if (node == null || node.aStarState == AStarState.PERMA_UNTRAVERSABLE || node.aStarState == AStarState.CURRENTLY_UNTRAVERSABLE) return;
        // RESET PREVIOUS TARGETNODE COLOR
        if (_targetNode != null) if (_debuggingActive) SetNodeDebugTextColor(_targetNode, Color.white);
        // SET NEW TARGETNODE COLOR
        if (_debuggingActive) SetNodeDebugTextColor(node, Color.red);
        _targetNode = node;
    }
    public void SetTargetNode(Vector3 pos)
    {
        SetTargetNode(GetGridNode(pos));
    }

    //===================================================================================================================
    // PATH
    //===================================================================================================================
    private List<AStarNode> GetQuickestPath(AStarNode finalNode)
    {
        if (finalNode == null) return null; //if finalnode is null, no hay path
        List<AStarNode> path = new();
        AStarNode current = finalNode;
        do
        {
            path.Add(current);
            current = current.parent;
        }
        while (current != null && current.parent != null);
        return path; //returns astar nodes
    }
    private List<Vector3> GetQuickestPositionPath(AStarNode finalNode)
    {
        if (finalNode == null) return null;
        List<Vector3> path = new();
        AStarNode current = finalNode;
        do
        {
            path.Add(new Vector3(current.x + .5f, current.y + .5f) + _gridOriginPosition);
            current = current.parent;
        }
        while (current != null && current.parent != null);
        return path; //returns physical points that the enemy has to traverse
    }
    private void LightUpPath(List<AStarNode> path)
    {
        if (path == null) return;
        foreach (AStarNode node in path)
        {
            SetNodeDebugTextColor(node, Color.yellow);
        }
        // SET ENDPOINT NODE COLOR
        SetNodeDebugTextColor(_startNode, Color.blue);
        SetNodeDebugTextColor(_targetNode, Color.red);
    }

    //===================================================================================================================
    // UPDATE NODES
    //===================================================================================================================
    private void SetNodeDebugTextColor(AStarNode node, Color color)
    {
        // Check if the text references are actually set before trying to access them!
        if (node.f_cost_text != null) node.f_cost_text.color = color;
        if (node.g_cost_text != null) node.g_cost_text.color = color;
        if (node.h_cost_text != null) node.h_cost_text.color = color;
    }

    private void UpdateNodeDebugText(AStarNode node)
    {
        if (node == null) return;
        if (node.g_cost_text != null) node.g_cost_text.text = node.g_cost.ToString();
        if (node.h_cost_text != null) node.h_cost_text.text = node.h_cost.ToString();
        if (node.f_cost_text != null) node.f_cost_text.text = node.F_cost().ToString();
    }
    private void MakeTraversableIfTileNotNull(AStarNode node, Tilemap tilemap) //alt name:  MakeTileUntestedIfFloor
    {
        Vector3 worldPosition = GetWorldPos(node); // Get the node's world position
        //World2Cell converts Vector3 to Vector3Int, so now you can only have vectors like <1,1,1> instead of <1.5,1.5,1.5>
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition); // Convert to tilemap cell position
        if (tilemap.HasTile(cellPosition)) //if there is a visible tile at cellPosition:
        {
            node.aStarState = AStarState.UNTESTED;
            //rn it's a vector3Int, so it's on the corner of 4 tiles. We add the 0.5 Vector3 offset so it's in the center of one tile. 
            //Now the whole if statement says "if this tile overlaps w/ the wall layer, make the tile perma untraversable"
            if (Physics2D.OverlapCircle(worldPosition + new Vector3(.5f, .5f), .1f, Utility.WALL__LAYERMASK))
            {
                node.aStarState = AStarState.PERMA_UNTRAVERSABLE;
            } 
            else if (Physics2D.OverlapCircle(worldPosition + new Vector3(.5f, .5f), .1f, Utility.TOWER__LAYERMASK))
            {
                node.aStarState = AStarState.CURRENTLY_UNTRAVERSABLE;
            }
            // else if (Physics2D.OverlapCircle(worldPosition + new Vector3(.5f, .5f), .1f, Utility.OBSTACLE__LAYERMASK))
            // {
            //     Debug.Log("OBSTA:CLE");
            //     node.aStarState = AStarState.CURRENTLY_UNTRAVERSABLE;
            // }
            // else if (Physics2D.OverlapCircle(worldPosition + new Vector3(.5f, .5f), .1f, Utility.INTERACTABLE__LAYERMASK))
            // {
            //     Debug.Log("INTERACTABLE LAYA");
            //     node.aStarState = AStarState.CURRENTLY_UNTRAVERSABLE;
            // }
        }
        else //if there is no visible tile at cellPosition (outside of map):
        {
            node.aStarState = AStarState.PERMA_UNTRAVERSABLE;
            Destroy(node.f_cost_text);
            Destroy(node.g_cost_text);
            Destroy(node.h_cost_text);
        }
    }

    //===================================================================================================================
    // ALGO MODS
    //===================================================================================================================
    public int CostOfDiagonal
    {
        set { _costOfDiagonal = value; }
    }
    public int CostOfHorizontal
    {
        set { _costOfHorizontal = value; }
    }
    public int WeightOfGCost
    {
        set { _weightOfGCost = value; }
    }
    public int WeightOfHCost
    {
        set { _weightOfHCost = value; }
    }

    //===================================================================================================================
    // ASTAR DEBUG PREFERENCES
    //===================================================================================================================
    public bool RunAStarInstantly
    {
        set { _runAStarInstantly = value; }
    }
    public float EachNeighborCheckDelay
    {
        set { _eachNeighborCheckDelay = value; }
    }
    public float EachCheckDelay
    {
        set { _eachCheckDelay = value; }
    }
    public bool DebuggingActive
    {
        set { _debuggingActive = value; }
    }

}
